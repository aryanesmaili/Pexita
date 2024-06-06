using FluentValidation;
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

        public UserController(IUserService userService,
            IValidator<UserCreateVM> CreateValidator,
            IValidator<UserLoginVM> LoginValidator,
            IValidator<UserUpdateVM> UpdateValidator,
            IValidator<Address> addressValidator)
        {
            _userService = userService;
            _createValidator = CreateValidator;
            _loginValidator = LoginValidator;
            _updateValidator = UpdateValidator;
            _addressValidator = addressValidator;
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

        [HttpPost("User/Register")]
        public async Task<IActionResult> Register([FromForm] UserCreateVM userCreateVM)
        {
            try
            {
                bool result = false;

                if (userCreateVM == null)
                    throw new ArgumentNullException($"{nameof(userCreateVM)} is Null");

                await _createValidator.ValidateAndThrowAsync(userCreateVM);
                result = _userService.Register(userCreateVM);

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

        [HttpPut("User/Edit")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateVM userUpdateVM)
        {
            try
            {
                UserInfoVM? result = null;

                if (userUpdateVM == null)
                    throw new ArgumentNullException($"{nameof(userUpdateVM)} is Null");

                await _updateValidator.ValidateAndThrowAsync(userUpdateVM);
                result = _userService.UpdateUser(userUpdateVM);

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

        [HttpPut("User/ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] UserLoginVM userLoginVM)
        {
            try
            {
                UserLoginVM? result = null;

                await _loginValidator.ValidateAndThrowAsync(userLoginVM);
                result = _userService.ResetPassword(userLoginVM);

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

        [HttpPut("User/ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserLoginVM userLoginVM)
        {
            try
            {
                bool result = false;

                await _loginValidator.ValidateAndThrowAsync(userLoginVM);
                result = _userService.ChangePassword(userLoginVM);

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

        [HttpDelete("User/Delete/{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                return _userService.DeleteUser(id) ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (NotFoundException)
            {
                return NotFound($"User with ID:{id} not Found");
            }
        }

        [HttpGet("User/Addresses/{id:int}")]
        public IActionResult GetAddresses(int id)
        {
            try
            {
                return Ok(_userService.GetAddresses(id));
            }
            catch (NotFoundException)
            {
                return NotFound($"User with ID:{id} not Found");
            }
        }

        [HttpPost("User/Addresses/{id:int}")]
        public IActionResult AddAddress(int id, [FromBody] Address address)
        {
            try
            {
                bool result = false;
                if (address == null)
                    throw new ArgumentNullException($"{nameof(address)} is Null");

                if (_addressValidator.Validate(address, options => options.ThrowOnFailures()).IsValid)
                    result = _userService.AddAddress(id, address);

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

        [HttpPut("User/Address/Edit/{id:int}")]
        public IActionResult UpdateAddress(int id, [FromBody] Address address)
        {
            try
            {
                return Ok(_userService.UpdateAddress(id, address));
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("User/Address/Delete/{id:int}")]
        public async Task<IActionResult> RemoveAddress(int id, [FromBody] Address address)
        {
            try
            {
                await _addressValidator.ValidateAndThrowAsync(address);
                return Ok(_userService.DeleteAddress(id, address.ID));
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
