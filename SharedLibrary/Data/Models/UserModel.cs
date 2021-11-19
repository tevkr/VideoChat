using System;

namespace SharedLibrary.Data.Models
{
    [Serializable]
    public class UserModel
    {
        public string id { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }
}
