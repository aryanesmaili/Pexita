using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Orders
{
    public class OrdersModel
    {
        public int ID { get; set; }
        public DateTime DateIssued { get; set; }
        public OrderStatus Status { get; set; }
        public enum OrderStatus
        {
            Preparing,
            Sent
        }
        // Navigation Properties
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public int PaymentID { get; set; }
        public PaymentModel Payment { get; set; }
        public int? BrandID { get; set; }
        public BrandModel? Brand { get; set; }

        // Add a reference to ShoppingCartModel
        public int ShoppingCartID { get; set; }
        public ShoppingCartModel ShoppingCart { get; set; }
    }
}
