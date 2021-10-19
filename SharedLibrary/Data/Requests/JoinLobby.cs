using System;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    [Serializable]
    public class JoinLobby
    {
        public LobbyModel lobby { get; }
        public JoinLobby(string lobbyId, string lobbyPassword)
        {
            lobby = new LobbyModel();
            lobby.id = lobbyId;
            lobby.password = lobbyPassword;
        }
    }
}
