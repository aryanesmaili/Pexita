using Pexita.Data.Entities.Payment;

namespace Pexita.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<List<PaymentModel>> GetPayments();
        public Task<PaymentModel> GetPayment(int id);

        public Task<bool> PaymentOutcomeValidation(PaymentOutcomeValidationResponse idpayResponse);
        public Task<string> SendPaymentRequest(PaymentRequest paymentRequest);
    }
}