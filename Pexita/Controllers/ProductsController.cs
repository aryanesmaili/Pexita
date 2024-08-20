using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.Security.Claims;

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
        private readonly IValidator<CommentsDTO> _productCommentValidator;
        private readonly ITagsService _tagsService;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProductsController(IProductService productService, IValidator<ProductCreateDTO> productCreateValidator
            , IValidator<ProductUpdateDTO> productUpdateValidator, IValidator<UpdateProductRateDTO> productRateValidator, IValidator<CommentsDTO> productCommentValidator, IHttpContextAccessor contextAccessor, ITagsService tagsService)
        {
            _productService = productService;
            _productCreateValidator = productCreateValidator;
            _productUpdateValidator = productUpdateValidator;
            _productRateValidator = productRateValidator;
            _productCommentValidator = productCommentValidator;
            _contextAccessor = contextAccessor;
            _tagsService = tagsService;
        }

        [HttpGet()]
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

        [HttpGet("get/{count:int}")]
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            try
            {
                return Ok(await _productService.GetProductByID(id));
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"there was an error processing your request: {e.Message}, {e.InnerException}");
            }
        }

        [Authorize(Policy = "Brand")]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateDTO product)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                ArgumentNullException.ThrowIfNull(product);

                await _productCreateValidator.ValidateAndThrowAsync(product);

                var result = await _productService.AddProduct(product, requestingUsername!);

                return Ok(result);
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
                return BadRequest($"Argument null {e.Message}");
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{e.Message}, {e.InnerException} \n \n \n \n \n \n{e.StackTrace}");
            }
        }
        [Authorize(Policy = "Brand")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDTO product)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                ProductInfoDTO? update = null;

                await _productUpdateValidator.ValidateAndThrowAsync(product);

                update = await _productService.UpdateProductInfo(id, product, requestingUsername!);
                return Ok(update);
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "OnlyUsers")]
        [HttpPut("update/rate")]
        public async Task<IActionResult> UpdateProductRate([FromForm] UpdateProductRateDTO rateDTO)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _productRateValidator.ValidateAndThrowAsync(rateDTO);
                await _productService.UpdateProductRate(rateDTO, requestingUsername!);
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
            catch (ValidationException e)
            {
                return BadRequest($"{e.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.StackTrace);
            }
        }
        [Authorize(Policy = "Brand")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                await _productService.DeleteProduct(id, requestingUsername!);
                return NoContent();
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
        [HttpPost("Comments/Add")]
        public async Task<IActionResult> AddCommentToProduct(CommentsDTO commentDTO)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                await _productCommentValidator.ValidateAndThrowAsync(commentDTO);

                return Ok(_productService.AddCommentToProduct(commentDTO, requestingUsername!));
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
        [HttpGet("Tags")]
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                return Ok(await _tagsService.GetAllTags());
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [Authorize(Policy = "AllUsers")]
        [HttpPost("Tags/Add")]
        public async Task<IActionResult> AddTag([FromForm] string tagTitle)
        {
            try
            {
                if (string.IsNullOrEmpty(tagTitle))
                    throw new ArgumentNullException(nameof(tagTitle));

                await _tagsService.AddTag(tagTitle);
                return Ok();
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
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("Tags/Delete/{TagID:int}")]
        public async Task<IActionResult> DeleteTag(int TagID)
        {
            var requesterRole = User.FindFirstValue(ClaimTypes.Role);
            try
            {
                if (requesterRole != "admin")
                    throw new NotAuthorizedException($"This user does not have authorization.");
                await _tagsService.DeleteTag(TagID);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}