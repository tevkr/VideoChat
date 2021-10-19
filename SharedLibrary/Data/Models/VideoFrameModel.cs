using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Data.Models
{
    [Serializable]
    class VideoFrameModel
    {
        public string UserId { get; set; }
        public byte[] FrameBytes { get; set; }
    }
}
