using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Data.Models
{
    [Serializable]
    class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
