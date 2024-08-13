using AutoMapper;
using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.User;
using Pexita.Services;
using Pexita.Services.Interfaces;

namespace Pexita.Utility
{
    public class AutoMapperConfig : Profile
    {

        public AutoMapperConfig()
        {

            CreateMap<ProductCreateDTO, ProductModel>()
                .ForMember(Product => Product.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Product => Product.IsAvailable, opt => opt.MapFrom(src => true))
                .ForMember(Product => Product.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(Product => Product.Rating, opt => opt.MapFrom(src => new List<ProductRating>()));

            CreateMap<ProductUpdateDTO, ProductModel>();

            CreateMap<ProductModel, ProductInfoDTO>();

            CreateMap<BrandCreateDTO, BrandModel>()
                .ForMember(Brand => Brand.BrandPicURL, opt => opt.MapFrom<BrandPicURLResolver>())
                .ForMember(Brand => Brand.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Brand => Brand.Products, opt => opt.MapFrom(src => new List<ProductModel>()))
                .ForMember(Brand => Brand.BrandOrders, opt => opt.MapFrom(src => new List<BrandOrder>()))
                .ForMember(Brand => Brand.BrandNewsLetters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(Brand => Brand.ProductNewsLetters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()));

            CreateMap<BrandUpdateDTO, BrandModel>();

            CreateMap<BrandModel, BrandInfoDTO>()
                .ForMember(b => b.Products, opt => opt.MapFrom<BrandProductResolver>());


            CreateMap<UserUpdateDTO, UserModel>();

            CreateMap<UserModel, UserInfoDTO>();

            CreateMap<UserCreateDTO, UserModel>()
                .ForMember(u => u.Addresses, opt => opt.MapFrom(src => new List<Address>()))
                .ForMember(u => u.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(u => u.Orders, opt => opt.MapFrom(src => new List<OrdersModel>()))
                .ForMember(u => u.ShoppingCarts, opt => opt.MapFrom(src => new List<ShoppingCartModel>()))
                .ForMember(u => u.BrandNewsletters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(u => u.ProductNewsletters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()))
                .ForMember(u => u.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()));

            CreateMap<BrandRefreshToken, BrandRefreshTokenDTO>();
            CreateMap<UserRefreshToken, UserRefreshTokenDTO>();

            Dictionary<int, bool> transactionStatus = new()
            {
                {1, false},
                {2, false},
                {3, false},
                {4, false},
                {5, false},
                {6, false},
                {7, false},
                {8, false},
                {10,false},
                {100, true},
                {101, true },
                {200, true }
            };

            CreateMap<PaymentOutcomeValidationResponse, PaymentModel>()
                .ForMember(p => p.IDPayTrackID, opt => opt.MapFrom(src => src.TrackID))
                .ForMember(p => p.CardNo, opt => opt.MapFrom(src => src.CardNo))
                .ForMember(p => p.HashedCardNo, opt => opt.MapFrom(src => src.HashedCardNo))
                .ForMember(p => p.DateTimePaid, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.TransactionTime).DateTime))
                .ForMember(p => p.Successful, opt => opt.MapFrom(src => transactionStatus[src.Status]));
        }
    }
    public class BrandPicURLResolver : IValueResolver<BrandCreateDTO, BrandModel, string?>
    {
        private readonly IPexitaTools _pexitaTools;

        public BrandPicURLResolver(IPexitaTools pexitaTools)
        {
            _pexitaTools = pexitaTools;
        }
        public string? Resolve(BrandCreateDTO source, BrandModel destination, string? destinationMember, ResolutionContext context)
        {
            // Only set the URL if BrandPic is not null or empty
            if (source.Brandpic != null && source.Brandpic.Length > 0)
            {
                return _pexitaTools.SaveEntityImages(source.Brandpic, $"Brands/{source.Name}", false).Result;
            }
            return null; // return null if the pic is empty.
        }
    }
    public class UserPicURLResolver : IValueResolver<UserCreateDTO, UserModel, string?>
    {
        private readonly IPexitaTools _pexitaTools;

        public UserPicURLResolver(IPexitaTools pexitaTools)
        {
            _pexitaTools = pexitaTools;
        }

        public string? Resolve(UserCreateDTO source, UserModel destination, string? destMember, ResolutionContext context)
        {
            if (source.ProfilePic != null && source.ProfilePic.Length > 0)
                return _pexitaTools.SaveEntityImages(source.ProfilePic, $"Users/{source.Username}", false).Result;

            return null;
        }
    }
    public class BrandProductResolver : IValueResolver<BrandModel, BrandInfoDTO, List<ProductInfoDTO>?>
    {
        private readonly IProductService _productService;

        public BrandProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public List<ProductInfoDTO>? Resolve(BrandModel source, BrandInfoDTO destination, List<ProductInfoDTO>? destMember, ResolutionContext context)
        {
            var result = source.Products?.Select(_productService.ProductModelToInfoDTO).ToList();
            return result;
        }
    }
}
