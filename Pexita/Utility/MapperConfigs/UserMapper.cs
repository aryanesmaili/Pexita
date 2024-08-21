using AutoMapper;
using Pexita.Data;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Utility.MapperConfigs
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserUpdateDTO, UserModel>();

            CreateMap<UserModel, UserInfoDTO>()
                .ForMember(u => u.Addresses, opt => opt.MapFrom<UserAddressResolver>())
                .ForMember(u => u.Orders, opt => opt.MapFrom<UserOrdersResolver>())
                .ForMember(u => u.BrandNewsletters, opt => opt.MapFrom<UserBrandNewsLettersResolver>())
                .ForMember(u => u.ProductNewsletters, opt => opt.MapFrom<UserProductNewsLettersResolver>());

            CreateMap<UserCreateDTO, UserModel>()
                .ForMember(u => u.Addresses, opt => opt.MapFrom(src => new List<Address>()))
                .ForMember(u => u.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(u => u.Orders, opt => opt.MapFrom(src => new List<OrdersModel>()))
                .ForMember(u => u.ShoppingCarts, opt => opt.MapFrom(src => new List<ShoppingCartModel>()))
                .ForMember(u => u.BrandNewsletters, opt => opt.MapFrom(src => new List<BrandNewsletterModel>()))
                .ForMember(u => u.ProductNewsletters, opt => opt.MapFrom(src => new List<ProductNewsLetterModel>()))
                .ForMember(u => u.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(u => u.ProfilePicURL, opt => opt.MapFrom<UserPicURLResolver>());

            CreateMap<Address, AddressDTO>();
            CreateMap<AddressDTO, Address>()
                .ForMember(x => x.User, opt => opt.MapFrom<AddressUserResolver>());
        }
    }
    public class UserAddressResolver : IValueResolver<UserModel, UserInfoDTO, List<AddressDTO>?>
    {
        private readonly IMapper _mapper;

        public UserAddressResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<AddressDTO>? Resolve(UserModel source, UserInfoDTO destination, List<AddressDTO>? destMember, ResolutionContext context)
        {
            return source.Addresses?.Select(x => _mapper.Map<AddressDTO>(x)).ToList();
        }
    }
    public class UserOrdersResolver : IValueResolver<UserModel, UserInfoDTO, List<OrdersDTO>?>
    {
        private readonly IMapper _mapper;

        public UserOrdersResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<OrdersDTO>? Resolve(UserModel source, UserInfoDTO destination, List<OrdersDTO>? destMember, ResolutionContext context)
        {
            return source.Orders?.Select(x => _mapper.Map<OrdersDTO>(x)).ToList();
        }
    }
    public class UserBrandNewsLettersResolver : IValueResolver<UserModel, UserInfoDTO, List<BrandNewsLetterDTO>?>
    {
        private readonly IMapper _mapper;

        public UserBrandNewsLettersResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<BrandNewsLetterDTO>? Resolve(UserModel source, UserInfoDTO destination, List<BrandNewsLetterDTO>? destMember, ResolutionContext context)
        {
            return source.BrandNewsletters?.Select(x => _mapper.Map<BrandNewsLetterDTO>(x)).ToList();
        }
    }
    public class UserProductNewsLettersResolver : IValueResolver<UserModel, UserInfoDTO, List<ProductNewsLetterDTO>?>
    {
        private readonly IMapper _mapper;

        public UserProductNewsLettersResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<ProductNewsLetterDTO>? Resolve(UserModel source, UserInfoDTO destination, List<ProductNewsLetterDTO>? destMember, ResolutionContext context)
        {
            return source.ProductNewsletters?.Select(x => _mapper.Map<ProductNewsLetterDTO>(x)).ToList();
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
    public class AddressUserResolver : IValueResolver<AddressDTO, Address, UserModel>
    {
        private readonly AppDBContext _context;

        public AddressUserResolver(AppDBContext context)
        {
            _context = context;
        }

        public UserModel Resolve(AddressDTO source, Address destination, UserModel destMember, ResolutionContext context)
        {
            return _context.Users.Find(source.UserID) ?? throw new NotFoundException($"user {source.UserID} not found.");
        }
    }
}
