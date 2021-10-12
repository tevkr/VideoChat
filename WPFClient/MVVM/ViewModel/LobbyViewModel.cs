using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;
using ServerDLL;
using WPFClient.Core;
using WPFClient.MVVM.View;

namespace WPFClient.MVVM.ViewModel
{
    class LobbyViewModel : ObservableObject
    {
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
                OnPropertyChanged(nameof(LobbyUserStats));
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

        public LobbyViewModel(ServerDLL.ServerResponse.Lobby CurrentLobby)
        {
            _webCamViews = new ObservableCollection<WebCamBannerView>();
            BackToMainMenuCommand = new AsyncRelayCommand(async (o) => await BackToMainMenuTask(o));
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _currentUserLeaved = false;
            if (CurrentLobby != null)
            {
                LobbyId = CurrentLobby.Id;
                LobbyName = CurrentLobby.Name;
                LobbyUserStats = $"{CurrentLobby.UsersCount}/{CurrentLobby.Capacity}";
                _lobbyCapacity = CurrentLobby.Capacity;
                _usersCount = CurrentLobby.UsersCount;
                for (int i = 0; i < CurrentLobby.UsersCount; i++)
                {
                    WebCamViews.Add(new WebCamBannerView(CurrentLobby.Users[i].UserName, CurrentLobby.Users[i].Id));
                    if (CurrentLobby.Users[i].Id == Application.Current.Properties["LocalUserId"].ToString())
                        _localWebCamView = WebCamViews.Last();
                }
                if (VideoDevices.Count >= 1)
                    SelectedFilterInfo = VideoDevices[0];
                SwitchWebCam = true;
                waitForLobbyChanges();
            }
        }


        private bool _currentUserLeaved;
        private async Task BackToMainMenuTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                ((WebCamBannerViewModel)_localWebCamView.DataContext).VideoSource.SignalToStop();
                Server.SendTCP(ServerCommand.leaveLobbyCommand());
                MainViewModel.CurrentView = MainViewModel.mainMenuViewModel;
                _currentUserLeaved = true;
            });
            await task;
        }
        private async Task waitForLobbyChanges()
        {
            var task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    ServerResponseConverter serverCommandConverter = new ServerResponseConverter(Server.listenToServerResponse(), 0);
                    ServerDLL.ServerResponse.Responses response = serverCommandConverter.ServerResponse.Response;
                    if (response == ServerDLL.ServerResponse.Responses.UserJoined)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _usersCount++;
                            LobbyUserStats = $"{_usersCount}/{_lobbyCapacity}";
                            WebCamViews.Add(new WebCamBannerView(serverCommandConverter.ServerResponse.user.UserName,
                                serverCommandConverter.ServerResponse.user.Id));
                        });
                    }
                    else if (response == ServerDLL.ServerResponse.Responses.UserLeaved)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _usersCount--;
                            LobbyUserStats = $"{_usersCount}/{_lobbyCapacity}";
                            foreach (var webCamView in WebCamViews.ToArray())
                            {
                                if (((WebCamBannerViewModel)webCamView.DataContext).UserId ==
                                    serverCommandConverter.ServerResponse.user.Id)
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
    }
}
