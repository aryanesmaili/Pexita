using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Newsletter
{
    public class BrandNewsletterModel
    {
        public int ID { get; set; }
        public int BrandID { get; set; }
        public required BrandModel Brand { get; set; }
        public int UserID { get; set; }
        public required UserModel User { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
    public class BrandNewsLetterDTO
    {
        public int ID { get; set; }
        public int BrandID { get; set; }
        public required BrandInfoDTO Brand { get; set; }
        public int UserID { get; set; }
        public required UserInfoDTO User { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}
