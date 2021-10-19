using System;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    [Serializable]
    public class UserJoinedToLobby
    {
        public UserModel user { get; private set; }
        public UserJoinedToLobby(string userJson)
        {
            user = JsonConvert.DeserializeObject<UserModel>(userJson);
        }
    }
}
