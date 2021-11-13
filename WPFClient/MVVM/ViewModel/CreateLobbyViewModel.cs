using System;
using System.Threading.Tasks;
using System.Windows;
using SharedLibrary.Data.Models;
using SharedLibrary.Data.Responses;
using DataObject = SharedLibrary.Data.DataObject;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class CreateLobbyViewModel : ObservableObject
    {
        public AsyncRelayCommand createAndConnectCommand { get; set; }
        public static RelayCommand backToMainMenuCommand { get; set; }

        private string _lobbyName;
        public string lobbyName
        {
            get { return _lobbyName; }
            set
            {
                _lobbyName = value;
                OnPropertyChanged(nameof(lobbyName));
            }
        }
        private int _lobbyCapacity;
        public int lobbyCapacity
        {
            get { return _lobbyCapacity; }
            set
            {
                try
                {
                    _lobbyCapacity = value;
                }
                catch
                {
                    try
                    {
                        _lobbyCapacity = Convert.ToInt32(value);
                    }
                    catch
                    {
                        _lobbyCapacity = 0;
                    }
                }
                OnPropertyChanged(nameof(lobbyCapacity));
            }
        }
        private string _lobbyPassword;
        public string lobbyPassword
        {
            get { return _lobbyPassword; }
            set
            {
                _lobbyPassword = value;
                OnPropertyChanged(nameof(lobbyPassword));
            }
        }
        public CreateLobbyViewModel()
        {
            createAndConnectCommand = new AsyncRelayCommand(async (o) => await createAndConnectTask(o));
            backToMainMenuCommand = new RelayCommand(o =>
            {
                MainViewModel.currentView = MainViewModel.mainMenuViewModel;
            });
        }
        private async Task createAndConnectTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.sendTcp(DataObject.createLobbyRequest(_lobbyName, _lobbyCapacity, _lobbyPassword));
                DataObject receivedDataObject = Server.listenToServerTcpResponse();
                if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.lobbyInfoResponse)
                {
                    var lobbyInfoResponse = receivedDataObject.dataObjectInfo as LobbyInfo;
                    var lobbyModel = lobbyInfoResponse.lobby;
                    Server.setUdpPort(lobbyModel.udpPort);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainViewModel.setLobbyView(lobbyModel);
                    });
                    MainViewModel.currentView = MainViewModel.lobbyView;
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
