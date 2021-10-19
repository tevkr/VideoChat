using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    class NameChanged
    {
        public UserModel User { get; private set; }

        public NameChanged(string userJson)
        {
            User = JsonConvert.DeserializeObject<UserModel>(userJson);
        }
    }
}
