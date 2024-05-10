using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pexita.Services;
using Pexita.Services.Interfaces;
using static Pexita.Additionals.Exceptions.PaymentException;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("Payments")]
        public IActionResult GetPayments()
        {
            return Ok(_paymentService.GetPayments());
        }

        [HttpGet("Payments/get/{id:int}")]
        public IActionResult GetPayment(int id)
        {
            return Ok(_paymentService.GetPayment(id));
        }

        [HttpPost("Payments/new")]
        public async Task<IActionResult> CreateNewPaymentRequest([FromBody] PaymentRequest request)
        {
            try
            {
                string paymentLink = await _paymentService.SendPaymentRequest(request);
                return Redirect(paymentLink);
            }

            catch (Exception e)
            {
                return BadRequest($"{e.InnerException}: {e.Message}");
            }

        }

        [HttpPost("Payments/callback")]
        public IActionResult PaymentOutcome([FromBody] PaymentOutcomeValidationResponse response)
        {
            try
            {
                var result = _paymentService.PaymentOutcomeValidation(response);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"{e.InnerException}: {e.Message}");
            }

        }
    }
}
