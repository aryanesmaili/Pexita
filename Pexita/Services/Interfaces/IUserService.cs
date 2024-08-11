using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserInfoVM> Register(UserCreateVM user);
        public Task<UserInfoVM> Login(UserLoginVM user);
        public List<UserInfoVM> GetUsers();
        public List<UserInfoVM> GetUsers(int Count);
        public Task<UserInfoVM> GetUserByID(int id);
        public UserInfoVM GetUserByUserName(string userName);
        public Task<UserInfoVM> UpdateUser(UserUpdateVM user, string requestingUsername);
        public Task DeleteUser(int id, string requestingUsername);
        public Task<UserInfoVM> ResetPassword(string loginInfo);
        public Task<UserInfoVM> ChangePassword(UserInfoVM user, string newPassword, string ConfirmPassword, string requestingUsername);
        public Task<UserInfoVM> CheckResetCode(UserInfoVM user, string Code);
        public Task<UserInfoVM> TokenRefresher(string token);
        public Task RevokeToken(string token);
        public Task<List<Address>> GetAddresses(int UserID, string requestingUsername);
        public Task AddAddress(int UserID, Address address, string requestingUsername);
        public Task UpdateAddress(int UserID, Address address, string requestingUsername);
        public Task DeleteAddress(int UserID, int id, string requestingUsername);
        public Task<List<CommentsModel>> GetComments(int UserID);
        public UserInfoVM UserModelToInfoVM(UserModel userModel);
        public UserInfoVM UserModelToInfoVM(UserModel userModel, UserRefreshTokenDTO refreshToken, string AccessToken);
        public bool IsUser(int id);
        public bool IsUser(string Username);
        public bool IsEmailInUse(string Email);
        public bool IsAdmin(string Username);
    }
}
