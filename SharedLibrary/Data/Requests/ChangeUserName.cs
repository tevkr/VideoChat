using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    class ChangeUserName
    {
        public UserModel User { get; private set; }

        public ChangeUserName(string userName)
        {
            User = new UserModel();
            User.UserName = userName;
        }
    }
}
