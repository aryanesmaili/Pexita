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
        private readonly IProductService _productService;
        public ShoppingCartValidation(ICartService cartService, IUserService userService, IPaymentService paymentService, IProductService productService)
        {
            _cartService = cartService;
            _userService = userService;
            _paymentService = paymentService;
            _productService = productService;

            RuleFor(s => s.TotalPrice)
                .NotEmpty().WithMessage("TotalPrice should not be empty.")
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be less than zero.");

            RuleFor(s => s.UserID)
                .NotEmpty().WithMessage("User Must be valid.")
                .Must(_userService.IsUser).WithMessage("User Does not Exist.");

            RuleFor(s => s.Payments).Must(x => _paymentService.AreValidPayments(x ?? [])).When(x => x.Payments != null);

            RuleForEach(s => s.Items).SetValidator(new CartItemsValidator(_productService));

            RuleFor(s => s.Order).Empty();
        }
    }
    public class CartItemsValidator : AbstractValidator<CartItemsDTO>
    {
        private readonly IProductService _productService;
        public CartItemsValidator(IProductService productService)
        {
            _productService = productService;

            RuleFor(x => x.ProductID)
                .NotEmpty()
                .Must(_productService.IsProduct).WithMessage("product with this ID is not valid.");

            RuleFor(x => x.Product)
                .NotEmpty()
                .Must(_productService.IsProduct).WithMessage("product with this properties is not valid.");

            RuleFor(x => x.Count)
                .NotEmpty()
                .GreaterThanOrEqualTo(1);
        }
    }
}
