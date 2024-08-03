using AutoMapper;
using Elfie.Serialization;
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

            /*            CreateMap<ProductPatchDTO, ProductModel>()
                            .ForMember(product => product.Title, opt => opt.MapFrom((src, dest) => string.IsNullOrEmpty(src.Title) ? dest.Title : src.Title))
                            .ForMember(product => product.Description, opt => opt.MapFrom((src, dest) => src.Description ?? dest.Description))
                            .ForMember(product => product.Price, opt => opt.MapFrom((src, dest) => src.Price ?? dest.Price))
                            .ForMember(product => product.Quantity, opt => opt.MapFrom((src, dest) => src.Quantity ?? dest.Quantity))
                            .ForMember(product => product.Brand, opt => opt.Ignore())
                            .ForMember(product => product.IsAvailable, opt => opt.MapFrom((src, dest) => src.IsAvailable))
                            .ForMember(product => product.Colors, opt => opt.MapFrom((src, dest) => src.Colors ?? dest.Colors))
                            .ForMember(product => product.Tags, opt => opt.MapFrom(src => _pexitaTools.StringToTags(src.Tags)))
                            .ForMember(product => product.ProductPicsURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}", true)));*/

            CreateMap<ProductModel, ProductInfoVM>();

            CreateMap<BrandCreateVM, BrandModel>()
                .ForMember(Brand => Brand.BrandPicURL, opt => opt.MapFrom<BrandPicURLResolver>())
                .ForMember(Brand => Brand.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Brand => Brand.Products, opt => opt.MapFrom(src => new List<ProductModel>()))
                .ForMember(Brand => Brand.BrandOrders, opt => opt.MapFrom(src => new List<BrandOrder>()))
                .ForMember(Brand => Brand.BrandNewsLetters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(Brand => Brand.ProductNewsLetters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()));

            CreateMap<BrandUpdateVM, BrandModel>();

            CreateMap<BrandModel, BrandInfoVM>();

            CreateMap<UserUpdateVM, UserModel>();

            CreateMap<UserModel, UserInfoVM>();

            CreateMap<UserCreateVM, UserModel>()
                .ForMember(u => u.Addresses, opt => opt.MapFrom(src => new List<Address>()))
                .ForMember(u => u.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(u => u.Orders, opt => opt.MapFrom(src => new List<OrdersModel>()))
                .ForMember(u => u.ShoppingCarts, opt => opt.MapFrom(src => new List<ShoppingCartModel>()))
                .ForMember(u => u.BrandNewsletters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(u => u.ProductNewsletters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()))
                .ForMember(u => u.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()));

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
                .ForMember(p => p.Successfull, opt => opt.MapFrom(src => transactionStatus[src.Status]));
        }
    }
    public class BrandPicURLResolver : IValueResolver<BrandCreateVM, BrandModel, string?>
    {
        private readonly IPexitaTools _pexitaTools;

        public BrandPicURLResolver(IPexitaTools pexitaTools)
        {
            _pexitaTools = pexitaTools;
        }
        public string? Resolve(BrandCreateVM source, BrandModel destination, string? destinationMember, ResolutionContext context)
        {
            // Only set the URL if BrandPic is not null or empty
            if (source.Brandpic != null && source.Brandpic.Length > 0)
            {
                return _pexitaTools.SaveEntityImages(source.Brandpic, $"{source.Name}/{source.Name}", false).Result;
            }
            return null; // return null if the pic is empty.
        }
    }
}
