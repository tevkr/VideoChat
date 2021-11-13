using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using SharedLibrary.Data.Models;
using SharedLibrary.Data.Responses;
using DataObject = SharedLibrary.Data.DataObject;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class FindLobbyViewModel : ObservableObject
    {
        private static void convertLobbiesToListViewLobbies(List<LobbyModel> l1,
            ObservableCollection<LobbyForListView> l2)
        {
            uiContext.Send(x => l2.Clear(), null);
            
            for (int i = 0; i < l1.Count; i++)
            {
                uiContext.Send(x => l2.Add(new LobbyForListView(l1[i].id, l1[i].name, l1[i].password, l1[i].users.Count, l1[i].capacity)), null);
            }
        }
        public class LobbyForListView
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
            public int CurrentUsers { get; set; }
            public int Capacity { get; set; }
            public LobbyForListView() { }
            public LobbyForListView(string id, string name, string password, int currentUsers, int capacity)
            {
                Id = id;
                Name = name;
                Password = password;
                CurrentUsers = currentUsers;
                Capacity = capacity;
            }
        }

        private ObservableCollection<LobbyForListView> _lobbiesForListView;
        public ObservableCollection<LobbyForListView> LobbiesForListView
        {
            get { return _lobbiesForListView; }
            set
            {
                _lobbiesForListView = value;
                OnPropertyChanged(nameof(LobbiesForListView));
            }
        }
        public AsyncRelayCommand WindowLoaded { get; set; }
        private List<LobbyModel> _lobbies;

        public List<LobbyModel> Lobbies
        {
            get { return _lobbies; }
            set
            {
                _lobbies = value;
                OnPropertyChanged(nameof(Lobbies));
            }
        }
        public static RelayCommand BackToMainMenuCommand { get; set; }
        private static SynchronizationContext uiContext;
        public FindLobbyViewModel()
        {
            WindowLoaded = new AsyncRelayCommand(async (o) => await getLobbies(o));
            LobbiesForListView = new ObservableCollection<LobbyForListView>();
            uiContext = SynchronizationContext.Current;
            BackToMainMenuCommand = new RelayCommand(o =>
            {
                MainViewModel.currentView = MainViewModel.mainMenuViewModel;
            });
        }
        private async Task getLobbies(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                
                Server.sendTcp(DataObject.getLobbiesRequest());
                DataObject receivedDataObject = Server.listenToServerTcpResponse();
                if (receivedDataObject.dataObjectType == DataObject.DataObjectTypes.lobbiesInfoResponse)
                {
                    var lobbiesInfoResponse = receivedDataObject.dataObjectInfo as LobbiesInfo;
                    Lobbies = lobbiesInfoResponse.lobbies;
                    convertLobbiesToListViewLobbies(Lobbies, LobbiesForListView);
                }
            });
            await task;
        }
    }
}
