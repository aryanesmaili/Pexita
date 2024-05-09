using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitTest.FakeServices
{
    internal class FakeUserService : IUserService
    {
        public bool AddAddress(int UserID, Address address)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(UserLoginVM loginVM)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAddress(int UserID, int id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public List<Address> GetAddresses(int UserID)
        {
            throw new NotImplementedException();
        }

        public List<CommentsModel> GetComments(int UserID)
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

        public bool IsEmailInUse(string Email)
        {
            throw new NotImplementedException();
        }

        public bool IsUser(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsUser(string Username)
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

        public bool UpdateAddress(int UserID, Address address)
        {
            throw new NotImplementedException();
        }

        public UserInfoVM UpdateUser(UserUpdateVM user)
        {
            throw new NotImplementedException();
        }

        public UserInfoVM UserModelToInfoVM(UserModel userModel)
        {
            throw new NotImplementedException();
        }
    }
}
