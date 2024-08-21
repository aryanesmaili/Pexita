using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.ShoppingCart
{
    public class ShoppingCartModel
    { // TODO: shopping carts need their own controllers
        public int ID { get; set; }
        public required List<CartItems> CartItems { get; set; }
        public double TotalPrice { get; set; }

        // Navigation Properties
        public int UserID { get; set; }
        public required UserModel User { get; set; }
        public List<PaymentModel>? Payments { get; set; }
        public OrdersModel? Order { get; set; }
    }

    public class CartItems
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public required ProductModel Product { get; set; }
        public int Count { get; set; }

        public int ShoppingCartID { get; set; }
        public required ShoppingCartModel ShoppingCart { get; set; }
    }
    public class ShoppingCartDTO
    {
        public int ID { get; set; }
        public required List<CartItemsDTO> Items { get; set; }
        public double TotalPrice { get; set; }
        public int UserID { get; set; }
        public List<PaymentDTO>? Payments { get; set; }
        public OrdersDTO? Order { get; set; }
    }

    public class CartItemsDTO
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public required ProductInfoDTO Product { get; set; }
        public int Count { get; set; }
        public int ShoppingCartID { get; set; }
    }
}
