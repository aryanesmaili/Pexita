using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Services.Interfaces;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Pexita.Additionals.Exceptions.PaymentException;

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


        public async Task<List<PaymentModel>> GetPayments()
        {
            return await _Context.Payments.ToListAsync();
        }

        public async Task<PaymentModel> GetPayment(int id)
        {
            return await _Context.Payments.SingleAsync(p => p.ID == id);
        }

        public PaymentService(string APIKey, string CallbackAddress, bool isTest, AppDBContext Context, IMapper mapper)
        {
            _apiKey = APIKey;
            _isTest = isTest;
            _CallbackAddress = CallbackAddress;
            _Context = Context;
            _mapper = mapper;
        }

        /// <summary>
        /// Sends a payment request to the IDPay API and returns the payment link if successful.
        /// </summary>
        /// <param name="paymentRequest">The payment request object containing details such as order ID, amount, and user information.</param>
        /// <returns>A task representing the asynchronous operation, containing the payment link if the request was successful.</returns>
        public async Task<string> SendPaymentRequest(PaymentRequest paymentRequest)
        {
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
        public async Task<bool> ToggleOrderToSent(int orderID)
        {
            var order = await _Context.Orders.SingleAsync(order => order.ID == orderID);
            order.Status = OrdersModel.OrderStatus.Sent;
            _Context.SaveChanges();
            return true;
        }
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
    }
    public class PaymentRequest
    {
        public required string OrderId { get; set; }
        public required int Amount { get; set; }
        public string? Description { get; set; }
        public required int ShoppingCartID { get; set; }
        public required ShoppingCartModel ShoppingCart { get; set; }

        public PaymentRequest(string Order_id, int Amount, string? Name, string? PhoneNumber, string? Email, string? Description, string CallBackAddress)
        {
            OrderId = Order_id;
            this.Amount = Amount;
            this.Description = Description;
        }
    }

    public class PaymentErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
    public class PaymentCreationSuccessResponse
    {
        public string Id { get; set; }
        public required string Link { get; set; }

    }
    public class PaymentOutcomeValidationResponse
    {
        public required int Status { get; set; }
        public required int TrackID { get; set; }
        public required string TransactionID { get; set; } // Already in the DB record
        public required string OrderID { get; set; } // Already in the DB record
        public required int Amount { get; set; }
        public required string CardNo { get; set; }
        public required string HashedCardNo { get; set; }
        public required long TransactionTime { get; set; }

    }

}
