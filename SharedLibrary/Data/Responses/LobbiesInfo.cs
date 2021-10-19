using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Data.Models;

namespace SharedLibrary.Data.Responses
{
    class LobbiesInfo
    {
        public List<LobbyModel> Lobbies { get; private set; }
        public LobbiesInfo(string lobbiesJson)
        {
            Lobbies = JsonConvert.DeserializeObject<List<LobbyModel>>(lobbiesJson);
            if (Lobbies != null)
                foreach (var lobby in Lobbies)
                    lobby.Password = !String.IsNullOrEmpty(lobby.Password) ? "Yes" : "No";
        }
    }
}
