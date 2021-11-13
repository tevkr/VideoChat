using System.Threading.Tasks;
using System.Windows;
using SharedLibrary.Data.Models;
using SharedLibrary.Data.Responses;
using DataObject = SharedLibrary.Data.DataObject;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class FirstLoadViewModel : ObservableObject
    {
        public AsyncRelayCommand enterCommand { get; set; }

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
        public FirstLoadViewModel()
        {
            enterCommand = new AsyncRelayCommand(async (o) => await enterCommandTask(o));
        }
        private async Task enterCommandTask(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.sendTcp(DataObject.changeUserNameRequest(_userNameTextBoxText));
                DataObject receivedDataObject = Server.listenToServerTcpResponse();
                if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.nameChangedResponse)
                {
                    var nameChangedResponse = receivedDataObject.dataObjectInfo as NameChanged;
                    var userModel = nameChangedResponse.user;
                    Application.Current.Properties["LocalUserId"] = userModel.id;
                    MainViewModel.currentView = MainViewModel.mainMenuViewModel;
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
