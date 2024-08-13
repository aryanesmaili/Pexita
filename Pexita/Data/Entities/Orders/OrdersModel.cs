using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.User;
using static Pexita.Data.Entities.Orders.OrdersModel;

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
        public required UserModel User { get; set; }
        public int PaymentID { get; set; }
        public required PaymentModel Payment { get; set; }
        public List<BrandOrder>? BrandOrders { get; set; }
        public int ShoppingCartID { get; set; }
        public required ShoppingCartModel ShoppingCart { get; set; }
    }

    public class OrdersDTO
    {
        public int ID { get; set; }
        public DateTime DateIssued { get; set; }
        public OrderStatus Status { get; set; }
        public int UserID { get; set; }
        public required UserInfoDTO User { get; set; }
        public int PaymentID { get; set; }
        public required PaymentDTO Payment { get; set; }
        public required List<BrandOrder> BrandOrders { get; set; }
        public int ShoppingCartID { get; set; }
        public required ShoppingCartDTO ShoppingCart { get; set; }
    }
}
