using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
using Pexita.Utility;

namespace Pexita.Additionals
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

            CreateMap<ProductCreateVM, ProductModel>()
                .ForMember(Product => Product.Brand, opt => opt.MapFrom(src => _brandService.GetBrandByName(src.Brand)))
                .ForMember(Product => Product.ProductPicsURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}").Result))
                .ForMember(Product => Product.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Product => Product.IsAvailable, opt => opt.MapFrom(src => true))
                .ForMember(Product => Product.Tags, opt => opt.MapFrom(src => _pexitaTools.StringToTags(src.Tags)))
                .ForMember(Product => Product.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(Product => Product.Rating, opt => opt.MapFrom(src => new List<ProductRating>()));

            CreateMap<ProductUpdateVM, ProductModel>()
                .ForMember(product => product.ProductPicsURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}")));


            CreateMap<ProductModel, ProductInfoVM>()
                .ForMember(PInfo => PInfo.Brand, opt => opt.MapFrom(src => _brandService.BrandModelToInfo(src.Brand)))
                .ForMember(PInfo => PInfo.Rate, opt => opt.MapFrom(src => _pexitaTools.GetRating(src.Rating.Select(x => x.Rating).ToList())))
                .ForMember(PInfo => PInfo.Tags, opt => opt.MapFrom(src => _tagsService.TagsToVM(src.Tags!)));

            CreateMap<BrandCreateVM, BrandModel>()
                .ForMember(Brand => Brand.BrandPicURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.BrandPic, $"{src.Name}/{src.Name}")))
                .ForMember(Brand => Brand.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Brand => Brand.Products, opt => opt.MapFrom(src => new List<ProductModel>()))
                .ForMember(Brand => Brand.Orders, opt => opt.MapFrom(src => new List<OrdersModel>()))
                .ForMember(Brand => Brand.BrandNewsLetters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(Brand => Brand.ProductNewsLetters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()));

            CreateMap<BrandUpdateVM, BrandModel>()
                .ForMember(brand => brand.BrandPicURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.BrandPic!, $"{src.Name}/{src.Name}")));

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
}
