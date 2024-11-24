using AutoMapper;
using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;

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
