using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    class JoinLobby
    {
        public LobbyModel Lobby { get; private set; }
        public JoinLobby(string lobbyId, string lobbyPassword)
        {
            Lobby = new LobbyModel();
            Lobby.Id = lobbyId;
            Lobby.Password = lobbyPassword;
        }
    }
}
