using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public PaymentController()
        {
            
        }

        [HttpGet("Payments")]
        public IActionResult GetPayments()
        {
            return Ok();
        }

        [HttpGet("Payments/get/{id:int}")]
        public IActionResult GetPayment(int id)
        {
            return Ok();
        }

        [HttpPost("Payments/new")]
        public IActionResult CreateNewPaymentRequest()
        {
            return Ok();
        }
        [HttpPost("Payments/callback")]
        public IActionResult PaymentOutcome()
        {
            // find the corresponding payment record

            // update it's values
            // return the response
            return Ok();
        }
    }
}
