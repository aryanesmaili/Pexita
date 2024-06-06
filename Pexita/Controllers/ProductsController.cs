using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Pexita.Utility.Exceptions;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
using Pexita.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<ProductCreateVM> _productCreateValidator;
        private readonly IValidator<ProductUpdateVM> _productUpdateValidator;
        private readonly IValidator<UpdateProductRateDTO> _productRateValidator;
        private readonly IValidator<ProductCommentDTO> _productCommentValidator;

        public ProductsController(IProductService productService, IValidator<ProductCreateVM> productCreateValidator
            , IValidator<ProductUpdateVM> productUpdateValidator, IValidator<UpdateProductRateDTO> productRateValidator, IValidator<ProductCommentDTO> productCommentValidator)
        {
            _productService = productService;
            _productCreateValidator = productCreateValidator;
            _productUpdateValidator = productUpdateValidator;
            _productRateValidator = productRateValidator;
            _productCommentValidator = productCommentValidator;
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
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateVM product)
        {
            try
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product));

                await _productCreateValidator.ValidateAndThrowAsync(product);

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
        [Authorize(Policy = "Brand")]
        [HttpPut("product/update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateVM product)
        {
            try
            {
                ProductInfoVM? update = null;

                await _productUpdateValidator.ValidateAndThrowAsync(product);

                update = _productService.UpdateProductInfo(id, product);
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
            try
            {
                await _productRateValidator.ValidateAndThrowAsync(rateDTO);
                return Ok(_productService.UpdateProductRate(rateDTO));
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
        [Authorize(Policy ="Brand")]
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
        [Authorize(Policy ="AllUsers")]
        [HttpPost("/product/Comments/Add/{id:int}")]
        public async Task<IActionResult> AddCommentToProduct(ProductCommentDTO commentDTO)
        {
            try
            {
                await _productCommentValidator.ValidateAndThrowAsync(commentDTO);

                return Ok(_productService.AddCommentToProduct(commentDTO));
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

