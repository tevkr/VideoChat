using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class WebCamBannerViewModel : ObservableObject
    {
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        public WebCamBannerViewModel(string username)
        {
            UserName = username;
        }
    }
}
