using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;
using Pexita.Utility;

namespace Pexita.Services
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _Context;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;

        public UserService(AppDBContext Context, IPexitaTools PexitaTools, IMapper Mapper)
        {
            _Context = Context;
            _pexitaTools = PexitaTools;
            _mapper = Mapper;
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

        public UserInfoVM GetUserByID(int UserID)
        {
            UserModel user = _Context.Users
                .Include(u => u.Orders).Include(u => u.Addresses)
                .Include(u => u.BrandNewsletters).Include(u => u.ProductNewsletters)
                .AsNoTracking()
                .FirstOrDefault(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

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


        public bool ChangePassword(UserLoginVM userLoginVM)
        {
            try
            {
                UserModel user;

                if (userLoginVM.UserName!.Length > 0)
                    user = _Context.Users.Single(user => user.Username == userLoginVM.UserName);

                else
                    user = _Context.Users.Single(user => user.Email == userLoginVM.Email);

                user.Password = userLoginVM.Password;
                _Context.SaveChanges();

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

        public UserLoginVM ResetPassword(UserLoginVM userLoginVM)
        {
            try
            {
                UserModel user;

                if (userLoginVM.UserName!.Length > 0)
                    user = _Context.Users.Single(user => user.Username == userLoginVM.UserName);

                else
                    user = _Context.Users.Single(user => user.Email == userLoginVM.Email);

                user.Password = _pexitaTools.GenerateRandomPassword(32);

                _Context.SaveChanges();

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

        public UserInfoVM UpdateUser(UserUpdateVM userUpdateVM)
        {
            try
            {

                UserModel User = _Context.Users.Single(u => u.ID == userUpdateVM.ID);

                _mapper.Map(userUpdateVM, User);

                _Context.SaveChanges();

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

        public bool DeleteUser(int UserID)
        {
            UserModel user = _Context.Users.FirstOrDefault(user => user.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");
            _Context.Remove(user);

            return true;
        }

        public UserInfoVM UserModelToInfoVM(UserModel userModel)
        {

            return _mapper.Map<UserInfoVM>(userModel);
        }

        public bool Login(UserLoginVM userLoginVM)
        {
            UserModel user = null;

            if (!string.IsNullOrEmpty(userLoginVM.UserName))
                user = _Context.Users.FirstOrDefault(u => u.Username == userLoginVM.UserName) ?? throw new NotFoundException();

            else if (string.IsNullOrEmpty(userLoginVM.Email))
                user = _Context.Users.FirstOrDefault(u => u.Email == userLoginVM.Email) ?? throw new NotFoundException();

            if (user != null)
            {
                string HashedDBPassword = user.Password;
            }

            throw new NotImplementedException();
        }

        public bool Register(UserCreateVM userCreateVM)
        {
            UserModel User = _mapper.Map<UserModel>(userCreateVM);

            _Context.Users.Add(User);

            _Context.SaveChanges();

            return true;

        }

        public List<Address> GetAddresses(int UserID)
        {
            UserModel user = _Context.Users.Include(u => u.Addresses).FirstOrDefault(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");
            return user.Addresses.ToList();
        }

        public bool AddAddress(int UserID, Address address)
        {
            UserModel user = _Context.Users.Include(u => u.Addresses).FirstOrDefault(user => user.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

            user.Addresses.Add(address);

            _Context.SaveChanges();

            return true;
        }

        public bool UpdateAddress(int UserID, Address address)
        {
            UserModel user = _Context.Users.Include(_u => _u.Addresses).FirstOrDefault(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

            // Find the existing address in the user's addresses list
            Address existingAddress = user.Addresses.FirstOrDefault(a => a.ID == address.ID) ?? throw new NotFoundException($"Address With ID:{address.ID} Not Found");

            // Update the properties of the existing address with the values from the 'address' parameter
            existingAddress.Province = address.Province;
            existingAddress.City = address.City;
            existingAddress.Text = address.Text;

            _Context.SaveChanges();
            return true;
        }


        public bool DeleteAddress(int UserID, int id)
        {
            UserModel user = _Context.Users.Include(_u => _u.Addresses).FirstOrDefault(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");

            _Context.Remove(user.Addresses.FirstOrDefault(a => a.ID == id) ?? throw new NotFoundException($"Address with ID {id} Not found"));

            _Context.SaveChanges();

            return true;
        }

        public List<CommentsModel> GetComments(int UserID)
        {
            UserModel user = _Context.Users.Include(u => u.Comments).FirstOrDefault(u => u.ID == UserID) ?? throw new NotFoundException($"User With ID:{UserID} Not Found");
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

    }
}
