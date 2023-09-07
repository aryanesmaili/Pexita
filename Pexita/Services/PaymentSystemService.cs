using Pexita.Data;
using Pexita.Data.Entities.ShoppingCart;
using System.Text;
using System.Text.Json;

namespace Pexita.Services
{
    public class PaymentSystemService
    {
        private readonly string _apiKey;
        private readonly bool _isTest;
        private const string _requestAPIAddress = "https://api.idpay.ir/v1.1/payment";
        private readonly string _CallbackAddress;
        private readonly AppDBContext _Context;
        public PaymentSystemService(string APIKey, string CallbackAddress, bool isTest, AppDBContext Context)
        {
            _apiKey = APIKey;
            _isTest = isTest;
            _CallbackAddress = CallbackAddress;
            _Context = Context;
        }

        public async Task<PaymentErrorResponse> SendPaymentRequest(PaymentRequest paymentRequest)
        {
            using (HttpClient Client = new HttpClient())
            {
                // Add APIKey to the header of the request
                Client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

                // If we're testing, we want to go into Sandbox mode
                if (_isTest)
                {
                    Client.DefaultRequestHeaders.Add("X-SANDBOX", "1");
                }

                // Serialize the PaymentRequest object to JSON to send it
                string body = JsonSerializer.Serialize(paymentRequest);

                // Creating the whole request 
                HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

                try
                {
                    // Sending the request to IDPay
                    HttpResponseMessage response = await Client.PostAsync(_requestAPIAddress, content);
                    
                    // Check Request Status
                    if (response.IsSuccessStatusCode)
                    {

                    }

                }

                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static string GenerateOrderID(int BrandID, int ProductID, int UserID, DateTime Datetime)
        {
            return $"{BrandID}{ProductID}{UserID}{Datetime}";
        }
    }
    public class PaymentRequest
    {
        public required string Order_id { get; set; }
        public required int Amount { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public required string CallBackAddress { get; set; }

        public PaymentRequest(string Order_id, int Amount, string? Name, string? PhoneNumber, string? Email, string? Description, string CallBackAddress)
        {
            this.Order_id = Order_id;
            this.Amount = Amount;
            this.Name = Name;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
            this.Description = Description;
            this.CallBackAddress = CallBackAddress;
        }
    }

    public class PaymentErrorResponse
    {
        public string Status { get; set; }
        public int Code { get; set; }
        public string? Message { get; set; }
    }

}
