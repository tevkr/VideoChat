using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ServerDLL;
using WPFClient.Core;
using WPFClient.MVVM.View;

namespace WPFClient.MVVM.ViewModel
{
    class LobbyViewModel : ObservableObject
    {
        public static AsyncRelayCommand BackToMainMenuCommand { get; set; }

        static public event EventHandler WebCamViewsChanged;
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
        private int _lobbyUsers;
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

        public LobbyViewModel(ServerDLL.ServerResponse.Lobby CurrentLobby)
        {
            _webCamViews = new ObservableCollection<WebCamBannerView>();
            BackToMainMenuCommand = new AsyncRelayCommand(async (o) => await BackToMainMenuTask(o));
            _currentUserLeaved = false;
            if (CurrentLobby != null)
            {
                LobbyId = CurrentLobby.Id;
                LobbyName = CurrentLobby.Name;
                LobbyUserStats = $"{CurrentLobby.UsersCount}/{CurrentLobby.Capacity}";
                _lobbyCapacity = CurrentLobby.Capacity;
                _lobbyUsers = CurrentLobby.UsersCount;
                for (int i = 0; i < CurrentLobby.UsersCount; i++)
                {
                    WebCamViews.Add(new WebCamBannerView(CurrentLobby.Users[i].UserName, CurrentLobby.Users[i].Id));
                }
                waitForLobbyChanges();
            }
        }

        private bool _currentUserLeaved;
        private async Task BackToMainMenuTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
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
                            _lobbyUsers++;
                            LobbyUserStats = $"{_lobbyUsers}/{_lobbyCapacity}";
                            WebCamViews.Add(new WebCamBannerView(serverCommandConverter.ServerResponse.user.UserName,
                                serverCommandConverter.ServerResponse.user.Id));
                        });
                    }
                    else if (response == ServerDLL.ServerResponse.Responses.UserLeaved)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _lobbyUsers--;
                            LobbyUserStats = $"{_lobbyUsers}/{_lobbyCapacity}";
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
                    if (_lobbyUsers <= 0 || _currentUserLeaved)
                        break;
                }
            });
            await task;
        }
    }
}
