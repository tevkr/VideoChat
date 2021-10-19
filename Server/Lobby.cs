using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using SharedLibrary.Data;
using SharedLibrary.Data.UDPData;
using SharedLibrary.SerDes;

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

        private Thread _userUdpThread; // Поток для прослушивания UDP
        private UdpClient _udpClientListener;
        public UdpClient udpClientListener
        {
            get
            {
                return _udpClientListener;
            }
        }
        private UdpClient _udpClientSender;
        
        public Lobby(string name, int capacity, string password, User owner, int udpPort)
        {
            _udpPort = udpPort;
            _udpClientListener = new UdpClient(_udpPort);
            _udpClientSender = new UdpClient();
            _users = new List<User> {owner};
            _id = Guid.NewGuid().ToString();
            _name = name;
            _capacity = capacity;
            _password = password;

            _userUdpThread = new Thread(udpListener);
            _userUdpThread.IsBackground = true;
            _userUdpThread.Start();
        }
        public void removeUser(User user)
        {
            foreach (var u in _users) // Оповещение пользователей об отключении
            {
                u.sendDataObject(DataObject.userLeavedFromLobbyResponse(JsonConvert.SerializeObject(user)));
            }
            _users.RemoveAll(u => u.id == user.id);
            if (_users.Count == 0)
                Server.endLobby(this);
        }
        public void addUser(User user)
        {
            foreach (var u in _users) // Оповещение пользователей о новом подключении
            {
                u.sendDataObject(DataObject.userJoinedToLobbyResponse(JsonConvert.SerializeObject(user)));
            }
            _users.Add(user);
        }
        private void udpListener()
        {
            byte[] buffer = new byte[15000];
            IPEndPoint remoteIpEndPoint = null;
            while (true)
            {
                try
                {
                    buffer = _udpClientListener.Receive(ref remoteIpEndPoint);
                    DataObject dataObject = Deserializer.deserialize(buffer);
                    handleCommand(dataObject);
                }
                catch {  return; }
            }
        }
        private void handleCommand(DataObject dataObject)
        {
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.newVideoFrame)
            {
                NewVideoFrame newVideoFrame = dataObject.dataObjectInfo as NewVideoFrame;
                sendDataObject(dataObject, newVideoFrame.videoFrame.userId);
            }
        }
        private void sendDataObject(DataObject dataObject, string userId)
        {
            try
            {
                foreach (var user in this._users)
                {
                    if (user.id != userId)
                    {
                        var bytes = Serializer.serialize(dataObject);
                        var remoteIpEndPoint = (IPEndPoint)user.handler.RemoteEndPoint;
                        remoteIpEndPoint.Port = _udpPort;
                        _udpClientSender.SendAsync(bytes, bytes.Length, remoteIpEndPoint);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
    }
}
