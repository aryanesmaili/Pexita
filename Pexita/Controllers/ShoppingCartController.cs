using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Services.Interfaces;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController(ICartService cartService, IValidator<ShoppingCartDTO> validator) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;
        private readonly IValidator<ShoppingCartDTO> _validator;

        [Authorize(Policy = "Allusers")]
        [HttpGet("/{id:int}")]
        public async Task<IActionResult> GetShoppingCart(int id)
        {
            try
            {
                return Ok(await _cartService.GetCart(id));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
