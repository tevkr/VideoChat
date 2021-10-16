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
        private string _id;
        private string _userName; // Имя пользователя
        private Socket _handler; // Сокет для прослушивания пользователя
        private Thread _userThread; // Поток для прослушивания
        private Lobby currentLobby;
        public User(Socket socket)
        {
            currentLobby = null;
            Guid myuuid = Guid.NewGuid();
            _id = myuuid.ToString();
            _handler = socket;
            _userThread = new Thread(listner);
            _userThread.IsBackground = true;
            _userThread.Start();
        }

        public Socket getHandler()
        {
            return _handler;
        }
        public string Id
        {
            get { return _id; }
        }
        public string UserName
        {
            get { return _userName; }
        }
        private void listner()
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
                    Server.EndUser(this); 
                    return; 
                }
            }
        }
       
        public void End()
        {
            try
            {
                _handler.Close();
                try
                {
                    _userThread.Abort();
                }
                catch { }
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
                currentLobby = Server.NewLobby(serverCommand.LobbyName, serverCommand.LobbyCapacity, 
                    serverCommand.LobbyPassword, this);
                if (currentLobby == null)
                {
                    SendResponseToUser(ServerDLL.ServerResponse.Responses.Error);
                    return;
                }
                SendResponseToUser(ServerResponse.LobbyInfoResponse(JsonConvert.SerializeObject(currentLobby)));
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
                Lobby temp = Server.lobbies.Find(l => serverCommand.LobbyId == l.Id);
                if (temp != null && temp.UsersCount != temp.Capacity)
                {
                    if (!String.IsNullOrEmpty(temp.Password))
                    {
                        if (serverCommand.LobbyPassword == temp.Password)
                        {
                            currentLobby = temp;
                            currentLobby.addUser(this);
                            SendResponseToUser(ServerResponse.LobbyInfoResponse(JsonConvert.SerializeObject(currentLobby)));
                        }
                        else
                        {
                            SendResponseToUser(ServerDLL.ServerResponse.Responses.Error);
                        }
                    }
                    else
                    {
                        currentLobby = temp;
                        currentLobby.addUser(this);
                        SendResponseToUser(ServerResponse.LobbyInfoResponse(JsonConvert.SerializeObject(currentLobby)));
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
                Server.EndUser(this);
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
                Server.EndUser(this);
            }
        }

        private void currentLobbyRemove()
        {
            Lobby temp = currentLobby;
            currentLobby = null;
            if (temp != null)
                temp.removeUser(this);
        }
    }
}
