using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;
using ServerDLL;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class WebCamBannerViewModel : ObservableObject
    {
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }
        private Bitmap _videoFrameBitmap;
        public Bitmap VideoFrameBitmap
        {
            get { return _videoFrameBitmap; }
            set
            {
                _videoFrameBitmap = value;
                VideoFrame = BitmapToImageSource(value);
            }
        }
        private BitmapImage _videoFrame;
        public BitmapImage VideoFrame
        {
            get { return _videoFrame; }
            set
            {
                _videoFrame = value;
                OnPropertyChanged(nameof(VideoFrame));
            }
        }
        private VideoCaptureDevice _videoSource;
        public VideoCaptureDevice VideoSource
        {
            get { return _videoSource; }
            set
            {
                _videoSource = value;
                _videoSource.NewFrame += VideoSource_NewFrame;
                _videoSource.Start();
                OnPropertyChanged(nameof(VideoSource));
            }
        }
        private void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bmp = new Bitmap(eventArgs.Frame, 250, 250);
            Application.Current.Dispatcher.Invoke(() =>
            {
                VideoFrameBitmap = bmp;
            });
            Server.SendUDP(ServerCommand.newFrameCommand(bmp));
        }

        public WebCamBannerViewModel(string username, string userId)
        {
            UserName = username;
            UserId = userId;
            waitForNewFrames();
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
        private async Task waitForNewFrames()
        {
            var task = Task.Factory.StartNew(() =>
            {

            });
            await task;
        }
    }
}
