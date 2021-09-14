using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class FindLobbyViewModel : ObservableObject
    {
        public static RelayCommand BackToMainMenuCommand { get; set; }

        public FindLobbyViewModel()
        {
            BackToMainMenuCommand = new RelayCommand(o =>
            {
                MainViewModel.CurrentView = MainViewModel.mainMenuViewModel;
            });
        }
    }
}
