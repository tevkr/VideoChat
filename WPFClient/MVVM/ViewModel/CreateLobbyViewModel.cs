using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class CreateLobbyViewModel : ObservableObject
    {
        public AsyncRelayCommand CreateAndConnectCommand { get; set; }

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

        }
    }
}
