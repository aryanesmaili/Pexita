using AutoMapper;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;

namespace Pexita.Utility.MapperConfigs
{
    public class NewsLettersMapper : Profile
    {
        public NewsLettersMapper()
        {

            CreateMap<BrandNewsletterModel, BrandNewsLetterDTO>()
                .ForMember(b => b.Brand, opt => opt.MapFrom<BNewsletterBrandResolver>())
                .ForMember(u => u.User, opt => opt.MapFrom<BNewsLetterUserResolver>());

            CreateMap<ProductNewsLetterModel, ProductNewsLetterDTO>()
                .ForMember(p => p.Product, opt => opt.MapFrom<PNewsLetterProductResolver>())
                .ForMember(u => u.User, opt => opt.MapFrom<PNewsLetterUserResolver>());
        }
    }
    public class BNewsletterBrandResolver : IValueResolver<BrandNewsletterModel, BrandNewsLetterDTO, BrandInfoDTO>
    {
        private readonly IBrandService _brandService;

        public BNewsletterBrandResolver(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public BrandInfoDTO Resolve(BrandNewsletterModel source, BrandNewsLetterDTO destination, BrandInfoDTO destMember, ResolutionContext context)
        {
            return _brandService.BrandModelToInfo(source.Brand);
        }
    }
    public class BNewsLetterUserResolver : IValueResolver<BrandNewsletterModel, BrandNewsLetterDTO, UserInfoDTO>
    {
        private readonly IUserService _userService;

        public BNewsLetterUserResolver(IUserService userService)
        {
            _userService = userService;
        }

        public UserInfoDTO Resolve(BrandNewsletterModel source, BrandNewsLetterDTO destination, UserInfoDTO destMember, ResolutionContext context)
        {
            return _userService.UserModelToInfoDTO(source.User);
        }
    }
    public class PNewsLetterProductResolver : IValueResolver<ProductNewsLetterModel, ProductNewsLetterDTO, ProductInfoDTO>
    {
        private readonly IProductService _productService;

        public PNewsLetterProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public ProductInfoDTO Resolve(ProductNewsLetterModel source, ProductNewsLetterDTO destination, ProductInfoDTO destMember, ResolutionContext context)
        {
            return _productService.ProductModelToInfoDTO(source.Product);
        }
    }
    public class PNewsLetterUserResolver : IValueResolver<ProductNewsLetterModel, ProductNewsLetterDTO, UserInfoDTO>
    {
        private readonly IUserService _userService;

        public PNewsLetterUserResolver(IUserService userService)
        {
            _userService = userService;
        }

        public UserInfoDTO Resolve(ProductNewsLetterModel source, ProductNewsLetterDTO destination, UserInfoDTO destMember, ResolutionContext context)
        {
            return _userService.UserModelToInfoDTO(source.User);
        }
    }
}
