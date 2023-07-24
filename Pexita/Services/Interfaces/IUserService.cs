using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IUserService
    {
        public bool Register(UserCreateVM user);
        public bool Login(UserLoginVM user);
        public List<UserInfoVM> GetUsers();
        public List<UserInfoVM> GetUsers(int Count);
        public UserInfoVM GetUserByID(int id);
        public UserInfoVM GetUserByUserName(string userName);
        public UserInfoVM UpdateUser(UserUpdateVM user);
        public bool DeleteUser(int id);
        public UserLoginVM ResetPassword(UserLoginVM loginVM);
        public UserLoginVM ChangePassword(UserLoginVM loginVM);
        public List<Address> GetAddresses(int id);
        public bool AddAddress(Address address);
        public bool UpdateAddress(Address address);
        public bool DeleteAddress(int id);
        public List<CommentsModel> GetComments(int id);
    }
}
