using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServerDLL
{
    [Serializable]
    public class ServerResponse
    {
        private ServerResponse() { }
        public enum Responses
        {
            Success,
            Error,
            LobbyInfo
        }
        private Responses _response;
        public Responses Response
        {
            get { return _response; }
            set { _response = value; }
        }
        public static ServerResponse CreateServerResponse(Responses response) // Success or Error
        {
            ServerResponse result = new ServerResponse();
            result.Response = response;
            return result;
        }
        public static ServerResponse SuccessResponse() // Success
        {
            ServerResponse result = new ServerResponse();
            result.Response = Responses.Success;
            return result;
        }
        public static ServerResponse ErrorResponse() // Error
        {
            ServerResponse result = new ServerResponse();
            result.Response = Responses.Error;
            return result;
        }
        public static ServerResponse LobbyInfoResponse(string jsonLobby) // LobbyInfo
        {
            ServerResponse result = new ServerResponse();
            result.Response = Responses.LobbyInfo;
            result.lobby = JsonConvert.DeserializeObject<Lobby>(jsonLobby);
            return result;
        }
        private Lobby _lobby;
        public Lobby lobby
        {
            get { return _lobby; }
            set { _lobby = value; }
        }
        [Serializable]
        public class User
        {
            public string Id { get; set; }
            public string UserName { get; set; }
        }
        [Serializable]
        public class Lobby
        {
            public List<User> Users { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public int Capacity { get; set; }
            public string Password { get; set; }
            public int UsersCount { get; set; }
        }
    }
}
