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
        private string _userId;

        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }
        public WebCamBannerViewModel(string username, string userId)
        {
            UserName = username;
            UserId = userId;
        }
    }
}
