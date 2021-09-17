using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ServerDLL;
using WPFClient.Core;

namespace WPFClient.MVVM.ViewModel
{
    class FindLobbyViewModel : ObservableObject
    {
        private static void convertLobbiesToListViewLobbies(List<ServerDLL.ServerResponse.Lobby> l1,
            ObservableCollection<LobbyForListView> l2)
        {
            uiContext.Send(x => l2.Clear(), null);
            
            for (int i = 0; i < l1.Count; i++)
            {
                uiContext.Send(x => l2.Add(new LobbyForListView(l1[i].Id, l1[i].Name, l1[i].Password, l1[i].UsersCount, l1[i].Capacity)), null);
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
        private List<ServerDLL.ServerResponse.Lobby> _lobbies;

        public List<ServerDLL.ServerResponse.Lobby> Lobbies
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
                MainViewModel.CurrentView = MainViewModel.mainMenuViewModel;
            });
        }
        private async Task getLobbies(object o)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Server.SendTCP(ServerCommand.getLobbiesCommand());
                ServerResponseConverter serverCommandConverter = new ServerResponseConverter(Server.listenToServerResponse(), 0);
                ServerDLL.ServerResponse.Responses response = serverCommandConverter.ServerResponse.Response;
                if (response == ServerDLL.ServerResponse.Responses.Lobbies)
                {
                    Lobbies = serverCommandConverter.ServerResponse.lobbies;
                    convertLobbiesToListViewLobbies(Lobbies, LobbiesForListView);
                }
            });
            await task;
        }
    }
}
