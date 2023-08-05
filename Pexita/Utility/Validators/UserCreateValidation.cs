using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;

namespace Pexita.Utility.Validators
{
    public class UserCreateValidation : AbstractValidator<UserCreateVM>
    {
        private readonly IUserService _userService;
        public UserCreateValidation(IUserService userService)
        {
            _userService = userService;

            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();

            RuleFor(x => x.Username).NotEmpty()
                .Must(user => !_userService.IsUser(user));

            RuleFor(x => x.Email).NotEmpty().EmailAddress()
                .Must(Email => !_userService.IsEmailInUse(Email));

            RuleFor(x => x.ConfirmPassword).NotEmpty();

            RuleFor(x => x.Password).NotEmpty()
                .Equal(x => x.ConfirmPassword)
                .MinimumLength(5).MaximumLength(32); ;
        }
    }
    public class UserLoginValidation : AbstractValidator<UserLoginVM>
    {
        private readonly IUserService _userService;
        public UserLoginValidation(IUserService userService)
        {

            _userService = userService;

            RuleFor(u => u.Email).EmailAddress()
                .Must(_userService.IsEmailInUse)
                .When(u => !u.Email.IsNullOrEmpty());

            RuleFor(u => u.Password).NotEmpty()
                .MinimumLength(5).MaximumLength(32); ;

            RuleFor(u => u.UserName).Must(_userService.IsUser)
                .When(u => !u.UserName.IsNullOrEmpty());
        }
    }
    public class UserUpdateValidator : AbstractValidator<UserUpdateVM> 
    {
        private readonly IUserService _userService;
        public UserUpdateValidator(IUserService userService)
        {
            _userService= userService;

            RuleFor(u => u.ID).NotEmpty().GreaterThanOrEqualTo(0)
                .Must(_userService.IsUser);

            RuleFor(u => u.FirstName).NotEmpty();
            RuleFor(u => u.LastName).NotEmpty();
            RuleFor(u => u.Password).NotEmpty()
                .Equal(u => u.ConfirmPassword)
                .MinimumLength(5).MaximumLength(32);
        }
    }
    public class AddressValidator : AbstractValidator<Address> 
    {
        private readonly IUserService _userService;
        public AddressValidator(IUserService userService)
        {
            _userService = userService;


            RuleFor(x => x.Text).NotEmpty()
                .Must(add => !_userService.IsAddressAlready(add));

             // TODO: Validate these with Regex or External Package
            RuleFor( x => x.Province).NotEmpty();
            RuleFor(x => x.City).NotEmpty();    
            RuleFor(x => x.PostalCode).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();   

        }
    }
}
