using System;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public static FirstLoadViewModel firstLoadViewModel { get; set; }
        public static MainMenuViewModel mainMenuViewModel { get; set; }

        static public event EventHandler CurrentViewChanged;
        private static object _currentView;
        public static object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                CurrentViewChanged?.Invoke(null, EventArgs.Empty);
            }
        }
        public MainViewModel()
        {
            firstLoadViewModel = new FirstLoadViewModel();
            mainMenuViewModel = new MainMenuViewModel();
            CurrentView = firstLoadViewModel;
        }
    }
}
