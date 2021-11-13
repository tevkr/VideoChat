using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;
using DataObject = SharedLibrary.Data.DataObject;
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
            Server.sendUdp(DataObject.newVideoFrame(bmp, Application.Current.Properties["LocalUserId"].ToString()));
        }

        public WebCamBannerViewModel(string username, string userId)
        {
            UserName = username;
            UserId = userId;
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
    }
}
