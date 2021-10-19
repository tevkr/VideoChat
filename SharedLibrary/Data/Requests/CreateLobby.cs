using System;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Requests
{
    [Serializable]
    public class CreateLobby
    {
        public LobbyModel lobby { get; }
        public CreateLobby(string lobbyName, int lobbyCapacity, string lobbyPassword)
        {
            lobby = new LobbyModel();
            lobby.name = lobbyName;
            lobby.capacity = lobbyCapacity;
            lobby.password = lobbyPassword;
        }
    }
}
