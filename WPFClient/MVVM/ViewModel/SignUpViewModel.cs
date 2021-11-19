using System.Threading.Tasks;
using System.Windows;
using SharedLibrary.Data.Responses;
using DataObject = SharedLibrary.Data.DataObject;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class SignUpViewModel : ObservableObject
    {
        public AsyncRelayCommand signUpCommand { get; set; }
        public static RelayCommand switchToLoginInCommand { get; set; }

        private string _userNameTextBoxText;
        public string userNameTextBoxText
        {
            get { return _userNameTextBoxText; }
            set
            {
                _userNameTextBoxText = value;
                OnPropertyChanged(nameof(userNameTextBoxText));
            }
        }
        private string _passwordTextBoxText;
        public string passwordTextBoxText
        {
            get { return _passwordTextBoxText; }
            set
            {
                _passwordTextBoxText = value;
                OnPropertyChanged(nameof(passwordTextBoxText));
            }
        }
        public SignUpViewModel()
        {
            userNameTextBoxText = "";
            passwordTextBoxText = "";
            signUpCommand = new AsyncRelayCommand(async (o) => await signUpCommandTask(o));
            switchToLoginInCommand = new RelayCommand(o =>
            {
                MainViewModel.currentView = MainViewModel.firstLoadViewModel;
            });
        }
        private async Task signUpCommandTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.sendTcp(DataObject.signUpRequest(_userNameTextBoxText, _passwordTextBoxText));
                DataObject receivedDataObject = Server.listenToServerTcpResponse();
                if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.userInfoResponse)
                {
                    var userInfoResponse = receivedDataObject.dataObjectInfo as UserInfo;
                    var userModel = userInfoResponse.user;
                    Application.Current.Properties["LocalUserId"] = userModel.id;
                    MainViewModel.currentView = MainViewModel.mainMenuViewModel;
                }
                else if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.errorResponse)
                {
                    var errorMessage = receivedDataObject.dataObjectInfo as string;
                    MessageBox.Show($"ServerSide error: {errorMessage}");
                }
                else
                {
                    MessageBox.Show("Невозможно отправить данные на сервер.");
                }
            });
            await task;
        }
    }
}
