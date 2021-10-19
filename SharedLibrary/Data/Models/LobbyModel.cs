using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Data.Models
{
    [Serializable]
    class LobbyModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Capacity { get; set; }
        public List<UserModel> Users { get; set; }
        public int UDPPort { get; set; }
    }
}
