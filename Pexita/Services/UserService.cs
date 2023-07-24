using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;

namespace Pexita.Services
{
    public class UserService : IUserService
    {
        public bool AddAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public UserLoginVM ChangePassword(UserLoginVM loginVM)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAddress(int id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public List<Address> GetAddresses(int id)
        {
            throw new NotImplementedException();
        }

        public List<CommentsModel> GetComments(int id)
        {
            throw new NotImplementedException();
        }

        public UserInfoVM GetUserByID(int id)
        {
            throw new NotImplementedException();
        }

        public UserInfoVM GetUserByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public List<UserInfoVM> GetUsers()
        {
            throw new NotImplementedException();
        }

        public List<UserInfoVM> GetUsers(int Count)
        {
            throw new NotImplementedException();
        }

        public bool Login(UserLoginVM user)
        {
            throw new NotImplementedException();
        }

        public bool Register(UserCreateVM user)
        {
            throw new NotImplementedException();
        }

        public UserLoginVM ResetPassword(UserLoginVM loginVM)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public UserInfoVM UpdateUser(UserUpdateVM user)
        {
            throw new NotImplementedException();
        }
    }
}
