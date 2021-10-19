using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    class LobbyInfo
    {
        public LobbyModel Lobby { get; private set; }
        public LobbyInfo(string lobbyJson)
        {
            Lobby = JsonConvert.DeserializeObject<LobbyModel>(lobbyJson);
        }
    }
}
