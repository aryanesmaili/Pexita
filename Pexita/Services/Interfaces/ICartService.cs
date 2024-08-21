using Pexita.Data.Entities.ShoppingCart;

namespace Pexita.Services.Interfaces
{
    public interface ICartService
    {
        public Task<ShoppingCartDTO> GetCart(int id, string reqUser);
        public Task<ShoppingCartDTO> AddCart(ShoppingCartDTO cart, string reqUser);
        public Task<ShoppingCartDTO> UpdateCart(ShoppingCartDTO cart, string reqUser);
        public ShoppingCartDTO CartModelToDTO(ShoppingCartModel cart);
        public Task DeleteCart(int id, string reqUser);
    }
}
