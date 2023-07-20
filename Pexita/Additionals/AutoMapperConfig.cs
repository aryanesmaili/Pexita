using AutoMapper;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Products;
using Pexita.Services.Interfaces;
using Pexita.Utility;

namespace Pexita.Additionals
{
    public class AutoMapperConfig : Profile
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly PexitaTools _pexitaTools;
        private readonly ITagsService _tagsService;

        public AutoMapperConfig(IBrandService brandService, IProductService productService, PexitaTools pexitaTools, ITagsService TagsService)
        {
            _brandService = brandService;
            _productService = productService;
            _pexitaTools = pexitaTools;
            _tagsService = TagsService;

            CreateMap<ProductCreateVM, ProductModel>()
                .ForMember(Product => Product.Brand, opt => opt.MapFrom(src => _brandService.GetBrandByName(src.Brand)))
                .ForMember(Product => Product.ProductPicsURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}").Result))
                .ForMember(Product => Product.DateAdded, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Product => Product.IsAvailable, opt => opt.MapFrom(src => true))
                .ForMember(Product => Product.Tags, opt => opt.MapFrom(src => _pexitaTools.StringToTags(src.Tags)))
                .ForMember(Product => Product.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(Product => Product.Rate, opt => opt.MapFrom(src => new List<double>()));

            CreateMap<BrandCreateVM, BrandModel>()
                .ForMember(Brand => Brand.BrandPicURL, opt => opt.MapFrom(src => _pexitaTools.SaveProductImages(src.BrandPics, $"{src.Name}/{src.Name}")))
                .ForMember(Brand => Brand.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Brand => Brand.Products, opt => opt.MapFrom(src => new List<ProductModel>()))
                .ForMember(Brand => Brand.Orders, opt => opt.MapFrom(src => new List<OrdersModel>()))
                .ForMember(Brand => Brand.BrandNewsLetters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(Brand => Brand.ProductNewsLetters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()));

            CreateMap<ProductModel, ProductInfoVM>()
                .ForMember(PInfo => PInfo.Brand, opt => opt.MapFrom(src => _brandService.BrandModelToInfo(src.Brand)))
                .ForMember(PInfo => PInfo.Rate, opt => opt.MapFrom(src => _pexitaTools.GetRating(src.Rate!)))
                .ForMember(PInfo => PInfo.Tags, opt => opt.MapFrom(src => _tagsService.TagsToVM(src.Tags!)));
        }
    }
}
