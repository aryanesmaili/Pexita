using AutoMapper;
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
using BCrypt.Net;

namespace Pexita.Utility
{
    public class AutoMapperConfig : Profile
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IPexitaTools _pexitaTools;
        private readonly ITagsService _tagsService;

        public AutoMapperConfig(IBrandService brandService, IProductService productService, IPexitaTools pexitaTools, ITagsService TagsService)
        {
            _brandService = brandService;
            _productService = productService;
            _pexitaTools = pexitaTools;
            _tagsService = TagsService;

            CreateMap<ProductCreateDTO, ProductModel>()
                .ForMember(Product => Product.Brand, opt => opt.MapFrom(src => _brandService.GetBrandByName(src.Brand)))
                .ForMember(Product => Product.ProductPicsURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}", false).Result))
                .ForMember(Product => Product.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Product => Product.IsAvailable, opt => opt.MapFrom(src => true))
                .ForMember(Product => Product.Tags, opt => opt.MapFrom(src => _pexitaTools.StringToTags(src.Tags)))
                .ForMember(Product => Product.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(Product => Product.Rating, opt => opt.MapFrom(src => new List<ProductRating>()));

            CreateMap<ProductUpdateDTO, ProductModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors))
                .ForMember(product => product.ProductPicsURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}", true)))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => _pexitaTools.StringToTags(src.Tags)))
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.BrandID, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.NewsLetters, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore());


            CreateMap<ProductPatchDTO, ProductModel>()
                .ForMember(product => product.Title, opt => opt.MapFrom((src, dest) => string.IsNullOrEmpty(src.Title) ? dest.Title : src.Title))
                .ForMember(product => product.Description, opt => opt.MapFrom((src, dest) => src.Description ?? dest.Description))
                .ForMember(product => product.Price, opt => opt.MapFrom((src, dest) => src.Price ?? dest.Price))
                .ForMember(product => product.Quantity, opt => opt.MapFrom((src, dest) => src.Quantity ?? dest.Quantity))
                .ForMember(product => product.Brand, opt => opt.Ignore())
                .ForMember(product => product.IsAvailable, opt => opt.MapFrom((src, dest) => src.IsAvailable))
                .ForMember(product => product.Colors, opt => opt.MapFrom((src, dest) => src.Colors ?? dest.Colors))
                .ForMember(product => product.Tags, opt => opt.MapFrom(src => _pexitaTools.StringToTags(src.Tags)))
                .ForMember(product => product.ProductPicsURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}", true)));

            CreateMap<ProductModel, ProductInfoVM>()
                .ForMember(PInfo => PInfo.Brand, opt => opt.MapFrom(src => _brandService.BrandModelToInfo(src.Brand)))
                .ForMember(PInfo => PInfo.Rate, opt => opt.MapFrom(src => _pexitaTools.GetRating(src.Rating.Select(x => x.Rating).ToList())))
                .ForMember(PInfo => PInfo.Tags, opt => opt.MapFrom(src => _tagsService.TagsToVM(src.Tags!)));

            CreateMap<BrandCreateVM, BrandModel>()
                .ForMember(Brand => Brand.BrandPicURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.BrandPic, $"{src.Name}/{src.Name}", false)))
                .ForMember(Brand => Brand.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Brand => Brand.Products, opt => opt.MapFrom(src => new List<ProductModel>()))
                .ForMember(Brand => Brand.BrandOrders, opt => opt.MapFrom(src => new List<BrandOrder>()))
                .ForMember(Brand => Brand.BrandNewsLetters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(Brand => Brand.ProductNewsLetters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()));

            CreateMap<BrandUpdateVM, BrandModel>()
                .ForMember(brand => brand.BrandPicURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.BrandPic!, $"{src.Name}/{src.Name}", true)));

            CreateMap<BrandModel, BrandInfoVM>()
                .ForMember(BInfo => BInfo.Products, opt => opt.MapFrom(src => src.Products!.Select(_productService.ProductModelToInfoVM) ?? null));

            CreateMap<UserUpdateVM, UserModel>();

            CreateMap<UserModel, UserInfoVM>();

            CreateMap<UserCreateVM, UserModel>()
                .ForMember(u => u.Addresses, opt => opt.MapFrom(src => new List<Address>()))
                .ForMember(u => u.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(u => u.Orders, opt => opt.MapFrom(src => new List<OrdersModel>()))
                .ForMember(u => u.ShoppingCarts, opt => opt.MapFrom(src => new List<ShoppingCartModel>()))
                .ForMember(u => u.BrandNewsletters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(u => u.ProductNewsletters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()))
                .ForMember(u => u.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(u => u.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

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
}
