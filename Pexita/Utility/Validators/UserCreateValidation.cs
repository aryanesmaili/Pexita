using FluentValidation;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;

namespace Pexita.Utility.Validators
{
    public class UserCreateValidation : AbstractValidator<UserCreateDTO>
    {
        private readonly IUserService _userService;
        public UserCreateValidation(IUserService userService)
        {
            _userService = userService;

            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();

            RuleFor(x => x.Username).NotEmpty()
                .MinimumLength(5)
                .MaximumLength(32)
                .Must(user => !_userService.IsUser(user)).WithMessage("Username already taken.");

            RuleFor(x => x.Email).NotEmpty().EmailAddress()
                .Must(Email => !_userService.IsEmailInUse(Email)).WithMessage("Email already in use.");

            RuleFor(x => x.ConfirmPassword).NotEmpty();

            RuleFor(x => x.Password).NotEmpty()
                .Equal(x => x.ConfirmPassword)
                .MinimumLength(5).MaximumLength(32);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"(0|\+98)?([ ]|-|[()]){0,2}9[1|2|3|4]([ ]|-|[()]){0,2}(?:[0-9]([ ]|-|[()]){0,2}){8}"); // regex for IR Phone Numbers
        }
    }
    public class UserLoginValidation : AbstractValidator<LoginDTO>
    {
        public UserLoginValidation()
        {
            RuleFor(DTO => DTO.Identifier)
                .NotEmpty();

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(5).MaximumLength(32); ;
        }
    }
    public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
    {
        private readonly IUserService _userService;
        public UserUpdateValidator(IUserService userService)
        {
            _userService = userService;
            RuleFor(u => u.ID).NotEmpty().GreaterThanOrEqualTo(0)
                .Must(_userService.IsUser);

            RuleFor(u => u.FirstName).NotEmpty();
            RuleFor(u => u.LastName).NotEmpty();
        }
    }
    public class AddressValidator : AbstractValidator<AddressDTO>
    {
        private readonly IIranAPI _iranAPI;

        public AddressValidator(IIranAPI IranAPI)
        {
            _iranAPI = IranAPI;

            RuleFor(x => x.Text).NotEmpty();

            RuleFor(x => x.Province).NotEmpty().Must(x => _iranAPI.IsStateValid(x, isEng: false).Result);
            RuleFor(x => x.City).NotEmpty().Must((state, city) => _iranAPI.IsCityValid(state.Province, state.City, isEng: false).Result);
            RuleFor(x => x.PostalCode).NotEmpty().Matches(@"\b(?!(\d)\1{3})[13-9]{4}[1346-9][013-9]{5}\b"); // regex for IR Postal Codes
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^(\+98|0)?9\d{9}$"); // regex for IR Phone Numbers

        }
    }
}
