namespace Pexita.Services
{
    public class PaymentSystemService
    {
        private readonly string _apiKey;
        private readonly bool _isTest;
        private const string _requestAPIAddress = "https://api.idpay.ir/v1.1/payment";

        public PaymentSystemService(string APIKey, bool isTest)
        {
            _apiKey = APIKey;
            _isTest = isTest;
        }
    }
    public class PaymentRequest
    {
        public required int Order_id { get; set; }
        public required int Amount { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public required string CallBackAddress { get; set; }
        public PaymentRequest(int Order_id, int Amount, string? Name, string? PhoneNumber, string? Email, string? Description, string CallBackAddress)
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
}
