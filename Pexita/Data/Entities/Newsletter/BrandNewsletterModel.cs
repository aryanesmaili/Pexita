using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Newsletter
{
    public class BrandNewsletterModel
    {
        public int ID { get; set; }
        public int BrandID { get; set; }
        public BrandModel? Brand { get; set; }

        public int UserID { get; set; }
        public UserModel User { get; set; }
    }
}
