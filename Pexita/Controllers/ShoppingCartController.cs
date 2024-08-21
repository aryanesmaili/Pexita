using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.Security.Claims;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController(ICartService cartService, IValidator<ShoppingCartDTO> validator) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;
        private readonly IValidator<ShoppingCartDTO> _validator = validator;

        [Authorize(Policy = "OnlyUsers")]
        [HttpGet("/{id:int}")]
        public async Task<IActionResult> GetShoppingCart([FromRoute] int id)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                return Ok(await _cartService.GetCart(id, requestingUsername!));
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [Authorize(Policy = "OnlyUsers")]
        [HttpPost("/Add")]
        public async Task<IActionResult> AddShoppingCart([FromForm] ShoppingCartDTO cart)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _validator.ValidateAndThrowAsync(cart);
                var result = await _cartService.AddCart(cart, requestingUsername!);
                return Ok(result);
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [Authorize(Policy ="OnlyUsers")]
        [HttpPut("/Update")]
        public async Task<IActionResult> UpdateCartInfo([FromForm] ShoppingCartDTO cartDTO)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _validator.ValidateAndThrowAsync(cartDTO);
                var result = await _cartService.UpdateCart(cartDTO, requestingUsername!);
                return Ok(result);
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [Authorize(Policy ="OnlyUsers")]
        [HttpDelete("/Delete/{id:int}")]
        public async Task<IActionResult> DeleteCart([FromRoute] int id)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _cartService.DeleteCart(id, requestingUsername!);
                return Ok();
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
