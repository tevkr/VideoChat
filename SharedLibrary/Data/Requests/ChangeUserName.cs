using System;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    [Serializable]
    public class ChangeUserName
    {
        public UserModel user { get; }
        public ChangeUserName(string userName)
        {
            user = new UserModel();
            user.userName = userName;
        }
    }
}
