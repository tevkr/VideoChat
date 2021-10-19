using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.UDPData
{
    class NewVideoFrame
    {
        public VideoFrameModel VideoFrame { get; private set; }
        public NewVideoFrame(Bitmap newFrame, string userId)
        {
            VideoFrame = new VideoFrameModel();
            VideoFrame.UserId = userId;
            try
            {
                using (var ms = new MemoryStream())
                {
                    newFrame.Save(ms, ImageFormat.Jpeg);
                    VideoFrame.FrameBytes = ms.ToArray();
                }
            }
            catch
            {
                VideoFrame.FrameBytes = null;
            }
        }
    }
}
