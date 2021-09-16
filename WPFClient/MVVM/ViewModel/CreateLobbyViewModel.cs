using System;
using System.Threading.Tasks;
using System.Windows;
using ServerDLL;
using WPFClient.Core;
using WPFClient.MVVM.View;

namespace WPFClient.MVVM.ViewModel
{
    class CreateLobbyViewModel : ObservableObject
    {
        public AsyncRelayCommand CreateAndConnectCommand { get; set; }
        public static RelayCommand BackToMainMenuCommand { get; set; }

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
        private int _lobbyMaxCapacity;
        public int LobbyMaxCapacity
        {
            get { return _lobbyMaxCapacity; }
            set
            {
                try
                {
                    _lobbyMaxCapacity = value;
                }
                catch
                {
                    try
                    {
                        _lobbyMaxCapacity = Convert.ToInt32(value);
                    }
                    catch { }
                }
                OnPropertyChanged(nameof(LobbyMaxCapacity));
            }
        }
        private string _lobbyPassword;
        public string LobbyPassword
        {
            get { return _lobbyPassword; }
            set
            {
                _lobbyPassword = value;
                OnPropertyChanged(nameof(LobbyPassword));
            }
        }
        public CreateLobbyViewModel()
        {
            CreateAndConnectCommand = new AsyncRelayCommand(async (o) => await CreateAndConnectTask(o));
            BackToMainMenuCommand = new RelayCommand(o =>
            {
                MainViewModel.CurrentView = MainViewModel.mainMenuViewModel;
            });
        }
        private async Task CreateAndConnectTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.SendTCP(ServerCommand.createLobbyCommand(LobbyName, LobbyMaxCapacity, LobbyPassword));
                ServerResponseConverter serverCommandConverter = new ServerResponseConverter(Server.listenToServerResponse(), 0);
                ServerDLL.ServerResponse serverResponse = serverCommandConverter.ServerResponse;
                if (serverResponse.Response == ServerDLL.ServerResponse.Responses.LobbyInfo)
                {
                    ServerDLL.ServerResponse.Lobby lobby = serverResponse.lobby;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainViewModel.setLobbyView(lobby);
                    });
                    MainViewModel.CurrentView = MainViewModel.lobbyView;
                    /*MessageBox.Show("Lobby name: " + lobby.Name + "\nLobby capacity: " + lobby.Capacity + "\nLobby password: " + lobby.Password + 
                                    "\nUsers count: " + lobby.UsersCount + "\nFirst user id: " + lobby.Users[0].Id + "\nFirst user name: " + lobby.Users[0].UserName + "\nLobby id " + lobby.Id);*/
                }
                else
                {
                    MessageBox.Show("Невозможно создать лобби.");
                }
            });
            await task;
        }
    }
}
