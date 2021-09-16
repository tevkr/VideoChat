using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFClient.MVVM.ViewModel;

namespace WPFClient.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для LobbyView.xaml
    /// </summary>
    public partial class LobbyView : UserControl
    {
        private LobbyViewModel currentViewModel;
        public LobbyView(ServerDLL.ServerResponse.Lobby lobby)
        {
            InitializeComponent();
            currentViewModel = new LobbyViewModel(lobby);
            this.DataContext = currentViewModel;
        }
    }
}
