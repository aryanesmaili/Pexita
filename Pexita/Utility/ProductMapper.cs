using AutoMapper;
using Pexita.Data;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;

namespace Pexita.Utility
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<ProductModel, ProductInfoDTO>()
                .ForMember(p => p.Tags, opt => opt.MapFrom<ProductTagsResolver>())
                .ForMember(p => p.Rate, opt => opt.MapFrom<ProductRateResolver>())
                .ForMember(p => p.Comments, opt => opt.MapFrom<ProductCommentResolver>());

            CreateMap<ProductCreateDTO, ProductModel>()
                .ForMember(Product => Product.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Product => Product.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(Product => Product.Rating, opt => opt.MapFrom(src => new List<ProductRating>()))
                .ForMember(Product => Product.ProductPicsURL, opt => opt.MapFrom<ProductCreatePicResolver>())
                .ForMember(Product => Product.Tags, opt => opt.MapFrom<ProductCreationTagsResolver>());

            CreateMap<ProductUpdateDTO, ProductModel>()
                .ForMember(p => p.ProductPicsURL, opt => opt.MapFrom<ProductUpdatePicResolver>())
                .ForMember(p => p.Tags, opt => opt.MapFrom<ProductUpdateTagsResolver>());
        }
    }
    public class ProductCommentResolver : IValueResolver<ProductModel, ProductInfoDTO, List<CommentsDTO>?>
    {
        private readonly IMapper _mapper;

        public ProductCommentResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<CommentsDTO>? Resolve(ProductModel source, ProductInfoDTO destination, List<CommentsDTO>? destMember, ResolutionContext context)
        {
            return source.Comments?.Select(x => _mapper.Map<CommentsDTO>(x)).ToList();
        }
    }
    public class ProductUpdatePicResolver : IValueResolver<ProductUpdateDTO, ProductModel, string?>
    {
        private readonly IPexitaTools _pexitaTools;
        private readonly IBrandService _brandService;
        public ProductUpdatePicResolver(IPexitaTools pexitaTools, IBrandService brandService)
        {
            _pexitaTools = pexitaTools;
            _brandService = brandService;
        }
        public string? Resolve(ProductUpdateDTO source, ProductModel destination, string? destMember, ResolutionContext context)
        {
            if (source.ProductPics?.Count > 0)
                return _pexitaTools.SaveEntityImages(source.ProductPics ?? [], $"{_brandService.GetBrandByID(source.BrandID).Result.Username}/{source.Title}", false).Result;
            return null;
        }
    }
    public class ProductCreatePicResolver : IValueResolver<ProductCreateDTO, ProductModel, string?>
    {
        private readonly IPexitaTools _pexitaTools;
        private readonly IBrandService _brandService;
        public ProductCreatePicResolver(IPexitaTools pexitaTools, IBrandService brandService)
        {
            _pexitaTools = pexitaTools;
            _brandService = brandService;
        }

        public string? Resolve(ProductCreateDTO source, ProductModel destination, string? destMember, ResolutionContext context)
        {
            if (source.ProductPics?.Count > 0)
                return _pexitaTools.SaveEntityImages(source.ProductPics ?? [], $"{_brandService.GetBrandByID(source.BrandID).Result.Username}/{source.Title}", false).Result;
            return null;
        }
    }
    public class ProductUpdateTagsResolver : IValueResolver<ProductUpdateDTO, ProductModel, List<TagModel>?>
    {
        private readonly IPexitaTools _pexitaTools;

        public ProductUpdateTagsResolver(IPexitaTools pexitaTools)
        {
            _pexitaTools = pexitaTools;
        }
        public List<TagModel>? Resolve(ProductUpdateDTO source, ProductModel destination, List<TagModel>? destMember, ResolutionContext context)
        {
            var result = _pexitaTools.StringToTags(source.Tags ?? "", destination).Result;
            return result;
        }
    }
    public class ProductCreationTagsResolver : IValueResolver<ProductCreateDTO, ProductModel, List<TagModel>?>
    {
        private readonly IPexitaTools _pexitaTools;

        public ProductCreationTagsResolver(IPexitaTools pexitaTools)
        {
            _pexitaTools = pexitaTools;
        }

        public List<TagModel>? Resolve(ProductCreateDTO source, ProductModel destination, List<TagModel>? destMember, ResolutionContext context)
        {
            return _pexitaTools.StringToTags(source.Tags ?? "", destination).Result;
        }
    }
    public class ProductTagsResolver : IValueResolver<ProductModel, ProductInfoDTO, List<ProductTagInfoDTO>?>
    {
        private readonly IMapper _mapper;

        public ProductTagsResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<ProductTagInfoDTO>? Resolve(ProductModel source, ProductInfoDTO destination, List<ProductTagInfoDTO>? destMember, ResolutionContext context)
        {
            return source.Tags?.Select(x => _mapper.Map<ProductTagInfoDTO>(x)).ToList() ?? [];
        }
    }
    public class ProductRateResolver : IValueResolver<ProductModel, ProductInfoDTO, double?>
    {
        private readonly IPexitaTools _pexitaTools;
        private readonly AppDBContext _context;

        public ProductRateResolver(IPexitaTools pexitaTools, AppDBContext context)
        {
            _pexitaTools = pexitaTools;
            _context = context;
        }

        public double? Resolve(ProductModel source, ProductInfoDTO destination, double? destMember, ResolutionContext context)
        {
            var ratings = _context.Ratings.Where(x => x.ProductID == source.ID).Select(x => x.Rating).ToList();
            return _pexitaTools.GetRating(ratings) ?? null;
        }
    }
}