using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<UserCreateVM> _createValidator;
        private readonly IValidator<UserLoginVM> _loginValidator;
        private readonly IValidator<UserUpdateVM> _updateValidator;
        private readonly IValidator<Address> _addressValidator;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserController(IUserService userService,
            IValidator<UserCreateVM> CreateValidator,
            IValidator<UserLoginVM> LoginValidator,
            IValidator<UserUpdateVM> UpdateValidator,
            IValidator<Address> addressValidator,
            IHttpContextAccessor contextAccessor)
        {
            _userService = userService;
            _createValidator = CreateValidator;
            _loginValidator = LoginValidator;
            _updateValidator = UpdateValidator;
            _addressValidator = addressValidator;
            _contextAccessor = contextAccessor;
        }


        [HttpGet("Users")]
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
        }

        [HttpGet("Users/get/{id:int}")]
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
        }

        [HttpGet("User/{id:int}")]
        public IActionResult GetUserByID(int id)
        {
            try
            {
                return Ok(_userService.GetUserByID(id));
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);

            }
        }
        [HttpPost("Auth/Login")]
        public async Task<IActionResult> Login(UserLoginVM loginVM)
        {
            try
            {
                await _loginValidator.ValidateAndThrowAsync(loginVM);

                var token = await _userService.Login(loginVM);
                return Ok(token);
            }
            catch (NotFoundException e)
            {
                return NotFound(nameof(loginVM.UserName));
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPost("Auth/Register")]
        public async Task<IActionResult> Register([FromForm] UserCreateVM userCreateVM)
        {
            try
            {
                bool result = false;

                if (userCreateVM == null)
                    throw new ArgumentNullException($"{nameof(userCreateVM)} is Null");

                await _createValidator.ValidateAndThrowAsync(userCreateVM);
                result = await _userService.Register(userCreateVM);

                return Ok();
            }

            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }

            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Policy = "AllUsers")]
        [HttpPut("User/Edit")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateVM userUpdateVM)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;
            try
            {
                UserInfoVM? result = null;

                if (userUpdateVM == null)
                    throw new ArgumentNullException($"{nameof(userUpdateVM)} is Null");

                await _updateValidator.ValidateAndThrowAsync(userUpdateVM);
                result = await _userService.UpdateUser(userUpdateVM, requestingUsername);

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

            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Policy = "AllUsers")]
        [HttpPut("User/ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] UserLoginVM userLoginVM)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;

            try
            {
                UserLoginVM? result = null;

                await _loginValidator.ValidateAndThrowAsync(userLoginVM);
                result = await _userService.ResetPassword(userLoginVM, requestingUsername);

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
        }
        [Authorize(Policy = "AllUsers")]
        [HttpPut("User/ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserLoginVM userLoginVM)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;

            try
            {

                bool result = false;

                await _loginValidator.ValidateAndThrowAsync(userLoginVM);
                result = await _userService.ChangePassword(userLoginVM, requestingUsername);

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
        }
        [Authorize(Policy = "AllUsers")]
        [HttpDelete("User/Delete/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;

            try
            {
                return await _userService.DeleteUser(id, requestingUsername) ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (NotFoundException)
            {
                return NotFound($"User with ID:{id} not Found");
            }
        }
        [Authorize(Policy = "AllUsers")]
        [HttpGet("User/Addresses/{id:int}")]
        public async Task<IActionResult> GetAddresses(int id)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;

            try
            {
                return Ok(await _userService.GetAddresses(id, requestingUsername));
            }
            catch (NotFoundException)
            {
                return NotFound($"User with ID:{id} not Found");
            }
        }
        [Authorize(Policy = "AllUsers")]
        [HttpPost("User/Addresses/{id:int}")]
        public async Task<IActionResult> AddAddress(int id, [FromBody] Address address)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;

            try
            {
                bool result = false;
                if (address == null)
                    throw new ArgumentNullException($"{nameof(address)} is Null");

                if (_addressValidator.Validate(address, options => options.ThrowOnFailures()).IsValid)
                    result = await _userService.AddAddress(id, address, requestingUsername);

                return Ok(false);

            }

            catch (NotFoundException)
            {
                return NotFound($"Address ID : {address.ID} Not found");
            }

            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }

            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Policy = "AllUsers")]
        [HttpPut("User/Address/Edit/{id:int}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] Address address)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;

            try
            {
                return Ok(await _userService.UpdateAddress(id, address, requestingUsername));
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
        [Authorize(Policy = "AllUsers")]
        [HttpDelete("User/Address/Delete/{id:int}")]
        public async Task<IActionResult> RemoveAddress(int id, [FromBody] Address address)
        {
            var requestingUsername = _contextAccessor.HttpContext!.User?.Identity?.Name;

            try
            {
                await _addressValidator.ValidateAndThrowAsync(address);
                return Ok(await _userService.DeleteAddress(id, address.ID, requestingUsername));
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

        [HttpGet("User/Comments/{id:int}")]
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
        }
    }
}
