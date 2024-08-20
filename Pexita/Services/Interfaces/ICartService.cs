using Pexita.Data.Entities.ShoppingCart;

namespace Pexita.Services.Interfaces
{
    public interface ICartService
    {
        public Task<ShoppingCartDTO> GetCart(int id);
        public Task<ShoppingCartDTO> AddCart(ShoppingCartDTO cart);
        public Task<ShoppingCartDTO> UpdateCart(ShoppingCartDTO cart);
        public Task DeleteCart(int id);
    }
}
