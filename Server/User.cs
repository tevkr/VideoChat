using ServerDLL;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace Server
{
    public class User
    {
        private string _id; // Идентификатор пользователя
        public string id
        {
            get { return _id; }
        }
        private string _userName; // Имя пользователя
        public string userName
        {
            get { return _userName; }
        }
        private Socket _handler; // Сокет для прослушивания пользователя
        [JsonIgnore]
        public Socket handler
        {
            get { return _handler; }
        }
        private Thread _userThread; // Поток для прослушивания
        private Lobby _currentLobby; // Лобби, в котором находится пользователь
        public User(Socket socket)
        {
            _currentLobby = null;
            _id = Guid.NewGuid().ToString();
            _handler = socket;
            _userThread = new Thread(tcpListener);
            _userThread.IsBackground = true;
            _userThread.Start();
        }
        
        private void tcpListener()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[8000];
                    int bytesRec = _handler.Receive(buffer);
                    ServerCommandConverter serverCommandConverter = new ServerCommandConverter(buffer, bytesRec);
                    ServerCommand serverCommand = serverCommandConverter.ServerCommand;
                    handleCommand(serverCommand);
                }
                catch
                {
                    currentLobbyRemove();
                    Server.endUser(this); 
                    return; 
                }
            }
        }
       
        public void end()
        {
            try
            {
                _handler.Close();
                _userThread.Abort();
            }
            catch (Exception e) { Console.WriteLine("Error with end: {0}.", e.Message); }
        }
        private void handleCommand(ServerCommand serverCommand)
        {
            if (serverCommand.Command == ServerCommand.Commands.ChangeUserName)
            {
                if (String.IsNullOrEmpty(serverCommand.UserName))
                {
                    SendResponseToUser(ServerDLL.ServerResponse.Responses.Error);
                    return;
                }
                _userName = serverCommand.UserName;
                SendResponseToUser(ServerResponse.NameChangedResponse(JsonConvert.SerializeObject(this)));
                return;
            }
            if (serverCommand.Command == ServerCommand.Commands.CreateLobby)
            {
                if (serverCommand.LobbyCapacity < 2  || serverCommand.LobbyCapacity > 8 || String.IsNullOrEmpty(serverCommand.LobbyName))
                {
                    SendResponseToUser(ServerDLL.ServerResponse.Responses.Error);
                    return;
                }
                _currentLobby = Server.newLobby(serverCommand.LobbyName, serverCommand.LobbyCapacity, 
                    serverCommand.LobbyPassword, this);
                if (_currentLobby == null)
                {
                    SendResponseToUser(ServerDLL.ServerResponse.Responses.Error);
                    return;
                }
                SendResponseToUser(ServerResponse.LobbyInfoResponse(JsonConvert.SerializeObject(_currentLobby)));
                return;
            }
            if (serverCommand.Command == ServerCommand.Commands.LeaveLobby)
            {
                currentLobbyRemove();
                return;
            }
            if (serverCommand.Command == ServerCommand.Commands.GetLobbies)
            {
                SendResponseToUser(ServerDLL.ServerResponse.LobbiesResponse(JsonConvert.SerializeObject(Server.lobbies)));
                return;
            }
            if (serverCommand.Command == ServerCommand.Commands.JoinLobby)
            {
                Lobby temp = Server.lobbies.Find(l => serverCommand.LobbyId == l.id);
                if (temp != null && temp.users.Count != temp.capacity)
                {
                    if (!String.IsNullOrEmpty(temp.password))
                    {
                        if (serverCommand.LobbyPassword == temp.password)
                        {
                            _currentLobby = temp;
                            _currentLobby.addUser(this);
                            SendResponseToUser(ServerResponse.LobbyInfoResponse(JsonConvert.SerializeObject(_currentLobby)));
                        }
                        else
                        {
                            SendResponseToUser(ServerDLL.ServerResponse.Responses.Error);
                        }
                    }
                    else
                    {
                        _currentLobby = temp;
                        _currentLobby.addUser(this);
                        SendResponseToUser(ServerResponse.LobbyInfoResponse(JsonConvert.SerializeObject(_currentLobby)));
                    }
                }
                else
                {
                    SendResponseToUser(ServerDLL.ServerResponse.Responses.Error);
                }
            }
        }
        public void SendResponseToUser(ServerDLL.ServerResponse.Responses response) // Success or Error
        {
            try
            {
                int bytesSent =
                    _handler.Send(ServerDLL.Serializer.Serialize(ServerResponse.CreateServerResponse(response)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with send command: {0}.", e.Message);
                currentLobbyRemove();
                Server.endUser(this);
            }
        }
        public void SendResponseToUser(ServerDLL.ServerResponse response) // CreateLobby
        {
            try
            {
                int bytesSent =
                    _handler.Send(ServerDLL.Serializer.Serialize(response));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with send command: {0}.", e.Message);
                currentLobbyRemove();
                Server.endUser(this);
            }
        }

        private void currentLobbyRemove()
        {
            Lobby temp = _currentLobby;
            _currentLobby = null;
            if (temp != null)
                temp.removeUser(this);
        }
    }
}
