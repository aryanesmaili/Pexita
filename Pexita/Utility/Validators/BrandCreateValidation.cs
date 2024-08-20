using FluentValidation;
using Pexita.Data.Entities.Brands;
using Pexita.Services.Interfaces;

namespace Pexita.Utility.Validators
{
    public class BrandCreateValidation : AbstractValidator<BrandCreateDTO>
    {
        private readonly IBrandService _brandService;
        private readonly IPexitaTools _pexitaTools;

        public BrandCreateValidation(IBrandService brandService, IPexitaTools pexitaTools)
        {
            _brandService = brandService;
            _pexitaTools = pexitaTools;

            RuleFor(b => b.Name).NotEmpty().MaximumLength(20)
                .WithMessage("Brand Name Cannot be Empty Or More than 20 Characters");

            RuleFor(bt => bt.Name).Must(x => !_brandService.IsBrand(x)).WithMessage("Brand with this name already exists.");

            RuleFor(b => b.BrandPic).Must(file => file != null && _pexitaTools.PictureFileValidation(file, 10)).When(b => b.BrandPic?.Length > 0)
                .WithMessage("Invalid or missing brand picture.");

            RuleFor(b => b.ConfirmPassword).NotEmpty();

            RuleFor(b => b.Password).NotEmpty().Equal(b => b.ConfirmPassword)
                .MinimumLength(5).MaximumLength(32);

            RuleFor(b => b.Email).NotEmpty().EmailAddress();
        }
    }

    public class BrandUpdateValidation : AbstractValidator<BrandUpdateDTO>
    {
        private readonly IBrandService _brandService;
        private readonly IPexitaTools _pexitaTools;
        public BrandUpdateValidation(IBrandService brandService, IPexitaTools pexitaTools)
        {
            _brandService = brandService;
            _pexitaTools = pexitaTools;

            RuleFor(b => b.Name).NotEmpty().MaximumLength(20)
                .WithMessage("Brand Name Cannot be Empty Or More than 20 Characters");

            RuleFor(b => b.Email).NotEmpty().EmailAddress();
            // Ignore Brandpic for validation
            RuleFor(b => b.BrandPic);
        }
    }

}
