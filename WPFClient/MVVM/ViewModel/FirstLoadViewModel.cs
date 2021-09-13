using ServerDLL;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
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
                ServerDLL.ServerResponse.Responses response = Server.listenToServerResponse();
                if (response == ServerDLL.ServerResponse.Responses.Success)
                {
                    MessageBox.Show("Success");
                }
                else
                {
                    MessageBox.Show("Error");
                }
            });
            await task;
        }
    }
}
