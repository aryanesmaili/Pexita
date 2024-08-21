using AutoMapper;
using Pexita.Data;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.User;
using Pexita.Services;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Utility.MapperConfigs
{
    public class CartMapper : Profile
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

        public CartMapper()
        {

            CreateMap<PaymentOutcomeValidationResponse, PaymentModel>()
                .ForMember(p => p.DateTimePaid, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.TransactionTime).DateTime))
                .ForMember(p => p.Successful, opt => opt.MapFrom(src => transactionStatus[src.Status]));

            CreateMap<CartItems, CartItemsDTO>()
                .ForMember(p => p.Product, opt => opt.MapFrom<CartItemsDTOProductResolver>());

            CreateMap<CartItemsDTO, CartItems>()
                .ForMember(p => p.Product, opt => opt.MapFrom<CartItemsProductResolver>());

            CreateMap<ShoppingCartModel, ShoppingCartDTO>()
                .ForMember(ci => ci.Items, opt => opt.MapFrom<ShoppingCartDTOCartItemResolver>())
                .ForMember(o => o.Order, opt => opt.MapFrom<ShoppingCartDTOOrderResolver>())
                .ForMember(p => p.Payments, opt => opt.MapFrom<ShoppingCartDTOPaymentResolver>());

            CreateMap<ShoppingCartDTO, ShoppingCartModel>()
                .ForMember(s => s.CartItems, opt => opt.MapFrom<ShoppingCartModelItemResolver>())
                .ForMember(s => s.Payments, opt => opt.MapFrom(src => new List<PaymentModel>()))
                .ForMember(s => s.Order, opt => opt.MapFrom(src => (object)null));

            CreateMap<PaymentModel, PaymentDTO>()
                .ForMember(sc => sc.ShoppingCart, opt => opt.MapFrom<PaymentShoppingCartResolver>());

            CreateMap<OrdersModel, OrdersDTO>()
                .ForMember(u => u.User, opt => opt.MapFrom<OrdersUserResolver>())
                .ForMember(p => p.Payment, opt => opt.MapFrom<OrdersPaymentResolver>())
                .ForMember(u => u.ShoppingCart, opt => opt.MapFrom<OrdersShoppingCartResolver>());
        }
    }

    public class CartItemsProductResolver : IValueResolver<CartItemsDTO, CartItems, ProductModel>
    {
        private readonly AppDBContext _context;

        public CartItemsProductResolver(AppDBContext context)
        {
            _context = context;
        }

        public ProductModel Resolve(CartItemsDTO source, CartItems destination, ProductModel destMember, ResolutionContext context)
        {
            ProductModel? i = _context.Products.Find(source.ProductID) ?? throw new NotFoundException($"Product {source.ProductID} not found.");
            return i;
        }
    }
    public class ShoppingCartModelItemResolver(IMapper mapper) : IValueResolver<ShoppingCartDTO, ShoppingCartModel, List<CartItems>>
    {
        private readonly IMapper _mapper = mapper;

        public List<CartItems> Resolve(ShoppingCartDTO source, ShoppingCartModel destination, List<CartItems> destMember, ResolutionContext context)
        {
            return source.Items.Select(x => _mapper.Map<CartItems>(x)).ToList();
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
    public class ShoppingCartDTOCartItemResolver : IValueResolver<ShoppingCartModel, ShoppingCartDTO, List<CartItemsDTO>>
    {
        private readonly IMapper _mapper;

        public ShoppingCartDTOCartItemResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<CartItemsDTO> Resolve(ShoppingCartModel source, ShoppingCartDTO destination, List<CartItemsDTO> destMember, ResolutionContext context)
        {
            return source.CartItems.Select(x => _mapper.Map<CartItemsDTO>(x)).ToList();
        }
    }
    public class ShoppingCartDTOOrderResolver : IValueResolver<ShoppingCartModel, ShoppingCartDTO, OrdersDTO?>
    {
        private readonly IMapper _mapper;

        public ShoppingCartDTOOrderResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public OrdersDTO? Resolve(ShoppingCartModel source, ShoppingCartDTO destination, OrdersDTO? destMember, ResolutionContext context)
        {
            return _mapper.Map<OrdersDTO>(source.Order);
        }
    }
    public class ShoppingCartDTOPaymentResolver : IValueResolver<ShoppingCartModel, ShoppingCartDTO, List<PaymentDTO>?>
    {
        private readonly IMapper _mapper;

        public ShoppingCartDTOPaymentResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<PaymentDTO>? Resolve(ShoppingCartModel source, ShoppingCartDTO destination, List<PaymentDTO>? destMember, ResolutionContext context)
        {
            return source.Payments?.Select(paymentModel => _mapper.Map<PaymentDTO>(paymentModel)).ToList();
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
    public class CartItemsDTOProductResolver : IValueResolver<CartItems, CartItemsDTO, ProductInfoDTO>
    {
        private readonly IProductService _productService;

        public CartItemsDTOProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public ProductInfoDTO Resolve(CartItems source, CartItemsDTO destination, ProductInfoDTO destMember, ResolutionContext context)
        {
            return _productService.ProductModelToInfoDTO(source.Product);
        }
    }
}
