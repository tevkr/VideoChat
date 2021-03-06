using System.Threading.Tasks;
using System.Windows;
using SharedLibrary.Data.Responses;
using DataObject = SharedLibrary.Data.DataObject;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class FirstLoadViewModel : ObservableObject
    {
        public AsyncRelayCommand loginInCommand { get; set; }
        public static RelayCommand switchToSignUpCommand { get; set; }

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
        public FirstLoadViewModel()
        {
            userNameTextBoxText = "";
            passwordTextBoxText = "";
            loginInCommand = new AsyncRelayCommand(async (o) => await loginInCommandTask(o));
            switchToSignUpCommand = new RelayCommand(o =>
            {
                MainViewModel.currentView = MainViewModel.signUpViewModel;
            });
        }
        private async Task loginInCommandTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.sendTcp(DataObject.loginInRequest(_userNameTextBoxText, _passwordTextBoxText));
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
