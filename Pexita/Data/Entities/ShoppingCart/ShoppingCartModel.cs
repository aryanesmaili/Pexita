using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;
using System.ComponentModel.DataAnnotations;

namespace Pexita.Data.Entities.ShoppingCart
{
    public class ShoppingCartModel
    {
        public int ID { get; set; }
        public List<CartItems> CartItems { get; set; }
        public double TotalPrice { get; set; }
        public enum ShoppingCartStatus
        {
            Selection,
            Paid
        }
        // Navigation Properties
        public UserModel User { get; set; }
        public List<PaymentModel> Payments { get; set; }
        public OrdersModel Order { get; set; }
    }

    public class CartItems
    {
        [Key]
        public int ID { get; set; }
        public int? ProductID { get; set; }
        public ProductModel? Product { get; set; }
        public int Count { get; set; }

        // Add a reference to ShoppingCartModel
        public int ShoppingCartID { get; set; }
        public ShoppingCartModel ShoppingCart { get; set; }
    }
}
