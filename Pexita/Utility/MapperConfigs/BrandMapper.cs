using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Products;
using Pexita.Services.Interfaces;
using System.Drawing.Drawing2D;

namespace Pexita.Utility.MapperConfigs
{
    public class BrandMapper : Profile
    {

        public BrandMapper()
        {
            CreateMap<BrandCreateDTO, BrandModel>()
                    .ForMember(Brand => Brand.BrandPicURL, opt => opt.MapFrom<BrandPicURLResolver>())
                    .ForMember(Brand => Brand.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(Brand => Brand.Products, opt => opt.MapFrom(src => new List<ProductModel>()))
                    .ForMember(Brand => Brand.BrandOrders, opt => opt.MapFrom(src => new List<BrandOrder>()))
                    .ForMember(Brand => Brand.BrandNewsLetters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                    .ForMember(Brand => Brand.ProductNewsLetters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()));

            CreateMap<BrandUpdateDTO, BrandModel>();

            CreateMap<BrandModel, BrandInfoDTO>()
                .ForMember(b => b.ProductsIDs, opt => opt.MapFrom<BrandProductResolver>());
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
            if (source.BrandPic != null && source.BrandPic.Length > 0)
            {
                return _pexitaTools.SaveEntityImages(source.BrandPic, $"Brands/{source.Name}", false).Result;
            }
            return null; // return null if the pic is empty.
        }
    }
    public class BrandProductResolver : IValueResolver<BrandModel, BrandInfoDTO, List<int>?>
    {
        private readonly AppDBContext _context;

        public BrandProductResolver(AppDBContext context)
        {
            _context = context;
        }

        public List<int>? Resolve(BrandModel source, BrandInfoDTO destination, List<int>? destMember, ResolutionContext context)
        {
            List<int>? result = _context.Products
                .AsNoTracking()
                .Where(x => x.BrandID == source.ID)
                .Select(x => x.ID)
                .ToList();
            return result;
        }
    }
}
