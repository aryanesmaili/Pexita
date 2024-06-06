using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pexita.Data;
using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pexita.Services
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _Context;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public UserService(AppDBContext Context, IPexitaTools PexitaTools, IMapper Mapper, JwtSettings jwtSettings)
        {
            _Context = Context;
            _pexitaTools = PexitaTools;
            _mapper = Mapper;
            _jwtSettings = jwtSettings;
        }

        public List<UserInfoVM> GetUsers()
        {
            var users = _Context.Users
                .Include(u => u.Orders)
                .Include(u => u.Addresses)
                .Include(u => u.BrandNewsletters)
                .Include(u => u.ProductNewsletters)
                .AsNoTracking()
                .ToList();
            if (users.Count == 0)
            {
                throw new NotFoundException("No User Found");
            }
            return users.Select(UserModelToInfoVM).ToList();
        }

        public List<UserInfoVM> GetUsers(int Count)
        {
            var users = _Context.Users
                .Include(u => u.Orders)
                .Include(u => u.Addresses)
                .Include(u => u.BrandNewsletters)
                .Include(u => u.ProductNewsletters)
                .AsNoTracking()
                .Take(Count)
                .ToList();

            if (users.Count == 0)
            {
                throw new NotFoundException("No User Found");
            }

            return users.Select(UserModelToInfoVM).ToList();
        }

        public async Task<UserInfoVM> GetUserByID(int UserID)
        {
            UserModel user = await _Context.Users
                .Include(u => u.Orders).Include(u => u.Addresses)
                .Include(u => u.BrandNewsletters).Include(u => u.ProductNewsletters)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

            return UserModelToInfoVM(user);
        }

        public UserInfoVM GetUserByUserName(string userName)
        {
            UserModel user = _Context.Users
                .Include(u => u.Orders).Include(u => u.Addresses)
                .Include(u => u.BrandNewsletters)
                .Include(u => u.ProductNewsletters)
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == userName) ?? throw new NotFoundException($"User With Username:{userName} Not Found");

            return UserModelToInfoVM(user);
        }


        public async Task<bool> ChangePassword(UserLoginVM userLoginVM, string requestingUsername)
        {
            try
            {
                UserModel reqUser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = reqUser.Role == "admin";
                if (!isAdmin || reqUser.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }

                UserModel user;

                if (userLoginVM.UserName!.Length > 0)
                    user = await _Context.Users.SingleAsync(user => user.Username == userLoginVM.UserName);

                else
                    user = await _Context.Users.SingleAsync(user => user.Email == userLoginVM.Email);

                user.Password = userLoginVM.Password;
                await _Context.SaveChangesAsync();

                return true;
            }

            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }

            catch (InvalidOperationException)
            {
                throw new NotFoundException($"User Not Found");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserLoginVM> ResetPassword(UserLoginVM userLoginVM, string requestingUsername)
        {
            try
            {
                UserModel requser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = requser.Role == "admin";
                if (!isAdmin || requser.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }

                UserModel user;

                if (userLoginVM.UserName!.Length > 0)
                    user = await _Context.Users.SingleAsync(user => user.Username == userLoginVM.UserName);

                else
                    user = await _Context.Users.SingleAsync(user => user.Email == userLoginVM.Email);

                user.Password = _pexitaTools.GenerateRandomPassword(32);

                await _Context.SaveChangesAsync();
                return new UserLoginVM { Password = user.Password, Email = userLoginVM.Email, UserName = userLoginVM.UserName };
            }

            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }

            catch (InvalidOperationException)
            {
                throw new NotFoundException();
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserInfoVM> UpdateUser(UserUpdateVM userUpdateVM, string requestingUsername)
        {
            try
            {
                UserModel reqUser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = reqUser.Role == "admin";
                if (!isAdmin || reqUser.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }

                UserModel User = await _Context.Users.SingleAsync(u => u.ID == userUpdateVM.ID);

                _mapper.Map(userUpdateVM, User);

                await _Context.SaveChangesAsync();

                return UserModelToInfoVM(User);
            }

            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }

            catch (InvalidOperationException)
            {
                throw new NotFoundException($"User With ID:{userUpdateVM.ID} Not Found");
            }
        }

        public async Task<bool> DeleteUser(int UserID, string requestingUsername)
        {

            UserModel reqUser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
            bool isAdmin = reqUser.Role == "admin";
            if (!isAdmin || reqUser.Username != requestingUsername)
            {
                throw new NotAuthorizedException();
            }

            UserModel user = await _Context.Users.FirstOrDefaultAsync(user => user.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");
            _Context.Remove(user);

            return true;
        }

        public UserInfoVM UserModelToInfoVM(UserModel userModel)
        {

            return _mapper.Map<UserInfoVM>(userModel);
        }

        public async Task<string> Login(UserLoginVM userLoginVM)
        {
            UserModel? user = null;

            if (!string.IsNullOrEmpty(userLoginVM.UserName))
                user = await _Context.Users.FirstOrDefaultAsync(u => u.Username == userLoginVM.UserName) ?? throw new NotFoundException();

            else if (string.IsNullOrEmpty(userLoginVM.Email))
                user = await _Context.Users.FirstOrDefaultAsync(u => u.Email == userLoginVM.Email) ?? throw new NotFoundException();

            if (user == null && !BCrypt.Net.BCrypt.Verify(userLoginVM.Password, user?.Password))
            {
                throw new NotAuthorizedException("Username or Password is not correct");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userLoginVM.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Email, userLoginVM.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> Register(UserCreateVM userCreateVM)
        {
            if (await _Context.Users.AnyAsync(u => u.Username == userCreateVM.Username))
                return false;

            UserModel User = _mapper.Map<UserModel>(userCreateVM);

            _Context.Users.Add(User);

            _Context.SaveChanges();

            return true;

        }

        public async Task<List<Address>> GetAddresses(int UserID, string requestingUsername)
        {
            UserModel reqUser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
            bool isAdmin = reqUser.Role == "admin";
            if (!isAdmin || reqUser.Username != requestingUsername)
            {
                throw new NotAuthorizedException();
            }

            UserModel user = await _Context.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");
            return user.Addresses.ToList();
        }

        public async Task<bool> AddAddress(int UserID, Address address, string requestingUsername)
        {
            UserModel reqUser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
            bool isAdmin = reqUser.Role == "admin";
            if (!isAdmin || reqUser.Username != requestingUsername)
            {
                throw new NotAuthorizedException();
            }

            UserModel user = await _Context.Users.Include(u => u.Addresses).FirstOrDefaultAsync(user => user.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

            user.Addresses.Add(address);

            await _Context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAddress(int UserID, Address address, string requestingUsername)
        {
            UserModel reqUser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
            bool isAdmin = reqUser.Role == "admin";
            if (!isAdmin || reqUser.Username != requestingUsername)
            {
                throw new NotAuthorizedException();
            }

            UserModel user = await _Context.Users.Include(_u => _u.Addresses).FirstOrDefaultAsync(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

            // Find the existing address in the user's addresses list
            Address existingAddress = user.Addresses.FirstOrDefault(a => a.ID == address.ID) ?? throw new NotFoundException($"Address With ID:{address.ID} Not Found");

            // Update the properties of the existing address with the values from the 'address' parameter
            existingAddress.Province = address.Province;
            existingAddress.City = address.City;
            existingAddress.Text = address.Text;

            await _Context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAddress(int UserID, int id, string requestingUsername)
        {
            UserModel reqUser = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
            bool isAdmin = reqUser.Role == "admin";
            if (!isAdmin || reqUser.Username != requestingUsername)
            {
                throw new NotAuthorizedException();
            }

            UserModel user = await _Context.Users.Include(_u => _u.Addresses).FirstOrDefaultAsync(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

            _Context.Remove(user.Addresses.FirstOrDefault(a => a.ID == id) ?? throw new NotFoundException($"Address with ID {id} Not found"));

            _Context.SaveChanges();

            return true;
        }

        public async Task<List<CommentsModel>> GetComments(int UserID)
        {

            UserModel user = await _Context.Users.Include(u => u.Comments).FirstOrDefaultAsync(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");
            return user.Comments.ToList();
        }

        public bool IsUser(int id)
        {
            return _Context.Users.FirstOrDefault(u => u.ID == id) != null;
        }

        public bool IsUser(string Username)
        {
            return _Context.Users.FirstOrDefault(u => u.Username == Username) != null;
        }

        public bool IsEmailInUse(string Email)
        {
            return _Context.Users.FirstOrDefault(_u => _u.Email == Email) != null;
        }

        public bool IsAdmin(string userName)
        {
            var user = _Context.Users.FirstOrDefault(u => u.Username == userName) ?? throw new NotFoundException();
            return user.Role == "admin";
        }
    }
}
