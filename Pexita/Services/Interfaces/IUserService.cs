using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserInfoDTO> Register(UserCreateDTO user);
        public Task<UserInfoDTO> Login(LoginDTO user);
        public List<UserInfoDTO> GetUsers();
        public List<UserInfoDTO> GetUsers(int Count);
        public Task<UserInfoDTO> GetUserByID(int id);
        public UserInfoDTO GetUserByUserName(string userName);
        public Task<UserInfoDTO> UpdateUser(UserUpdateDTO user, string requestingUsername);
        public Task DeleteUser(int id, string requestingUsername);
        public Task<UserInfoDTO> ResetPassword(string loginInfo);
        public Task<UserInfoDTO> ChangePassword(UserInfoDTO user, string newPassword, string ConfirmPassword, string requestingUsername);
        public Task<UserInfoDTO> CheckResetCode(UserInfoDTO user, string Code);
        public Task<UserInfoDTO> TokenRefresher(string token);
        public Task RevokeToken(string token);
        public Task<List<AddressDTO>> GetAddresses(int UserID, string requestingUsername);
        public Task AddAddress(int UserID, Address address, string requestingUsername);
        public Task UpdateAddress(int UserID, Address address, string requestingUsername);
        public Task DeleteAddress(int UserID, int id, string requestingUsername);
        public Task<List<CommentsDTO>> GetComments(int UserID);
        public UserInfoDTO UserModelToInfoDTO(UserModel userModel);
        public UserInfoDTO UserModelToInfoDTO(UserModel userModel, UserRefreshTokenDTO refreshToken, string AccessToken);
        public bool IsUser(int id);
        public bool IsUser(string Username);
        public bool IsEmailInUse(string Email);
        public bool IsAdmin(string Username);
    }
}
