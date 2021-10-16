using ServerDLL;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class FirstLoadViewModel : ObservableObject
    {
        public AsyncRelayCommand EnterCommand { get; set; }

        private string _userNameTextBoxText;
        public string UserNameTextBoxText
        {
            get { return _userNameTextBoxText; }
            set
            {
                _userNameTextBoxText = value;
                OnPropertyChanged(nameof(UserNameTextBoxText));
            }
        }
        public FirstLoadViewModel()
        {
            EnterCommand = new AsyncRelayCommand(async (o) => await EnterCommandTask(o));
        }
        private async Task EnterCommandTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.SendTCP(ServerCommand.changeUserNameServerCommand(UserNameTextBoxText));
                ServerResponseConverter serverCommandConverter = new ServerResponseConverter(Server.listenToServerResponse(), 0);
                ServerDLL.ServerResponse.Responses response = serverCommandConverter.ServerResponse.Response;
                if (response == ServerDLL.ServerResponse.Responses.NameChanged)
                {
                    Application.Current.Properties["LocalUserId"] = serverCommandConverter.ServerResponse.user.Id;
                    //MessageBox.Show(Application.Current.Properties["LocalUserId"].ToString());
                    MainViewModel.CurrentView = MainViewModel.mainMenuViewModel;
                }
                else 
                {
                    MessageBox.Show("Невозможно отправить никнейм на сервер.");
                }
            });
            await task;
        }
    }
}
