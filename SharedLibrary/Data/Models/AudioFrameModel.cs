using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Data.Models
{
    [Serializable]
    public class AudioFrameModel
    {
        public string userId { get; set; }
        public byte[] frameBytes { get; set; }
    }
}
