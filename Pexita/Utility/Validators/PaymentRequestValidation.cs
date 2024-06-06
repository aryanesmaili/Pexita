using FluentValidation;
using Pexita.Services;

namespace Pexita.Utility.Validators
{
    public class PaymentRequestValidation : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidation()
        {
            RuleFor(p => p.Amount).NotEmpty().NotNull().GreaterThan(0);
            RuleFor(p => p.ShoppingCartID).NotEmpty().NotNull();
            RuleFor(p => p.ShoppingCart).NotEmpty().NotNull();
            RuleFor(p => p.OrderId).NotEmpty().NotNull();

        }
    }
}
