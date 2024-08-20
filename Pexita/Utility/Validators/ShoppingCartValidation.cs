using FluentValidation;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Services.Interfaces;

namespace Pexita.Utility.Validators
{
    public class ShoppingCartValidation : AbstractValidator<ShoppingCartDTO>
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        public ShoppingCartValidation(ICartService cartService, IUserService userService, IPaymentService paymentService)
        {
            _cartService = cartService;
            _userService = userService;
            _paymentService = paymentService;

            RuleFor(s => s.TotalPrice)
                .NotEmpty().WithMessage("TotalPrice should not be empty.")
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be less than zero.");

            RuleFor(s => s.UserID)
                .NotEmpty().WithMessage("User Must be valid.")
                .Must(_userService.IsUser).WithMessage("User Does not Exist.");

            RuleFor(s => s.Payments).Must(x => _paymentService.AreValidPayments(x ?? [])).When(x => x.Payments != null);


        }
    }
}
