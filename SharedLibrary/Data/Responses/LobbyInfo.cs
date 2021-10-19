using System;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    [Serializable]
    public class LobbyInfo
    {
        public LobbyModel lobby { get; private set; }
        public LobbyInfo(string lobbyJson)
        {
            lobby = JsonConvert.DeserializeObject<LobbyModel>(lobbyJson);
        }
    }
}
