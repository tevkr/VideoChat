using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    [Serializable]
    public class LobbiesInfo
    {
        public List<LobbyModel> lobbies { get; private set; }
        public LobbiesInfo(string lobbiesJson)
        {
            lobbies = JsonConvert.DeserializeObject<List<LobbyModel>>(lobbiesJson);
            if (lobbies != null)
                foreach (var lobby in lobbies)
                    lobby.password = !String.IsNullOrEmpty(lobby.password) ? "Yes" : "No";
        }
    }
}
