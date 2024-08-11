using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;
using Pexita.Services;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.Security.Claims;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IValidator<BrandCreateDTO> _brandCreateValidator;
        private readonly IValidator<BrandUpdateVM> _brandUpdateValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<UserLoginVM> _userLoginValidator;
        private readonly IHttpContextAccessor _contextAccessor;

        public BrandsController(IBrandService brandService,
            IValidator<BrandCreateDTO> brandCreateValidator,
            IValidator<BrandUpdateVM> brandUpdateValidator,
            IHttpContextAccessor httpContextAccessor,
            IValidator<UserLoginVM> userLoginValidator,
            IHttpContextAccessor contextAccessor)
        {
            _brandService = brandService;
            _brandCreateValidator = brandCreateValidator;
            _brandUpdateValidator = brandUpdateValidator;
            _httpContextAccessor = httpContextAccessor;
            _userLoginValidator = userLoginValidator;
            _contextAccessor = contextAccessor;
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
        public async Task<IActionResult> GetBrands(int id)
        {
            try
            {
                return Ok(await _brandService.GetBrandByID(id));
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
        public async Task<IActionResult> Login([FromForm] UserLoginVM brand)
        {
            try
            {
                await _userLoginValidator.ValidateAndThrowAsync(brand);
                var result = await _brandService.Login(brand);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound(brand);
            }
            catch (ValidationException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, new {error = e.Message});
            }
        }

        [Authorize(Policy = "Brand")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] string logout)
        {
            try
            {
                await _brandService.RevokeToken(logout);

                return Ok();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
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

        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand([FromForm] BrandCreateDTO createVM)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(createVM);

                await _brandCreateValidator.ValidateAndThrowAsync(createVM);

                var result = await _brandService.Register(createVM);
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

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] string userInfo)
        {
            try
            {

                BrandInfoVM user = await _brandService.ResetPassword(userInfo);

                return Ok(user);
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
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

        [HttpPost("CheckResetCode")] 
        public async Task<IActionResult> CheckResetCode([FromBody] BrandInfoVM user, [FromQuery] string Code)
        {
            try
            {
                var result = await _brandService.CheckResetCode(user, Code);
                return Ok(result);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
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

        [Authorize(Policy = "Brand")]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] BrandInfoVM brandID, [FromQuery] string newPassword, [FromQuery] string confirmPassword)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                var result = await _brandService.ChangePassword(brandID, newPassword, confirmPassword, requestingUsername!);

                return Ok(result);
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
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

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _brandService.TokenRefresher(refreshToken);

                return Ok(result);
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
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

        [Authorize(Policy = "Brand")]
        [HttpPut("Edit/{id:int}")]
        public async Task<IActionResult> EditBrand([FromRoute]int id, [FromForm] BrandUpdateVM brand)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _brandUpdateValidator.ValidateAndThrowAsync(brand);

                var result = await _brandService.UpdateBrandInfo(id, brand, requestingUsername!);
                return Ok(result);
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.StackTrace);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
        }

        [Authorize(Policy = "Brand")]
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _brandService.RemoveBrand(id, requestingUsername!);
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
