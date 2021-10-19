using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    class CreateLobby
    {
        public LobbyModel Lobby { get; private set; }
        public CreateLobby(string lobbyName, int lobbyCapacity, string lobbyPassword)
        {
            Lobby = new LobbyModel();
            Lobby.Name = lobbyName;
            Lobby.Capacity = lobbyCapacity;
            Lobby.Password = lobbyPassword;
        }
    }
}
