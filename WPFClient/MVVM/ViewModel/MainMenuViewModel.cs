using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class MainMenuViewModel : ObservableObject
    {
        public static RelayCommand createLobbyCommand { get; set; }
        public static RelayCommand findLobbyCommand { get; set; }
        public MainMenuViewModel()
        {
            createLobbyCommand = new RelayCommand(o =>
            {
                MainViewModel.currentView = MainViewModel.createLobbyViewModel;
            });
            findLobbyCommand = new RelayCommand(o =>
            {
                MainViewModel.currentView = MainViewModel.findLobbyViewModel;
            });
        }
    }
}
