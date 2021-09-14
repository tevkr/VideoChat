using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class MainMenuViewModel : ObservableObject
    {
        public static RelayCommand CreateLobbyCommand { get; set; }
        public static RelayCommand FindLobbyCommand { get; set; }
        public MainMenuViewModel()
        {
            CreateLobbyCommand = new RelayCommand(o =>
            {
                MainViewModel.CurrentView = MainViewModel.createLobbyViewModel;
            });
            FindLobbyCommand = new RelayCommand(o =>
            {
                MainViewModel.CurrentView = MainViewModel.findLobbyViewModel;
            });
        }
    }
}
