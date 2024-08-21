using AutoMapper;
using Pexita.Data;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;

namespace Pexita.Services
{
    public class CartService(AppDBContext context, IPexitaTools pexitaTools, IMapper mapper) : ICartService
    {
        private readonly AppDBContext _context = context;
        private readonly IPexitaTools _pexitaTools = pexitaTools;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Returns a shopping cart's info after the requester has been authorized.
        /// </summary>
        /// <param name="id">ID of the shoppingCart to be accessed.</param>
        /// <param name="reqUsername">Username of the user asking to receive the data (retreived from jwt).</param>
        /// <returns>a <see cref="ShoppingCartDTO"/> Object containing info about the DB record.</returns>
        public async Task<ShoppingCartDTO> GetCart(int id, string reqUsername)
        {
            return _mapper.Map<ShoppingCartDTO>(await _pexitaTools.AuthorizeCartAccess(id, reqUsername));
        }

        public async Task<ShoppingCartDTO> AddCart(ShoppingCartDTO cart, string reqUser)
        {
            UserModel user = await _pexitaTools.AuthorizeCartCreation(cart.UserID, reqUser);
            ShoppingCartModel sc = _mapper.Map<ShoppingCartModel>(cart);

            user.ShoppingCarts ??= [];
            user.ShoppingCarts.Add(sc);
            await _context.SaveChangesAsync();

            return CartModelToDTO(sc);
        }
        /// <summary>
        /// Updates the info about a ShoppingCart.
        /// </summary>
        /// <param name="cart">object containing the new information</param>
        /// <param name="reqUsername">Username asking the change.</param>
        /// <returns><see cref="ShoppingCartDTO"/> object containing the new State.</returns>
        public async Task<ShoppingCartDTO> UpdateCart(ShoppingCartDTO cart, string reqUsername)
        {
            ShoppingCartModel cartToModify = await _pexitaTools.AuthorizeCartAccess(cart.ID, reqUsername);
            cartToModify = _mapper.Map(cart, cartToModify);

            _context.Update(cartToModify);
            await _context.SaveChangesAsync();

            return CartModelToDTO(cartToModify);
        }
        /// <summary>
        /// Deletes a Cart from Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteCart(int id, string Username)
        {
            ShoppingCartModel cart = await _pexitaTools.AuthorizeCartAccess(id, Username);
            _context.Remove(cart);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Maps a DB Model to  DTO able to be shown to front-end.
        /// </summary>
        /// <param name="cart">the DB record we're going to represent.</param>
        /// <returns>a <see cref="ShoppingCartDTO"/> Object demonstrating the DB record.</returns>
        public ShoppingCartDTO CartModelToDTO(ShoppingCartModel cart)
        {
            return _mapper.Map<ShoppingCartDTO>(cart);
        }
    }
}
