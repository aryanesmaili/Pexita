using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IValidator<BrandCreateVM> _brandCreateValidator;
        private readonly IValidator<BrandUpdateVM> _brandUpdateValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<UserLoginVM> _userLoginValidator;

        public BrandsController(IBrandService brandService,
            IValidator<BrandCreateVM> brandCreateValidator,
            IValidator<BrandUpdateVM> brandUpdateValidator,
            IHttpContextAccessor httpContextAccessor,
            IValidator<UserLoginVM> userLoginValidator)
        {
            _brandService = brandService;
            _brandCreateValidator = brandCreateValidator;
            _brandUpdateValidator = brandUpdateValidator;
            _httpContextAccessor = httpContextAccessor;
            _userLoginValidator = userLoginValidator;
        }

        [HttpGet]
        public IActionResult GetAllBrands()
        {
            try
            {
                var brands = _brandService.GetBrands();
                return Ok(brands);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("Get/{count:int}")]
        public IActionResult GetAllBrands(int count)
        {
            try
            {
                var brands = _brandService.GetBrands(count);
                return Ok(brands);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpGet("GetBrand/{id}")]
        public IActionResult GetBrands(int id)
        {
            try
            {
                return Ok(_brandService.GetBrandByID(id));
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginVM brand)
        {
            try
            {
                await _userLoginValidator.ValidateAndThrowAsync(brand);
                string token = await _brandService.Login(brand);
                return Ok(token);
            }
            catch (NotFoundException)
            {
                return NotFound(brand);
            }
            catch (ValidationException)
            {
                return BadRequest(brand);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand([FromForm] BrandCreateVM createVM)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(createVM);

                await _brandCreateValidator.ValidateAndThrowAsync(createVM);

                var result = await _brandService.AddBrand(createVM);
                return Ok(result);
            }

            catch (ArgumentNullException e)
            {
                return BadRequest($"Argument null {e.Message}");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Brand")]
        [HttpPut("Edit/{id:int}")]
        public async Task<IActionResult> EditBrand(int id, [FromBody] BrandUpdateVM brand)
        {
            string requestingUsername = _httpContextAccessor.HttpContext!.User?.Identity?.Name!;
            try
            {
                await _brandUpdateValidator.ValidateAndThrowAsync(brand);

                await _brandService.UpdateBrandInfo(id, brand, requestingUsername);
                return Ok();
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException)
            {
                return NotFound(nameof(id));
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Brand")]
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            string requestingUsername = _httpContextAccessor.HttpContext!.User?.Identity?.Name!;
            try
            {
                await _brandService.RemoveBrand(id, requestingUsername);
                return NoContent();
            }

            catch (NotFoundException)
            {
                return NotFound(id);
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
