using System.Windows.Controls;
using SharedLibrary.Data.Models;
using WPFClient.MVVM.ViewModel;

namespace WPFClient.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для LobbyView.xaml
    /// </summary>
    public partial class LobbyView : UserControl
    {
        private LobbyViewModel currentViewModel;
        public LobbyView(LobbyModel lobbyModel)
        {
            InitializeComponent();
            currentViewModel = new LobbyViewModel(lobbyModel);
            this.DataContext = currentViewModel;
        }

    }
}
