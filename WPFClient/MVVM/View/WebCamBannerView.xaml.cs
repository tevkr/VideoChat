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
    /// Логика взаимодействия для WebCamBannerView.xaml
    /// </summary>
    public partial class WebCamBannerView : UserControl
    {
        private WebCamBannerViewModel currentViewModel;
        public string Username { get; set; }
        public string UserId { get; set; }
        public WebCamBannerView(string username, string userId)
        {
            InitializeComponent();
            currentViewModel = new WebCamBannerViewModel(username, userId);
            this.DataContext = currentViewModel;
            Username = username;
            UserId = userId;
        }
    }
}
