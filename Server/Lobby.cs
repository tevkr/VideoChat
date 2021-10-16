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
        public List<User> Users;
        private int _UDPPort;
        private string _id;
        private string _name;
        private int _capacity;
        private string _password;

        private Thread _userUPDThread; // Поток для прослушивания UPD
        private UdpClient _udpClient;
        private UdpClient udpClientToSend;
        public int UDPPort
        {
            get { return _UDPPort; }
        }

        public UdpClient getUdpClient()
        {
            return _udpClient;
        }
        public string Id
        {
            get { return _id; }
        }
        public string Name
        {
            get { return _name; }
        }
        public int Capacity
        {
            get { return _capacity; }
        }
        public string Password
        {
            get { return _password; }
        }
        public int UsersCount
        {
            get { return Users.Count; }
        }
        public Lobby(string name, int capacity, string password, User owner, int UDPPort)
        {
            _UDPPort = UDPPort;
            Users = new List<User>();
            Users.Add(owner);
            Guid myuuid = Guid.NewGuid();
            _id = myuuid.ToString();
            _name = name;
            _capacity = capacity;
            _password = password;

            _userUPDThread = new Thread(UDPlistener);
            _userUPDThread.IsBackground = true;
            _userUPDThread.Start();
        }
        public void removeUser(User user)
        {
            foreach (var u in Users) // Оповещение пользователей об отключении
            {
                u.SendResponseToUser(ServerResponse.UserLeavedResponse(JsonConvert.SerializeObject(user)));
            }
            Users.RemoveAll(u => u.Id == user.Id);
            if (Users.Count == 0)
                Server.EndLobby(this);
        }
        public void addUser(User user)
        {
            foreach (var u in Users) // Оповещение пользователей о новом подключении
            {
                u.SendResponseToUser(ServerResponse.UserJoinedResponse(JsonConvert.SerializeObject(user)));
            }
            Users.Add(user);
        }
        private async void UDPlistener()
        {
            _udpClient = new UdpClient(this.UDPPort);
            byte[] buffer = new byte[15000];
            IPEndPoint remoteIpEndPoint = null;
            while (true)
            {
                try
                {
                    buffer = _udpClient.Receive(ref remoteIpEndPoint);
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
                SendFrameToLobbyUsers(serverCommand.FrameBytes, serverCommand.UserId);
            }
        }
        public void SendFrameToLobbyUsers(byte[] frameBytes, string userId)
        {
            try
            {
                udpClientToSend = new UdpClient();
                foreach (var user in this.Users)
                {
                    if (user.Id != userId)
                    {
                        ServerResponse.Frame frame = new ServerResponse.Frame();
                        frame.UserId = userId;
                        frame.FrameBytes = frameBytes;
                        var bytes = ServerDLL.Serializer.Serialize(ServerResponse.NewFrameResponse(frame));
                        var remoteIpEndPoint = (IPEndPoint)user.getHandler().RemoteEndPoint;
                        remoteIpEndPoint.Port = this.UDPPort;
                        udpClientToSend.SendAsync(bytes, bytes.Length, remoteIpEndPoint);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString());}
        }
    }
}
