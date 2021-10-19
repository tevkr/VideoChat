using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    class UserJoinedToLobby
    {
        public UserModel User { get; private set; }

        public UserJoinedToLobby(string userJson)
        {
            User = JsonConvert.DeserializeObject<UserModel>(userJson);
        }
    }
}
