using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Products;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<ProductCreateDTO> _productCreateValidator;
        private readonly IValidator<ProductUpdateDTO> _productUpdateValidator;
        private readonly IValidator<UpdateProductRateDTO> _productRateValidator;
        private readonly IValidator<ProductCommentDTO> _productCommentValidator;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProductsController(IProductService productService, IValidator<ProductCreateDTO> productCreateValidator
            , IValidator<ProductUpdateDTO> productUpdateValidator, IValidator<UpdateProductRateDTO> productRateValidator, IValidator<ProductCommentDTO> productCommentValidator, IHttpContextAccessor contextAccessor)
        {
            _productService = productService;
            _productCreateValidator = productCreateValidator;
            _productUpdateValidator = productUpdateValidator;
            _productRateValidator = productRateValidator;
            _productCommentValidator = productCommentValidator;
            _contextAccessor = contextAccessor;
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

        [Authorize(Policy = "Brand")]
        [HttpPost("product/add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDTO product)
        {
            var requestingUsername = _contextAccessor.HttpContext?.User.Identity?.Name;
            try
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product));

                await _productCreateValidator.ValidateAndThrowAsync(product);

                _productService.AddProduct(product, requestingUsername);

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
        [Authorize(Policy = "Brand")]
        [HttpPatch("product/patch/{id}")]
        public async Task<IActionResult> UpdateProductPartially(int id, [FromBody] ProductUpdateDTO product)
        {
            var requestingUsername = _contextAccessor.HttpContext?.User.Identity?.Name;
            try
            {
                ProductInfoVM? update = null;
                update = await _productService.PatchProductInfo(id, product, requestingUsername);
                return Ok(update);
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Policy = "Brand")]
        [HttpPut("product/update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO product)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;
            try
            {
                ProductInfoVM? update = null;

                await _productUpdateValidator.ValidateAndThrowAsync(product);

                update = await _productService.UpdateProductInfo(id, product, requestingUsername);
                return Ok(update);
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

        [Authorize(Policy = "OnlyUsers")]
        [HttpPut("product/update/rate/{id:int}")]
        public async Task<IActionResult> UpdateProductRate([FromBody] UpdateProductRateDTO rateDTO)
        {
            string requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name!;
            try
            {
                await _productRateValidator.ValidateAndThrowAsync(rateDTO);
                return Ok(_productService.UpdateProductRate(rateDTO, requestingUsername));
            }

            catch (NotFoundException)
            {
                return NotFound(rateDTO.ProductID);
            }
            catch (ValidationException e)
            {
                return BadRequest($"{e.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}");
            }
        }
        [Authorize(Policy = "Brand")]
        [HttpDelete("products/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            string requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name!;

            try
            {
                return await _productService.DeleteProduct(id, requestingUsername) ? NoContent() : BadRequest(id);

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
        [Authorize(Policy = "AllUsers")]
        [HttpPost("/product/Comments/Add/{id:int}")]
        public async Task<IActionResult> AddCommentToProduct(ProductCommentDTO commentDTO)
        {
            string requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name!;

            try
            {
                await _productCommentValidator.ValidateAndThrowAsync(commentDTO);

                return Ok(_productService.AddCommentToProduct(commentDTO, requestingUsername));
            }
            catch (NotFoundException)
            {
                return NotFound($"Entity was not found : {nameof(commentDTO.ProductID)}");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}");
            }
        }
    }
}

