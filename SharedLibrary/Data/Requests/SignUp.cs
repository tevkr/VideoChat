using System;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    [Serializable]
    public class SignUp
    {
        public UserModel user { get; }
        public SignUp(string userName, string password)
        {
            user = new UserModel();
            user.userName = userName;
            user.password = password;
        }
    }
}
