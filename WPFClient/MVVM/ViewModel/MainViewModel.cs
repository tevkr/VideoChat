using System;
using SharedLibrary.Data.Models;
using WPFClient.Core;
using WPFClient.MVVM.View;

namespace WPFClient.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public static FirstLoadViewModel firstLoadViewModel { get; set; }
        public static MainMenuViewModel mainMenuViewModel { get; set; }
        public static CreateLobbyViewModel createLobbyViewModel { get; set; }
        public static FindLobbyViewModel findLobbyViewModel { get; set; }
        public static SignUpViewModel signUpViewModel { get; set; }
        public static LobbyView lobbyView { get; set; }
        public static void setLobbyView(LobbyModel lobbyModel)
        {
            lobbyView = new LobbyView(lobbyModel);
        }

        public static event EventHandler currentViewChanged;
        private static object _currentView;
        public static object currentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                currentViewChanged?.Invoke(null, EventArgs.Empty);
            }
        }
        public MainViewModel()
        {
            firstLoadViewModel = new FirstLoadViewModel();
            mainMenuViewModel = new MainMenuViewModel();
            createLobbyViewModel = new CreateLobbyViewModel();
            findLobbyViewModel = new FindLobbyViewModel();
            signUpViewModel = new SignUpViewModel();
            lobbyView = null;
            currentView = firstLoadViewModel;
        }
    }
}
