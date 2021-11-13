using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharedLibrary.Data.Responses;
using DataObject = SharedLibrary.Data.DataObject;
using WPFClient.MVVM.ViewModel;

namespace WPFClient.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для FindLobbyView.xaml
    /// </summary>
    public partial class FindLobbyView : UserControl
    {
        public FindLobbyView()
        {
            InitializeComponent();
        }

        private async void JoinLobbyHandler(object sender, MouseButtonEventArgs e)
        {
            string id = ((FindLobbyViewModel.LobbyForListView)((ListViewItem)sender).Content).Id;
            string password = ((FindLobbyViewModel.LobbyForListView)((ListViewItem)sender).Content).Password;
            if (password == "Yes")
            {
                string typedPassword = Microsoft.VisualBasic.Interaction.InputBox("Введите пароль и нажмите ок для попытки входа в лобби.", "Введите пароль", "",
                    (int)Application.Current.MainWindow.Left + 150, (int)Application.Current.MainWindow.Top + 150);
                joinLobbyTask(id, typedPassword);
            }
            else if (password == "No")
            {
                joinLobbyTask(id, "");
            }
        }
        private async Task joinLobbyTask(string lobbyId, string password)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.sendTcp(DataObject.joinLobbyRequest(lobbyId, password));
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
                    /*MessageBox.Show("Lobby name: " + lobby.Name + "\nLobby capacity: " + lobby.Capacity + "\nLobby password: " + lobby.Password + 
                                    "\nUsers count: " + lobby.UsersCount + "\nFirst user id: " + lobby.Users[0].Id + "\nFirst user name: " + lobby.Users[0].UserName + "\nLobby id " + lobby.Id);*/
                }
                else
                {
                    MessageBox.Show("Невозможно войти в лобби.");
                }
            });
            await task;
        }
    }
}
