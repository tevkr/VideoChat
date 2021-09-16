using ServerDLL;
using System;
using System.Linq;
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
                    if (currentLobby != null)
                        currentLobby.removeUser(this);
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
                _userName = serverCommand.UserName;
                SendResponseToUser(ServerDLL.ServerResponse.Responses.Success);
                return;
            }
            if (serverCommand.Command == ServerCommand.Commands.CreateLobby)
            {
                
                currentLobby = Server.NewLobby(serverCommand.LobbyName, serverCommand.LobbyCapacity, 
                    serverCommand.LobbyPassword, this);
                SendResponseToUser(ServerResponse.LobbyInfoResponse(JsonConvert.SerializeObject(currentLobby)));
                return;
            }
            if (serverCommand.Command == ServerCommand.Commands.LeaveLobby)
            {
                currentLobby.removeUser(this);
                return;
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
                if (currentLobby != null)
                    currentLobby.removeUser(this);
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
                if (currentLobby != null)
                    currentLobby.removeUser(this);
                Server.EndUser(this);
            }
        }
    }
}
