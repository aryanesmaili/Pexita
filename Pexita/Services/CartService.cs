using Pexita.Data;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Services.Interfaces;

namespace Pexita.Services
{
    public class CartService(AppDBContext context) : ICartService
    {
        private readonly AppDBContext _context = context;

        public Task<ShoppingCartDTO> GetCart(int id)
        {
            throw new NotImplementedException();
        }
        public Task<ShoppingCartDTO> AddCart(ShoppingCartDTO cart)
        {
            throw new NotImplementedException();
        }

        public Task<ShoppingCartDTO> UpdateCart(ShoppingCartDTO cart)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCart(int id)
        {
            throw new NotImplementedException();
        }
    }
}
