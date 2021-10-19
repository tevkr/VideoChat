using System;
using System.Collections.Generic;

namespace SharedLibrary.Data.Models
{
    [Serializable]
    public class LobbyModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int capacity { get; set; }
        public List<UserModel> users { get; set; }
        public int udpPort { get; set; }
    }
}
