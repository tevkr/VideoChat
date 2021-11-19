using System;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    [Serializable]
    public class UserInfo
    {
        public UserModel user { get; private set; }
        public UserInfo(string userJson)
        {
            user = JsonConvert.DeserializeObject<UserModel>(userJson);
        }
    }
}
