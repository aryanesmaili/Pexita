using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.ShoppingCart;

namespace Pexita.Data.Entities.Payment
{
    public class PaymentModel
    {
        public enum PaymentStatus
        {
            paid,
            awaiting,
            cancelled
        }
        public int ID { get; set; }
        public PaymentStatus Status { get; set; }
        public long OperationID { get; set; }
        public DateTime DateIssued { get; set; }
        public bool Successfull { get; set; }
        public int? ShoppingCartID { get; set; }
        public ShoppingCartModel? ShoppingCart { get; set; }
    }
}
