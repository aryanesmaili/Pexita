using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.User;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;
using System.Net;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
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
        public IActionResult Register([FromForm] UserCreateVM userCreateVM)
        {
            try
            {
                if (userCreateVM == null)
                    throw new ArgumentNullException($"{nameof(userCreateVM)} is Null");

                return Ok(_userService.Register(userCreateVM));
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
        public IActionResult UpdateUser([FromBody] UserUpdateVM userUpdateVM)
        {
            try
            {
                if (userUpdateVM == null)
                    throw new ArgumentNullException($"{nameof(userUpdateVM)} is Null");

                return Ok(_userService.UpdateUser(userUpdateVM));
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
        public IActionResult ResetPassword([FromBody] UserLoginVM userLoginVM)
        {
            try
            {
                if (userLoginVM == null)
                    throw new ArgumentNullException($"{nameof(userLoginVM)} is Null");

                if (string.IsNullOrWhiteSpace(userLoginVM.Password) || userLoginVM.Password.Length > 250)
                    throw new ArgumentException($"Password is Either null or Empty Or WhiteSpace!");

                if ((string.IsNullOrWhiteSpace(userLoginVM.UserName) || userLoginVM.UserName.Length > 100)
                    && (string.IsNullOrWhiteSpace(userLoginVM.Email) || userLoginVM.Email.Length > 100))
                    throw new ArgumentException($"Username is Either null or Empty Or WhiteSpace!");

                return Ok(_userService.ResetPassword(userLoginVM));
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);
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

        [HttpPut("User/ChangePassword")]
        public IActionResult ChangePassword([FromBody] UserLoginVM userLoginVM)
        {
            try
            {
                if (userLoginVM == null)
                    throw new ArgumentNullException($"{nameof(userLoginVM)} is Null");

                if (string.IsNullOrWhiteSpace(userLoginVM.Password) || userLoginVM.Password.Length > 250)
                    throw new ArgumentException($"Password is Either null or Empty Or WhiteSpace!");

                if ((string.IsNullOrWhiteSpace(userLoginVM.UserName) || userLoginVM.UserName.Length > 100)
                    && (string.IsNullOrWhiteSpace(userLoginVM.Email) || userLoginVM.Email.Length > 100))
                    throw new ArgumentException($"Username is Either null or Empty Or WhiteSpace!");

                return Ok(_userService.ChangePassword(userLoginVM));
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);
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
                if (address == null)
                    throw new ArgumentNullException($"{nameof(address)} is Null");

                if (string.IsNullOrWhiteSpace(address.Province) || address.Province.Length > 100)
                    throw new ArgumentException($"{nameof(address)} Province is not valid");

                if (string.IsNullOrWhiteSpace(address.City) || address.City.Length > 100)
                    throw new ArgumentException($"{nameof(address)} City is not valid");

                if (string.IsNullOrWhiteSpace(address.Text) || address.Text.Length > 250)
                    throw new ArgumentException($"{nameof(address)} Text is not Valid!");

                return Ok(_userService.AddAddress(id, address));

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
        public IActionResult RemoveAddress(int id, [FromBody] Address address)
        {
            try
            {
                return Ok(_userService.DeleteAddress(id, address.ID));
            }

            catch (NotFoundException e)
            {
                return NotFound(e.Message);
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
