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
        public bool ChangePassword(UserLoginVM loginVM);
        public List<Address> GetAddresses(int UserID);
        public bool AddAddress(int UserID, Address address);
        public bool UpdateAddress(int UserID, Address address);
        public bool DeleteAddress(int UserID, int id);
        public List<CommentsModel> GetComments(int UserID);
        public UserInfoVM UserModelToInfoVM(UserModel userModel);
        public bool IsUser(int id);
        public bool IsUser(string Username);
        public bool IsEmailInUse(string Email);
        public bool IsAddressAlready(string text);
    }
}
