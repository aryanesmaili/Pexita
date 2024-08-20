using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Pexita.Utility.Exceptions.PaymentException;

namespace Pexita.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly string _apiKey;
        private readonly bool _isTest;
        private const string _requestNewTransactionAPI = "https://api.idpay.ir/v1.1/payment";
        private const string _paymentVerificationAPI = "https://api.idpay.ir/v1.1/payment/verify";
        private const string _paymentInquiry = "https://api.idpay.ir/v1.1/payment/inquiry";
        private readonly string _CallbackAddress;
        private readonly AppDBContext _Context;
        private readonly IMapper _mapper;

        public PaymentService(string APIKey, string CallbackAddress, bool isTest, AppDBContext Context, IMapper mapper)
        {
            _apiKey = APIKey;
            _isTest = isTest;
            _CallbackAddress = CallbackAddress;
            _Context = Context;
            _mapper = mapper;
        }
        /// <summary>
        /// Get the list of all payments.
        /// </summary>
        /// <returns>The list of all payments.</returns>
        public async Task<List<PaymentModel>> GetPayments()
        {
            var result = await _Context.Payments.ToListAsync();
            return result.Count > 0 ? result : throw new EmptyResultException();

        }
        /// <summary>
        /// Get info about a certain payment by its Id.
        /// </summary>
        /// <param name="id">id of the payment.</param>
        /// <returns>database record demonstrating that payment.</returns>
        public async Task<PaymentModel> GetPayment(int id)
        {
            return await _Context.Payments.SingleAsync(p => p.ID == id);
        }

        /// <summary>
        /// Sends a payment request to the IDPay API and returns the payment link if successful.
        /// </summary>
        /// <param name="paymentRequest">The payment request object containing details such as order ID, amount, and user information.</param>
        /// <returns>A task representing the asynchronous operation, containing the payment link if the request was successful.</returns>
        public async Task<string> SendPaymentRequest(PaymentRequest paymentRequest, string requestingUsername)
        {
            UserModel user = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
            bool isAdmin = user.Role == "admin";
            if (!isAdmin || user.Username != requestingUsername)
            {
                throw new NotAuthorizedException();
            }

            // Create a new HttpClient instance to communicate with the IDPay API.
            using (HttpClient client = new())
            {
                // Add API key to the request headers.
                client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

                // If in test mode, add a header to indicate sandbox mode.
                if (_isTest)
                {
                    client.DefaultRequestHeaders.Add("X-SANDBOX", "1");
                }

                // Prepare the request body as a JSON object.
                Dictionary<string, dynamic> requestBody = new()
                {
                    { "order_id", paymentRequest.OrderId },
                    { "amount", paymentRequest.Amount },
                    { "name", paymentRequest.ShoppingCart.User.Username },
                    { "phone", paymentRequest.ShoppingCart.User.PhoneNumber },
                    { "mail", paymentRequest.ShoppingCart.User.Email },
                    { "desc", paymentRequest.Description ?? "" },
                    { "callback", _CallbackAddress}
                };
                string requestBodyJson = JsonSerializer.Serialize(requestBody);

                // Create the HTTP content from the request body.
                HttpContent content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                // Send a POST request to the IDPay API to create a new transaction.
                HttpResponseMessage response = await client.PostAsync(_requestNewTransactionAPI, content);

                // Check the status code of the response.
                if (response.StatusCode == HttpStatusCode.Created) // 201   // means transaction has successfully been created 
                {
                    // Deserialize the response JSON to extract the payment creation success details.
                    PaymentCreationSuccessResponse? deserializedResponse = await JsonSerializer.DeserializeAsync<PaymentCreationSuccessResponse>(await response.Content.ReadAsStreamAsync());

                    // Create a new PaymentModel object to store the payment details in the database.
                    PaymentModel payment = new()
                    {
                        PaymentOrderID = paymentRequest.OrderId,
                        PaymentLink = deserializedResponse!.Link,
                        Amount = paymentRequest.Amount,
                        TransactionID = deserializedResponse!.Id,
                        DateIssued = DateTime.UtcNow,
                        ShoppingCartID = paymentRequest.ShoppingCartID,
                        ShoppingCart = paymentRequest.ShoppingCart,
                    };

                    // Return the payment link to redirect the user to the payment page.
                    return deserializedResponse.Link;
                }
                else
                {
                    // If the request was not successful, deserialize the error response and handle it.
                    var errorResponse = JsonSerializer.Deserialize<PaymentErrorResponse>(await response.Content.ReadAsStreamAsync());
                    PaymentExceptionManager(response.StatusCode, errorResponse!);

                    // Throw an exception with the error message.
                    throw new Exception($"{response.StatusCode}: {errorResponse.Message}");
                }
            }
        }

        /// <summary>
        /// Validates the outcome of a payment based on the response received from the IDPay API.
        /// </summary>
        /// <param name="idpayResponse">The response received from the IDPay API.</param>
        /// <returns>A task representing the asynchronous operation, indicating whether the validation was successful.</returns>
        public async Task<bool> PaymentOutcomeValidation(PaymentOutcomeValidationResponse idpayResponse)
        {
            // Retrieve the payment record from the database based on the transaction ID.
            PaymentModel payment = _Context.Payments.Single(i => i.TransactionID == idpayResponse.TransactionID);

            // Map the properties from the IDPay response to the PaymentModel entity.
            payment = _mapper.Map<PaymentModel>(idpayResponse);

            // Additional verification logic can be added here before approving the payment.

            // Create a new HttpClient instance to communicate with the IDPay API.
            using (HttpClient client = new())
            {
                // Set the API key in the request headers.
                client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

                // If in test mode, add a header to indicate sandbox mode.
                if (_isTest)
                {
                    client.DefaultRequestHeaders.Add("X-SANDBOX", "1");
                }

                // Prepare the request body as a JSON object.
                Dictionary<string, string> requestBody = new()
                {
                    {"id", payment.TransactionID!},
                    {"order_id", payment.PaymentOrderID!}
                };
                string requestBodyJson = JsonSerializer.Serialize(requestBody);
                HttpContent content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                // Send a POST request to the payment verification API endpoint.
                HttpResponseMessage response = await client.PostAsync(_paymentVerificationAPI, content);

                // Check if the request was successful.
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response JSON to extract the verification date.
                    var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStreamAsync())!;
                    string verificationDateUnixString = responseData["verify"];

                    // Convert the verification date from Unix timestamp to DateTime and update the payment record.
                    payment.PaymentVerificationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(verificationDateUnixString)).DateTime;
                }
                else
                {
                    // If the request was not successful, deserialize the error response and handle it.
                    var errorResponse = JsonSerializer.Deserialize<PaymentErrorResponse>(await response.Content.ReadAsStreamAsync());
                    PaymentExceptionManager(response.StatusCode, errorResponse!);
                }
            }
            // Create a new Order with this new succesful payment
            OrdersModel order = new()
            {
                DateIssued = payment.DateTimePaid!.Value,
                Status = OrdersModel.OrderStatus.Preparing,
                Payment = payment,
                PaymentID = payment.ID,
                UserId = payment.ShoppingCart!.User.ID,
                ShoppingCart = payment.ShoppingCart,
                ShoppingCartID = payment.ShoppingCart.ID,
                User = payment.ShoppingCart.User,
            };
            order.BrandOrders = GetBrandsFromCart(order, payment.ShoppingCart);
            _Context.Orders.Add(order);

            // Save changes to the database.
            _Context.SaveChanges();

            // Return true to indicate that the payment outcome validation was successful.
            return true;
        }
        /// <summary>
        /// Changes the status of an order to "Sent".
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="requestingUsername"></param>
        /// <returns></returns>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<bool> ToggleOrderToSent(int orderID, string requestingUsername)
        {
            UserModel user = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
            bool isAdmin = user.Role == "admin";
            if (!isAdmin || user.Username != requestingUsername)
            {
                throw new NotAuthorizedException();
            }

            var order = await _Context.Orders.SingleAsync(order => order.ID == orderID);
            order.Status = OrdersModel.OrderStatus.Sent;
            _Context.SaveChanges();
            return true;
        }
        /// <summary>
        /// Extracts the brands from a given shopping cart.
        /// </summary>
        /// <param name="order"> the order </param>
        /// <param name="shoppingCart"> the shopping cart to extract brands from</param>
        /// <returns></returns>
        private List<BrandOrder> GetBrandsFromCart(OrdersModel order, ShoppingCartModel shoppingCart)
        {
            // Initialize a list to store brand orders
            List<BrandOrder> brandOrders = new();

            // Retrieve the IDs of brands associated with products in the shopping cart
            List<int> brandIDs = shoppingCart.CartItems.Select(ci => ci.Product!.BrandID).ToList();

            // Retrieve the brands from the database based on the IDs
            var brands = _Context.Brands.Where(brand => brandIDs.Contains(brand.ID)).ToList();

            // Iterate through each brand and create a brand order for the order
            foreach (var brandItem in brands)
            {
                // Create a new brand order instance
                BrandOrder brandOrder = new()
                {
                    Brand = brandItem,            // Set the brand associated with the brand order
                    Order = order,                // Set the order associated with the brand order
                    BrandID = brandItem.ID,       // Set the ID of the brand
                    OrderID = order.ID           // Set the ID of the order
                };

                // Add the brand order to the list of brand orders
                brandOrders.Add(brandOrder);
            }

            // Add all brand orders to the context
            _Context.BrandOrder.AddRange(brandOrders);

            // Save changes to the database
            _Context.SaveChanges();

            // Return the list of brand orders created
            return brandOrders;
        }

        static int ExtractNumberFromString(string input)
        {
            string pattern = @"\d+"; // \d matches any digit, and + matches one or more occurrences
            Match match = Regex.Match(input, pattern);
            return int.Parse(match.Value);
        }
        /// <summary>
        /// manages the exceptions that can be thrown throughout payment.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="innerResponse"></param>
        /// <exception cref="AmountLessThanMinimumException"></exception>
        /// <exception cref="AmountExceedsMaximumException"></exception>
        /// <exception cref="AmountExceedsLimitException"></exception>
        /// <exception cref="CallbackDomainMismatchException"></exception>
        /// <exception cref="InvalidCallbackAddressException"></exception>
        /// <exception cref="UnexpectedErrorException"></exception>
        /// <exception cref="UserBlockedException"></exception>
        /// <exception cref="ApiKeyNotFoundException"></exception>
        /// <exception cref="IpMismatchException"></exception>
        /// <exception cref="WebServiceNotApprovedException"></exception>
        /// <exception cref="BankAccountNotApprovedException"></exception>
        /// <exception cref="BankAccountInactiveException"></exception>
        /// <exception cref="TransactionNotCreatedException"></exception>
        private static void PaymentExceptionManager(HttpStatusCode response, PaymentErrorResponse innerResponse)
        {
            if (response == HttpStatusCode.NotAcceptable) // 406
            {
                switch (innerResponse!.Code)
                {
                    case 34:
                        int minimumAcceptable = ExtractNumberFromString(innerResponse.Message);
                        throw new AmountLessThanMinimumException(minimumAcceptable);
                    case 35:
                        int maximumAcceptable = ExtractNumberFromString(innerResponse.Message);
                        throw new AmountExceedsMaximumException(maximumAcceptable);
                    case 36:
                        throw new AmountExceedsLimitException();
                    case 38:
                        throw new CallbackDomainMismatchException();
                    case 39:
                        throw new InvalidCallbackAddressException();
                    default:
                        throw new UnexpectedErrorException();
                }
            }
            else if (response == HttpStatusCode.Forbidden)
            {
                switch (innerResponse!.Code)
                {
                    case 11:
                        throw new UserBlockedException();
                    case 12:
                        throw new ApiKeyNotFoundException();
                    case 13:
                        string pattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b"; // This pattern matches the typical format of an IPv4 address
                        string ipAddress = Regex.Match(innerResponse.Message, pattern).Value;
                        throw new IpMismatchException(ipAddress);
                    case 14:
                        throw new WebServiceNotApprovedException();
                    case 21:
                        throw new BankAccountNotApprovedException();
                    case 24:
                        throw new BankAccountInactiveException();
                    default:
                        throw new UnexpectedErrorException();
                }
            }
            else if (response == HttpStatusCode.MethodNotAllowed)
            {
                throw new TransactionNotCreatedException();

            }
            else
            {
                throw new UnexpectedErrorException();
            }

        }
        private static string GenerateOrderID(int BrandID, int ProductID, int UserID, DateTime Datetime)
        {
            return $"{BrandID}{ProductID}{UserID}{Datetime}";
        }

        public bool IsValidPayment(int id)
        {
            return _Context.Payments.Find(id) != null;
        }
        public bool AreValidPayments(List<PaymentDTO> payments)
        {
            return payments.All(x => IsValidPayment(x.ID));
        }
    }
    /// <summary>
    /// Info required for requesting a payment from IDPay as an object.
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// order id generated by our application.
        /// </summary>
        public required string OrderId { get; set; }
        /// <summary>
        /// amount to be paid by client.
        /// </summary>
        public required int Amount { get; set; }
        /// <summary>
        /// transaction description by client.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// foreign key pointing to shopping cart.
        /// </summary>
        public required int ShoppingCartID { get; set; }
        /// <summary>
        /// the shopping cart the client is paying for.
        /// </summary>
        public required ShoppingCartModel ShoppingCart { get; set; }

        public PaymentRequest(string Order_id, int Amount, string? Name, string? PhoneNumber, string? Email, string? Description, string CallBackAddress)
        {
            OrderId = Order_id;
            this.Amount = Amount;
            this.Description = Description;
        }
    }
    /// <summary>
    /// format of an Exception thrown by IDPay. used for binding.
    /// </summary>
    public class PaymentErrorResponse
    {
        /// <summary>
        /// Error Code
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Error Message
        /// </summary>
        public string Message { get; set; }
    }
    /// <summary>
    /// The response sent by idpay showing payment creation has been successful and we can guide our user to the link given. 
    /// </summary>
    public class PaymentCreationSuccessResponse
    {
        /// <summary>
        /// payment id generated by idpay.
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// the URL of the terminal.
        /// </summary>
        public required string Link { get; set; }

    }
    /// <summary>
    /// The response given by IDPay for validating a payment.
    /// </summary>
    public class PaymentOutcomeValidationResponse
    {
        /// <summary>
        /// Payment Status as code
        /// </summary>
        public required int Status { get; set; }
        /// <summary>
        /// the number you can use to get info about a transaction.
        /// </summary>
        public required int TrackID { get; set; }
        /// <summary>
        /// a transactions identifier.
        /// </summary>
        public required string TransactionID { get; set; } // Already in the DB record
        /// <summary>
        /// the order id generated by us
        /// </summary>
        public required string OrderID { get; set; } // Already in the DB record
        /// <summary>
        /// the amount to pay by client.
        /// </summary>
        public required int Amount { get; set; }
        /// <summary>
        /// cardNo of our client.
        /// </summary>
        public required string CardNo { get; set; }
        /// <summary>
        /// hashed card number of our client.
        /// </summary>
        public required string HashedCardNo { get; set; }
        /// <summary>
        /// time of when the transaction occurred.
        /// </summary>
        public required long TransactionTime { get; set; }

    }
}
