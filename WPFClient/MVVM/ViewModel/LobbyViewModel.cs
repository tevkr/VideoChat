using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AForge.Video.DirectShow;
using SharedLibrary.Data.Models;
using SharedLibrary.Data.Responses;
using SharedLibrary.Data.UDPData;
using SharedLibrary.SerDes;
using WPFClient.Core;
using DataObject = SharedLibrary.Data.DataObject;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using WPFClient.MVVM.View;

namespace WPFClient.MVVM.ViewModel
{
    class LobbyViewModel : ObservableObject
    {
        //поток для нашей речи
        WaveIn input;
        //поток для речи собеседника
        WaveOut output;
        //буфферный поток для передачи через сеть
        BufferedWaveProvider bufferStream;
        WaveFormat commonFormat = new WaveFormat(16000, 16, 1);
        private UdpClient client;
        public static AsyncRelayCommand BackToMainMenuCommand { get; set; }

        static public event EventHandler WebCamViewsChanged;
        private static WebCamBannerView _localWebCamView;
        private static ObservableCollection<WebCamBannerView> _webCamViews;
        public static ObservableCollection<WebCamBannerView> WebCamViews
        {
            get { return _webCamViews; }
            set
            {
                _webCamViews = value;
                WebCamViewsChanged?.Invoke(null, EventArgs.Empty);
            }
        }
        private int _usersCount;
        private int _lobbyCapacity;
        private string _lobbyId;
        public string LobbyId
        {
            get { return _lobbyId; }
            set
            {
                _lobbyId = value;
                OnPropertyChanged(nameof(LobbyId));
            }
        }
        private string _lobbyName;
        public string LobbyName
        {
            get { return _lobbyName; }
            set
            {
                _lobbyName = value;
                OnPropertyChanged(nameof(LobbyName));
            }
        }
        private string _lobbyUserStats;
        public string LobbyUserStats
        {
            get { return _lobbyUserStats; }
            set
            {
                _lobbyUserStats = value;
                OnPropertyChanged(nameof(LobbyUserStats));
            }
        }

        private bool _switchRecordingDevice;
        public bool SwitchRecordingDevice
        {
            get { return _switchRecordingDevice; }
            set
            {
                if (value)
                {
                    input.StartRecording();
                }
                else
                {
                    input.StopRecording();
                }
                _switchRecordingDevice = value;
                OnPropertyChanged(nameof(SwitchRecordingDevice));
            }
        }

        private List<string> getWaveInDevices()
        {
            List<string> result = new List<string>();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All))
                {
                    if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                    {
                        result.Add(device.FriendlyName);
                        break;
                    }
                }
            }
            return result;
        }

        private List<string> _recordingDevices;
        public List<string> RecordingDevices
        {
            get { return _recordingDevices; }
            set
            {
                _recordingDevices = value;
                OnPropertyChanged(nameof(RecordingDevices));
            }
        }

        private int _selectedRecordingDeviceIndex;
        public int SelectedRecordingDeviceIndex
        {
            get { return _selectedRecordingDeviceIndex; }
            set
            {
                if (input != null)
                    input.Dispose();
                input = new WaveIn();
                input.DeviceNumber = value;
                input.WaveFormat = commonFormat;
                input.DataAvailable += VoiceInput;
                SwitchRecordingDevice = _switchRecordingDevice;
                _selectedRecordingDeviceIndex = value;
                OnPropertyChanged(nameof(SelectedRecordingDeviceIndex));
            }
        }
        private async void VoiceInput(object sender, WaveInEventArgs e)
        {
            try
            {
                Server.sendUdp(DataObject.newAudioFrame(e.Buffer, Application.Current.Properties["LocalUserId"].ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool _switchCaptureDevice;
        public bool SwitchCaptureDevice
        {
            get { return _switchCaptureDevice; }
            set
            {
                if (value)
                {
                    bufferStream = new BufferedWaveProvider(commonFormat);
                    output.Init(bufferStream);
                    output.Play();
                }
                else
                {
                    output.Pause();
                }
                _switchCaptureDevice = value;
                OnPropertyChanged(nameof(SwitchCaptureDevice));
            }
        }

        private List<string> getWaveOutDevices()
        {
            List<string> result = new List<string>();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            int waveOutDevices = WaveOut.DeviceCount;
            for (int waveOutDevice = 0; waveOutDevice < waveOutDevices; waveOutDevice++)
            {
                WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveOutDevice);
                foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All))
                {
                    if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                    {
                        result.Add(device.FriendlyName);
                        break;
                    }
                }
            }
            return result;
        }
        private List<string> _captureDevices;
        public List<string> CaptureDevices
        {
            get { return _captureDevices; }
            set
            {
                _captureDevices = value;
                OnPropertyChanged(nameof(CaptureDevices));
            }
        }

        private int _selectedCaptureDeviceIndex;
        public int SelectedCaptureDeviceIndex
        {
            get { return _selectedCaptureDeviceIndex; }
            set
            {
                if (output != null)
                {
                    output.Stop();
                    output.Dispose();
                }
                output = new WaveOut();
                output.DeviceNumber = value;
                bufferStream = new BufferedWaveProvider(commonFormat);
                output.Init(bufferStream);
                SwitchCaptureDevice = _switchCaptureDevice;
                _selectedCaptureDeviceIndex = value;
                OnPropertyChanged(nameof(SelectedCaptureDeviceIndex));
            }
        }

        private bool _switchWebCam;
        public bool SwitchWebCam
        {
            get { return _switchWebCam; }
            set
            {
                _switchWebCam = value;
                if (_switchWebCam)
                    ((WebCamBannerViewModel)_localWebCamView.DataContext).VideoSource.Start();
                else
                    ((WebCamBannerViewModel)_localWebCamView.DataContext).VideoSource.SignalToStop();
                OnPropertyChanged(nameof(SwitchWebCam));
            }
        }

        
        private FilterInfo _selectedFilterInfo;
        public FilterInfo SelectedFilterInfo
        {
            get { return _selectedFilterInfo; }
            set
            {
                _selectedFilterInfo = value;
                ((WebCamBannerViewModel)_localWebCamView.DataContext).VideoSource =
                    new VideoCaptureDevice(_selectedFilterInfo.MonikerString);
                OnPropertyChanged(nameof(SelectedFilterInfo));
            }
        }

        private FilterInfoCollection _videoDevices;
        public FilterInfoCollection VideoDevices
        {
            get { return _videoDevices; }
            set
            {
                _videoDevices = value;
                OnPropertyChanged(nameof(VideoDevices));
            }
        }

        public LobbyViewModel(LobbyModel lobbyModel)
        {
            _webCamViews = new ObservableCollection<WebCamBannerView>();
            BackToMainMenuCommand = new AsyncRelayCommand(async (o) => await BackToMainMenuTask(o));
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _currentUserLeaved = false;
            if (lobbyModel != null)
            {
                LobbyId = lobbyModel.id;
                LobbyName = lobbyModel.name;
                LobbyUserStats = $"{lobbyModel.users.Count}/{lobbyModel.capacity}";
                _lobbyCapacity = lobbyModel.capacity;
                _usersCount = lobbyModel.users.Count;
                for (int i = 0; i < lobbyModel.users.Count; i++)
                {
                    WebCamViews.Add(new WebCamBannerView(lobbyModel.users[i].userName, lobbyModel.users[i].id));
                    if (lobbyModel.users[i].id == Application.Current.Properties["LocalUserId"].ToString())
                        _localWebCamView = WebCamViews.Last();
                }
                if (VideoDevices.Count >= 1)
                    SelectedFilterInfo = VideoDevices[0];
                SwitchWebCam = true;


                bufferStream = new BufferedWaveProvider(commonFormat);
                //create enumerator
                var enumerator = new MMDeviceEnumerator();
                //cycle through all audio devices
                RecordingDevices = getWaveInDevices();
                SelectedRecordingDeviceIndex = 0;
                CaptureDevices = getWaveOutDevices();
                SelectedCaptureDeviceIndex = 0;
                //clean up
                enumerator.Dispose();
                SwitchRecordingDevice = true;
                SwitchCaptureDevice = true;


                waitForLobbyChanges();
                Thread th = new Thread(waitForNewFrames);
                th.IsBackground = true;
                th.Start();
            }
        }


        private bool _currentUserLeaved;
        private async Task BackToMainMenuTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ((WebCamBannerViewModel)_localWebCamView.DataContext).VideoSource.SignalToStop();
                    
                });
                _currentUserLeaved = true;
                Server.sendTcp(DataObject.leaveLobbyRequest());
                Server.clearUdpPort();
                if (output != null)
                {
                    output.Stop();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        output.Dispose();
                    });
                    output = null;
                }
                if (input != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        input.Dispose();
                    });
                    input = null;
                }
                bufferStream = null;
                client.Dispose();
                MainViewModel.currentView = MainViewModel.mainMenuViewModel;
            });
            await task;
        }
        private async Task waitForLobbyChanges()
        {
            var task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    DataObject receivedDataObject = Server.listenToServerTcpResponse();
                    if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.userJoinedToLobbyResponse)
                    {
                        var userJoinedToLobbyResponse = receivedDataObject.dataObjectInfo as UserJoinedToLobby;
                        var userModel = userJoinedToLobbyResponse.user;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _usersCount++;
                            LobbyUserStats = $"{_usersCount}/{_lobbyCapacity}";
                            WebCamViews.Add(new WebCamBannerView(userModel.userName, userModel.id));
                        });
                    }
                    else if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.userLeavedFromLobbyResponse)
                    {
                        var userLeavedFromLobbyResponse = receivedDataObject.dataObjectInfo as UserLeavedFromLobby;
                        var userModel = userLeavedFromLobbyResponse.user;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _usersCount--;
                            LobbyUserStats = $"{_usersCount}/{_lobbyCapacity}";
                            foreach (var webCamView in WebCamViews.ToArray())
                            {
                                if (((WebCamBannerViewModel)webCamView.DataContext).UserId == userModel.id)
                                {
                                    WebCamViews.Remove(webCamView);
                                }
                            }
                        });
                    }
                    if (_usersCount <= 0 || _currentUserLeaved)
                        break;
                }
            });
            await task;
        }
        private async void waitForNewFrames()
        {
            client = new UdpClient(Server.getUdpPort());
            byte[] buffer = new byte[15000];
            IPEndPoint remoteIpEndPoint = null;
            while (true)
            {
                try
                {
                    buffer = client.Receive(ref remoteIpEndPoint);
                    DataObject receivedDataObject = Deserializer.deserialize(buffer);
                    if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.newVideoFrame)
                    {
                        var newVideoFrame = receivedDataObject.dataObjectInfo as NewVideoFrame;
                        var videoFrame = newVideoFrame.videoFrame;
                        if (videoFrame.userId == Application.Current.Properties["LocalUserId"].ToString()) continue; // TODO REMOVE THIS LINE
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (var webCamView in WebCamViews)
                            {
                                if (((WebCamBannerViewModel)webCamView.DataContext).UserId == videoFrame.userId)
                                {
                                    using (var ms = new System.IO.MemoryStream(videoFrame.frameBytes))
                                    {
                                        var bmp = new Bitmap(ms);
                                        ((WebCamBannerViewModel)webCamView.DataContext).VideoFrameBitmap = bmp;
                                    }
                                }
                            }
                        });
                    }
                    if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.newAudioFrame)
                    {
                        var newAudioFrame = receivedDataObject.dataObjectInfo as NewAudioFrame;
                        var audioFrame = newAudioFrame.audioFrame;
                        bufferStream.AddSamples(audioFrame.frameBytes, 0, audioFrame.frameBytes.Length);
                    }
                    if (_usersCount <= 0 || _currentUserLeaved)
                    {
                        try { client.Close(); } catch { }
                        break;
                    }
                }
                catch { }
            }
        }
    }
}
