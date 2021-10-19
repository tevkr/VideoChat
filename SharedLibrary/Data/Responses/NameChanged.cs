using System;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    [Serializable]
    public class NameChanged
    {
        public UserModel user { get; private set; }
        public NameChanged(string userJson)
        {
            user = JsonConvert.DeserializeObject<UserModel>(userJson);
        }
    }
}
