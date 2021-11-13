using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.UDPData
{
    [Serializable]
    public class NewAudioFrame
    {
        public AudioFrameModel audioFrame { get; private set; }
        public NewAudioFrame(byte[] newFrame, string userId)
        {
            audioFrame = new AudioFrameModel();
            audioFrame.userId = userId;
            audioFrame.frameBytes = newFrame;
        }
    }
}
