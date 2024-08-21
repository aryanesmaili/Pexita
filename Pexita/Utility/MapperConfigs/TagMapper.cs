using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;
using Pexita.Services;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Utility.MapperConfigs
{
    public class TagMapper : Profile
    {
        public TagMapper()
        {

            CreateMap<TagModel, TagInfoDTO>()
                .ForMember(t => t.Products, opt => opt.MapFrom<TagProductResolver>());

            CreateMap<TagModel, ProductTagInfoDTO>();
        }
    }

    public class TagProductResolver : IValueResolver<TagModel, TagInfoDTO, List<int>?>
    {
        private readonly IProductService _productService;

        public TagProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public List<int>? Resolve(TagModel source, TagInfoDTO destination, List<int>? destMember, ResolutionContext context)
        {
            var s = source.Products?.Select(x => x.ID).ToList();
            return s;
        }
    }
}
