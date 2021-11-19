using System;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    [Serializable]
    public class LoginIn
    {
        public UserModel user { get; }
        public LoginIn(string userName, string password)
        {
            user = new UserModel();
            user.userName = userName;
            user.password = password;
        }
    }
}
