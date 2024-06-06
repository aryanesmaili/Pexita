using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IUserService
    {
        public Task<bool> Register(UserCreateVM user);
        public Task<string> Login(UserLoginVM user);
        public List<UserInfoVM> GetUsers();
        public List<UserInfoVM> GetUsers(int Count);
        public Task<UserInfoVM> GetUserByID(int id);
        public UserInfoVM GetUserByUserName(string userName);
        public Task<UserInfoVM> UpdateUser(UserUpdateVM user, string requestingUsername);
        public Task<bool> DeleteUser(int id, string requestingUsername);
        public Task<UserLoginVM> ResetPassword(UserLoginVM loginVM, string requestingUsername);
        public Task<bool> ChangePassword(UserLoginVM loginVM, string requestingUsername);
        public Task<List<Address>> GetAddresses(int UserID, string requestingUsername);
        public Task<bool> AddAddress(int UserID, Address address, string requestingUsername);
        public Task<bool> UpdateAddress(int UserID, Address address, string requestingUsername);
        public Task<bool> DeleteAddress(int UserID, int id, string requestingUsername);
        public Task<List<CommentsModel>> GetComments(int UserID);
        public UserInfoVM UserModelToInfoVM(UserModel userModel);
        public Task<bool> IsUser(int id);
        public Task<bool> IsUser(string Username);
        public Task<bool> IsEmailInUse(string Email);
        public Task<bool> IsAdmin(string Username);
    }
}
