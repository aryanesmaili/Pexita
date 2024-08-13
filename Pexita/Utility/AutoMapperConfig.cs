﻿using AutoMapper;
using Microsoft.Identity.Client;
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

namespace Pexita.Utility
{
    public class AutoMapperConfig : Profile
    {
        private readonly static Dictionary<int, bool> transactionStatus = new()
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

        public AutoMapperConfig()
        {

            CreateMap<ProductCreateDTO, ProductModel>()
                .ForMember(Product => Product.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(Product => Product.IsAvailable, opt => opt.MapFrom(src => true))
                .ForMember(Product => Product.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()))
                .ForMember(Product => Product.Rating, opt => opt.MapFrom(src => new List<ProductRating>()));

            CreateMap<ProductUpdateDTO, ProductModel>();

            CreateMap<ProductModel, ProductInfoDTO>()
                .ForMember(p => p.Brand, opt => opt.MapFrom<ProductBrandResolver>())
                .ForMember(p => p.Tags, opt => opt.MapFrom<ProductTagsResolver>())
                .ForMember(p => p.Rate, opt => opt.MapFrom<ProductRateResolver>());

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
                .ForMember(u => u.Comments, opt => opt.MapFrom(src => new List<CommentsModel>()));

            CreateMap<BrandRefreshToken, BrandRefreshTokenDTO>();
            CreateMap<UserRefreshToken, UserRefreshTokenDTO>();
            CreateMap<Address, AddressDTO>();

            CreateMap<CommentsModel, CommentsDTO>()
                .ForMember(u => u.User, opt => opt.MapFrom<CommentUserResolver>())
                .ForMember(p => p.Product, opt => opt.MapFrom<CommentProductResolver>());

            CreateMap<BrandNewsletterModel, BrandNewsLetterDTO>()
                .ForMember(b => b.Brand, opt => opt.MapFrom<BNewsletterBrandResolver>())
                .ForMember(u => u.User, opt => opt.MapFrom<BNewsLetterUserResolver>());

            CreateMap<ProductNewsLetterModel, ProductNewsLetterDTO>()
                .ForMember(p => p.Product, opt => opt.MapFrom<PNewsLetterProductResolver>())
                .ForMember(u => u.User, opt => opt.MapFrom<PNewsLetterUserResolver>());


            CreateMap<PaymentOutcomeValidationResponse, PaymentModel>()
                .ForMember(p => p.DateTimePaid, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.TransactionTime).DateTime))
                .ForMember(p => p.Successful, opt => opt.MapFrom(src => transactionStatus[src.Status]));

            CreateMap<CartItems, CartItemsDTO>()
                .ForMember(p => p.Product, opt => opt.MapFrom<CartItemProductResolver>());

            CreateMap<ShoppingCartModel, ShoppingCartDTO>()
                .ForMember(ci => ci.Items, opt => opt.MapFrom<ShoppingCartCartItemResolver>())
                .ForMember(o => o.Order, opt => opt.MapFrom<ShoppingCartOrderResolver>())
                .ForMember(p => p.Payments, opt => opt.MapFrom<ShoppingCartPaymentResolver>())
                .ForMember(u => u.User, opt => opt.MapFrom<ShoppingCartUserResolver>());

            CreateMap<PaymentModel, PaymentDTO>()
                .ForMember(sc => sc.ShoppingCart, opt => opt.MapFrom<PaymentShoppingCartResolver>());

            CreateMap<OrdersModel, OrdersDTO>()
                .ForMember(u => u.User, opt => opt.MapFrom<OrdersUserResolver>())
                .ForMember(p => p.Payment, opt => opt.MapFrom<OrdersPaymentResolver>())
                .ForMember(u => u.ShoppingCart, opt => opt.MapFrom<OrdersShoppingCartResolver>());

            CreateMap<TagModel, TagInfoDTO>()
                .ForMember(t => t.Products, opt => opt.MapFrom<TagProductResolver>());
        }
    }
    public class ProductBrandResolver : IValueResolver<ProductModel, ProductInfoDTO, BrandInfoDTO>
    {
        private readonly IBrandService _brandService;

        public ProductBrandResolver(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public BrandInfoDTO Resolve(ProductModel source, ProductInfoDTO destination, BrandInfoDTO destMember, ResolutionContext context)
        {
            return _brandService.BrandModelToInfo(source.Brand);
        }
    }
    public class ProductTagsResolver : IValueResolver<ProductModel, ProductInfoDTO, List<TagInfoDTO>?>
    {
        private readonly IMapper _mapper;

        public ProductTagsResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<TagInfoDTO>? Resolve(ProductModel source, ProductInfoDTO destination, List<TagInfoDTO>? destMember, ResolutionContext context)
        {
            return source.Tags?.Select(x => _mapper.Map<TagInfoDTO>(x)).ToList() ?? [];
        }
    }
    public class ProductRateResolver : IValueResolver<ProductModel, ProductInfoDTO, double?>
    {
        private readonly IPexitaTools _pexitaTools;

        public ProductRateResolver(IPexitaTools pexitaTools)
        {
            _pexitaTools = pexitaTools;
        }

        public double? Resolve(ProductModel source, ProductInfoDTO destination, double? destMember, ResolutionContext context)
        {
            return _pexitaTools.GetRating(source.Rating?.Select(x => x.Rating).ToList() ?? null) ?? null;
        }
    }
    public class CommentUserResolver : IValueResolver<CommentsModel, CommentsDTO, UserInfoDTO>
    {
        private readonly IUserService _userService;

        public CommentUserResolver(IUserService userService)
        {
            _userService = userService;
        }

        public UserInfoDTO Resolve(CommentsModel source, CommentsDTO destination, UserInfoDTO destMember, ResolutionContext context)
        {
            return _userService.UserModelToInfoDTO(source.User);
        }
    }
    public class CommentProductResolver : IValueResolver<CommentsModel, CommentsDTO, ProductInfoDTO>
    {
        private readonly IProductService _productService;

        public CommentProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public ProductInfoDTO Resolve(CommentsModel source, CommentsDTO destination, ProductInfoDTO destMember, ResolutionContext context)
        {
            return _productService.ProductModelToInfoDTO(source.Product);
        }
    }
    public class BNewsletterBrandResolver : IValueResolver<BrandNewsletterModel, BrandNewsLetterDTO, BrandInfoDTO>
    {
        private readonly IBrandService _brandService;

        public BNewsletterBrandResolver(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public BrandInfoDTO Resolve(BrandNewsletterModel source, BrandNewsLetterDTO destination, BrandInfoDTO destMember, ResolutionContext context)
        {
            return _brandService.BrandModelToInfo(source.Brand);
        }
    }
    public class BNewsLetterUserResolver : IValueResolver<BrandNewsletterModel, BrandNewsLetterDTO, UserInfoDTO>
    {
        private readonly IUserService _userService;

        public BNewsLetterUserResolver(IUserService userService)
        {
            _userService = userService;
        }

        public UserInfoDTO Resolve(BrandNewsletterModel source, BrandNewsLetterDTO destination, UserInfoDTO destMember, ResolutionContext context)
        {
            return _userService.UserModelToInfoDTO(source.User);
        }
    }
    public class PNewsLetterProductResolver : IValueResolver<ProductNewsLetterModel, ProductNewsLetterDTO, ProductInfoDTO>
    {
        private readonly IProductService _productService;

        public PNewsLetterProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public ProductInfoDTO Resolve(ProductNewsLetterModel source, ProductNewsLetterDTO destination, ProductInfoDTO destMember, ResolutionContext context)
        {
            return _productService.ProductModelToInfoDTO(source.Product);
        }
    }
    public class PNewsLetterUserResolver : IValueResolver<ProductNewsLetterModel, ProductNewsLetterDTO, UserInfoDTO>
    {
        private readonly IUserService _userService;

        public PNewsLetterUserResolver(IUserService userService)
        {
            _userService = userService;
        }

        public UserInfoDTO Resolve(ProductNewsLetterModel source, ProductNewsLetterDTO destination, UserInfoDTO destMember, ResolutionContext context)
        {
            return _userService.UserModelToInfoDTO(source.User);
        }
    }
    public class PaymentShoppingCartResolver : IValueResolver<PaymentModel, PaymentDTO, ShoppingCartDTO>
    {
        private readonly IMapper _mapper;

        public PaymentShoppingCartResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ShoppingCartDTO Resolve(PaymentModel source, PaymentDTO destination, ShoppingCartDTO destMember, ResolutionContext context)
        {
            return _mapper.Map<ShoppingCartDTO>(source.ShoppingCart);
        }
    }
    public class ShoppingCartCartItemResolver : IValueResolver<ShoppingCartModel, ShoppingCartDTO, List<CartItemsDTO>>
    {
        private readonly IMapper _mapper;

        public ShoppingCartCartItemResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<CartItemsDTO> Resolve(ShoppingCartModel source, ShoppingCartDTO destination, List<CartItemsDTO> destMember, ResolutionContext context)
        {
            return source.CartItems.Select(x => _mapper.Map<CartItemsDTO>(x)).ToList();
        }
    }
    public class ShoppingCartOrderResolver : IValueResolver<ShoppingCartModel, ShoppingCartDTO, OrdersDTO?>
    {
        private readonly IMapper _mapper;

        public ShoppingCartOrderResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public OrdersDTO? Resolve(ShoppingCartModel source, ShoppingCartDTO destination, OrdersDTO? destMember, ResolutionContext context)
        {
            return _mapper.Map<OrdersDTO>(source.Order);
        }
    }
    public class ShoppingCartUserResolver : IValueResolver<ShoppingCartModel, ShoppingCartDTO, UserInfoDTO>
    {
        private readonly IUserService _userService;

        public ShoppingCartUserResolver(IUserService userService)
        {
            _userService = userService;
        }

        public UserInfoDTO Resolve(ShoppingCartModel source, ShoppingCartDTO destination, UserInfoDTO destMember, ResolutionContext context)
        {
            return _userService.UserModelToInfoDTO(source.User);
        }
    }
    public class ShoppingCartPaymentResolver : IValueResolver<ShoppingCartModel, ShoppingCartDTO, List<PaymentDTO>?>
    {
        private readonly IMapper _mapper;

        public ShoppingCartPaymentResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<PaymentDTO>? Resolve(ShoppingCartModel source, ShoppingCartDTO destination, List<PaymentDTO>? destMember, ResolutionContext context)
        {
            return source.Payments.Select(paymentModel => _mapper.Map<PaymentDTO>(paymentModel)).ToList();
        }
    }
    public class OrdersUserResolver : IValueResolver<OrdersModel, OrdersDTO, UserInfoDTO>
    {
        private readonly IUserService _userService;

        public OrdersUserResolver(IUserService userService)
        {
            _userService = userService;
        }

        public UserInfoDTO Resolve(OrdersModel source, OrdersDTO destination, UserInfoDTO destMember, ResolutionContext context)
        {
            return _userService.UserModelToInfoDTO(source.User);
        }
    }
    public class OrdersPaymentResolver : IValueResolver<OrdersModel, OrdersDTO, PaymentDTO>
    {
        private readonly IMapper _mapper;

        public OrdersPaymentResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public PaymentDTO Resolve(OrdersModel source, OrdersDTO destination, PaymentDTO destMember, ResolutionContext context)
        {
            return _mapper.Map<PaymentDTO>(source.Payment);
        }
    }
    public class OrdersShoppingCartResolver : IValueResolver<OrdersModel, OrdersDTO, ShoppingCartDTO>
    {
        private readonly IMapper _mapper;

        public OrdersShoppingCartResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ShoppingCartDTO Resolve(OrdersModel source, OrdersDTO destination, ShoppingCartDTO destMember, ResolutionContext context)
        {
            return _mapper.Map<ShoppingCartDTO>(source.ShoppingCart);
        }
    }
    public class CartItemProductResolver : IValueResolver<CartItems, CartItemsDTO, ProductInfoDTO>
    {
        private readonly IProductService _productService;

        public CartItemProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public ProductInfoDTO Resolve(CartItems source, CartItemsDTO destination, ProductInfoDTO destMember, ResolutionContext context)
        {
            return _productService.ProductModelToInfoDTO(source.Product);
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
    public class TagProductResolver : IValueResolver<TagModel, TagInfoDTO, List<ProductInfoDTO>?>
    {
        private readonly IProductService _productService;

        public TagProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public List<ProductInfoDTO>? Resolve(TagModel source, TagInfoDTO destination, List<ProductInfoDTO>? destMember, ResolutionContext context)
        {
            return source.Products.Select(x => _productService.ProductModelToInfoDTO(x)).ToList();
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
}
