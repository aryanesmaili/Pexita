using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
using Pexita.Exceptions;
using Pexita.Services;
using Pexita.Services.Interfaces;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public IActionResult GetAllProducts()
        {
            try
            {
                var result = _productService.GetProducts();
                return Ok(result);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}, {e.InnerException}");
            }
        }

        [HttpGet("products/get/{count:int}")]
        public IActionResult GetProducts(int count)
        {
            try
            {
                return Ok(_productService.GetProducts(count));
            }

            catch (ArgumentOutOfRangeException)
            {
                return BadRequest($"Count ({count}) was more than the records we had");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message + " " + nameof(count));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}, {e.InnerException}");
            }
        }

        [HttpGet("products/{id:int}")]
        public IActionResult GetProductByID(int id)
        {
            try
            {
                return Ok(_productService.GetProductByID(id));
            }
            catch (NotFoundException)
            {
                return NotFound(nameof(id));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}, {e.InnerException}");
            }
        }

        [HttpPost("product/add")]
        public IActionResult AddProduct([FromBody] ProductCreateVM product)
        {
            try
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product));

                _productService.AddProduct(product);
                return Ok();
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (FormatException fe)
            {
                return BadRequest($"saving the given file {fe.Message} failed because of format");

            }

            catch (ArgumentNullException e)
            {
                return BadRequest($"Arguement null {e.Message}");
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}, {e.InnerException}");
            }
        }

        [HttpPut("product/update/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateVM product)
        {
            try
            {
                return Ok(_productService.UpdateProductInfo(id, product));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("product/update/rate/{id:int}")]
        public IActionResult UpdateProductRate(int id, [FromBody] int value)
        {
            try
            {
                return Ok(_productService.UpdateProductRate(id, value));
            }

            catch (NotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}");
            }
        }

        [HttpDelete("products/delete/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                return _productService.DeleteProduct(id) ? NoContent() : BadRequest(id);

            }
            catch (NotFoundException)
            {
                return NotFound(nameof(id));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}");
            }
        }

        [HttpPost("/product/Comments/Add/{id:int}")]
        public IActionResult AddCommentToProduct(int id, [FromBody] CommentsModel Comment)
        {
            try
            {
                return Ok(_productService.AddCommentToProduct(id, Comment));
            }
            catch (NotFoundException)
            {
                return NotFound($"Entity was not found : {nameof(id)}");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}");
            }
        }
    }
}

