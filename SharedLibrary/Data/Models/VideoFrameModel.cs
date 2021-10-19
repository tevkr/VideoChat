using System;

namespace SharedLibrary.Data.Models
{
    [Serializable]
    public class VideoFrameModel
    {
        public string userId { get; set; }
        public byte[] frameBytes { get; set; }
    }
}
