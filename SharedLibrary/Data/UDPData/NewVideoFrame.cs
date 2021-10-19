using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.UDPData
{
    [Serializable]
    public class NewVideoFrame
    {
        public VideoFrameModel videoFrame { get; private set; }
        public NewVideoFrame(Bitmap newFrame, string userId)
        {
            videoFrame = new VideoFrameModel();
            videoFrame.userId = userId;
            try
            {
                using (var ms = new MemoryStream())
                {
                    newFrame.Save(ms, ImageFormat.Jpeg);
                    videoFrame.frameBytes = ms.ToArray();
                }
            }
            catch
            {
                videoFrame.frameBytes = null;
            }
        }
        public NewVideoFrame(byte[] newFrame, string userId)
        {
            videoFrame = new VideoFrameModel();
            videoFrame.userId = userId;
            videoFrame.frameBytes = newFrame;
        }
    }
}
