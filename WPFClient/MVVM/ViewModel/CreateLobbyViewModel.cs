using System;
using System.Threading.Tasks;
using System.Windows;
using ServerDLL;
using WPFClient.Core;

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
                    MessageBox.Show("Lobby name: {0}" + lobby.Name + "\nLobby capacity: {1}" + lobby.Capacity + "\nLobby password: {2}" + lobby.Password + 
                                    "\nUsers count: {3}" + lobby.UsersCount + "\nFirst user id: {4}" + lobby.Users[0].Id + "\nFirst user name:{5}" + lobby.Users[0].UserName + "\nLobby id {6}" + lobby.Id);
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
