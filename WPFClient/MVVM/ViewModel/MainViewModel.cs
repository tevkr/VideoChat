using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public FirstLoadViewModel firstLoadViewModel { get; set; }
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            firstLoadViewModel = new FirstLoadViewModel();
            CurrentView = firstLoadViewModel;
        }
    }
}
