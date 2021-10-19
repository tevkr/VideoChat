using System;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    [Serializable]
    public class UserLeavedFromLobby
    {
        public UserModel user { get; private set; }
        public UserLeavedFromLobby(string userJson)
        {
            user = JsonConvert.DeserializeObject<UserModel>(userJson);
        }
    }
}
