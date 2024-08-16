using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.Security.Claims;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<UserCreateDTO> _createValidator;
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IValidator<UserUpdateDTO> _updateValidator;
        private readonly IValidator<AddressDTO> _addressValidator;
        private readonly INewsletterService _newsletterService;

        public UserController(IUserService userService,
            IValidator<UserCreateDTO> CreateValidator,
            IValidator<LoginDTO> LoginValidator,
            IValidator<UserUpdateDTO> UpdateValidator,
            IValidator<AddressDTO> addressValidator,
            INewsletterService newsletterService)
        {
            _userService = userService;
            _createValidator = CreateValidator;
            _loginValidator = LoginValidator;
            _updateValidator = UpdateValidator;
            _addressValidator = addressValidator;
            _newsletterService = newsletterService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                return Ok(_userService.GetUsers());
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
        [HttpGet("get")]
        public IActionResult GetUsers(int count)
        {
            try
            {
                return Ok(_userService.GetUsers(count));
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
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            try
            {
                return Ok(await _userService.GetUserByID(id));
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
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
        {
            try
            {
                await _loginValidator.ValidateAndThrowAsync(loginDTO);
                var result = await _userService.Login(loginDTO);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound(loginDTO);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] UserCreateDTO userCreateDTO)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(userCreateDTO);
                await _createValidator.ValidateAndThrowAsync(userCreateDTO);
                var result = await _userService.Register(userCreateDTO);
                return Ok(result);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
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
        [Authorize(Policy = "AllUsers")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] string logout)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(logout);

                await _userService.RevokeToken(logout);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return BadRequest(logout);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.StackTrace);
            }
        }

        [Authorize(Policy = "AllUsers")]
        [HttpPut("Edit")]
        public async Task<IActionResult>  UpdateUser([FromForm] UserUpdateDTO userUpdateDTO)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _updateValidator.ValidateAndThrowAsync(userUpdateDTO);

                UserInfoDTO result = await _userService.UpdateUser(userUpdateDTO, requestingUsername!);

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

            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] string userInfo)
        {
            try
            {
                UserInfoDTO user = await _userService.ResetPassword(userInfo);

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
        public async Task<IActionResult> CheckResetCode([FromBody] UserInfoDTO user, [FromQuery] string Code)
        {
            try
            {
                var result = await _userService.CheckResetCode(user, Code);
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
        [Authorize(Policy = "OnlyUsers")]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserInfoDTO userID, [FromQuery] string newPassword, [FromQuery] string confirmPassword)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                var result = await _userService.ChangePassword(userID, newPassword, confirmPassword, requestingUsername!);
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
        [Authorize(Policy = "OnlyUsers")]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _userService.TokenRefresher(refreshToken);
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
        [Authorize(Policy = "OnlyUsers")]
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                await _userService.DeleteUser(id, requestingUsername!);
                return Ok();
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (NotFoundException)
            {
                return NotFound($"User with ID:{id} not Found");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Policy = "OnlyUsers")]
        [HttpGet("Addresses/{id:int}")]
        public async Task<IActionResult> GetAddresses(int id)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                return Ok(await _userService.GetAddresses(id, requestingUsername!));
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (NotFoundException)
            {
                return NotFound($"User with ID:{id} not Found");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [Authorize(Policy = "OnlyUsers")]
        [HttpPost("Addresses/Add/{id:int}")]
        public async Task<IActionResult> AddAddress(int id, [FromForm] AddressDTO address)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                ArgumentNullException.ThrowIfNull(address);
                await _addressValidator.ValidateAndThrowAsync(address);
                await _userService.AddAddress(id, address, requestingUsername!);

                return Ok();
            }
            catch (NotAuthorizedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (NotFoundException)
            {
                return NotFound($"Address ID {address.ID} Not found");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
        [Authorize(Policy = "OnlyUsers")]
        [HttpPut("Address/Edit/{id:int}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromForm] AddressDTO address)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                await _addressValidator.ValidateAndThrowAsync(address);
                await _userService.UpdateAddress(id, address, requestingUsername!);
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
        [Authorize(Policy = "OnlyUsers")]
        [HttpDelete("Address/Delete/{id:int}")]
        public async Task<IActionResult> RemoveAddress(int id, [FromBody] AddressDTO address)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                await _userService.DeleteAddress(id, address.ID, requestingUsername!);
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
                return BadRequest(e.Message);
            }
        }
        [HttpGet("Comments/{id:int}")]
        public IActionResult GetComments(int id)
        {
            try
            {
                return Ok(_userService.GetComments(id));
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
        [Authorize(Policy = "OnlyUsers")]
        [HttpPost("Newsletters/Add/Product")]
        public async Task<IActionResult> AddProductNewsletter([FromQuery] int UserID, [FromQuery] int ProductID)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _newsletterService.AddProductNewsLetter(UserID, ProductID, requestingUsername!);
                return Ok(ProductID);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (NotAuthorizedException)
            {
                return Unauthorized(UserID);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [Authorize(Policy = "OnlyUsers")]
        [HttpPost("Newsletters/Add/Brand")]
        public async Task<IActionResult> AddBrandNewsletter([FromQuery] int UserID, [FromQuery] int ProductID)
        {
            var requestingUsername = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                await _newsletterService.AddBrandNewProductNewsLetter(UserID, ProductID, requestingUsername!);

                return Ok(ProductID);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (NotAuthorizedException)
            {
                return Unauthorized(UserID);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}