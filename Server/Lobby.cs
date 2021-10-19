using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using ServerDLL;

namespace Server
{
    public class Lobby
    {
        private List<User> _users;

        public List<User> users
        {
            get { return _users; }
        }
        private int _udpPort;
        public int udpPort
        {
            get { return _udpPort; }
        }
        private string _id;
        public string id
        {
            get { return _id; }
        }
        private string _name;
        public string name
        {
            get { return _name; }
        }
        private int _capacity;
        public int capacity
        {
            get { return _capacity; }
        }
        private string _password;
        public string password
        {
            get { return _password; }
        }

        private Thread _userUPDThread; // Поток для прослушивания UPD
        private UdpClient _udpClientListener;
        public UdpClient udpClientListener
        {
            get
            {
                return _udpClientListener;
            }
        }
        private UdpClient _udpClientSender;
        
        public Lobby(string name, int capacity, string password, User owner, int UDPPort)
        {
            _udpPort = UDPPort;
            _users = new List<User>();
            _users.Add(owner);
            Guid myuuid = Guid.NewGuid();
            _id = myuuid.ToString();
            _name = name;
            _capacity = capacity;
            _password = password;

            _userUPDThread = new Thread(udpListener);
            _userUPDThread.IsBackground = true;
            _userUPDThread.Start();
        }
        public void removeUser(User user)
        {
            foreach (var u in _users) // Оповещение пользователей об отключении
            {
                u.SendResponseToUser(ServerResponse.UserLeavedResponse(JsonConvert.SerializeObject(user)));
            }
            _users.RemoveAll(u => u.id == user.id);
            if (_users.Count == 0)
                Server.endLobby(this);
        }
        public void addUser(User user)
        {
            foreach (var u in _users) // Оповещение пользователей о новом подключении
            {
                u.SendResponseToUser(ServerResponse.UserJoinedResponse(JsonConvert.SerializeObject(user)));
            }
            _users.Add(user);
        }
        private async void udpListener()
        {
            _udpClientListener = new UdpClient(this.udpPort);
            byte[] buffer = new byte[15000];
            IPEndPoint remoteIpEndPoint = null;
            while (true)
            {
                try
                {
                    buffer = _udpClientListener.Receive(ref remoteIpEndPoint);
                    ServerCommandConverter serverCommandConverter = new ServerCommandConverter(buffer, buffer.Length);
                    ServerCommand serverCommand = serverCommandConverter.ServerCommand;
                    handleCommand(serverCommand);
                }
                catch {  return; }
            }
        }
        private void handleCommand(ServerCommand serverCommand)
        {
            if (serverCommand.Command == ServerCommand.Commands.NewFrame)
            {
                sendFrameToLobbyUsers(serverCommand.FrameBytes, serverCommand.UserId);
            }
        }
        public void sendFrameToLobbyUsers(byte[] frameBytes, string userId)
        {
            try
            {
                _udpClientSender = new UdpClient();
                foreach (var user in this._users)
                {
                    if (user.id != userId)
                    {
                        ServerResponse.Frame frame = new ServerResponse.Frame();
                        frame.UserId = userId;
                        frame.FrameBytes = frameBytes;
                        var bytes = ServerDLL.Serializer.Serialize(ServerResponse.NewFrameResponse(frame));
                        var remoteIpEndPoint = (IPEndPoint)user.handler.RemoteEndPoint;
                        remoteIpEndPoint.Port = this.udpPort;
                        _udpClientSender.SendAsync(bytes, bytes.Length, remoteIpEndPoint);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString());}
        }
    }
}
