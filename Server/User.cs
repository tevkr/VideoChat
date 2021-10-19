using System;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using SharedLibrary.Data;
using SharedLibrary.Data.Requests;
using SharedLibrary.SerDes;

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
                    _handler.Receive(buffer);
                    DataObject dataObject = Deserializer.deserialize(buffer);
                    handleCommand(dataObject);
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
        private void handleCommand(DataObject dataObject)
        {
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.changeUserNameRequest)
            {
                ChangeUserName changeUserName = dataObject.dataObjectInfo as ChangeUserName;
                if (String.IsNullOrEmpty(changeUserName.user.userName))
                {
                    sendDataObject(DataObject.errorResponse());
                    return;
                }
                _userName = changeUserName.user.userName;
                sendDataObject(DataObject.nameChangedResponse(JsonConvert.SerializeObject(this)));
                return;
            }
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.createLobbyRequest)
            {
                CreateLobby createLobby = dataObject.dataObjectInfo as CreateLobby;
                if (createLobby.lobby.capacity < 2  || createLobby.lobby.capacity > 8 || String.IsNullOrEmpty(createLobby.lobby.name))
                {
                    sendDataObject(DataObject.errorResponse());
                    return;
                }
                _currentLobby = Server.newLobby(createLobby.lobby.name, createLobby.lobby.capacity,
                    createLobby.lobby.password, this);
                if (_currentLobby == null)
                {
                    sendDataObject(DataObject.errorResponse());
                    return;
                }
                sendDataObject(DataObject.lobbyInfoResponse(JsonConvert.SerializeObject(_currentLobby)));
                return;
            }
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.leaveLobbyRequest)
            {
                currentLobbyRemove();
                return;
            }
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.lobbiesInfoResponse)
            {
                sendDataObject(DataObject.lobbiesInfoResponse(JsonConvert.SerializeObject(Server.lobbies)));
                return;
            }
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.joinLobbyRequest)
            {
                JoinLobby joinLobby = dataObject.dataObjectInfo as JoinLobby;
                Lobby temp = Server.lobbies.Find(l => joinLobby.lobby.id == l.id);
                if (temp != null && temp.users.Count != temp.capacity)
                {
                    if (!String.IsNullOrEmpty(temp.password))
                    {
                        if (joinLobby.lobby.password == temp.password)
                        {
                            _currentLobby = temp;
                            _currentLobby.addUser(this);
                            sendDataObject(DataObject.lobbyInfoResponse(JsonConvert.SerializeObject(_currentLobby)));
                        }
                        else
                        {
                            sendDataObject(DataObject.errorResponse());
                        }
                    }
                    else
                    {
                        _currentLobby = temp;
                        _currentLobby.addUser(this);
                        sendDataObject(DataObject.lobbyInfoResponse(JsonConvert.SerializeObject(_currentLobby)));
                    }
                }
                else
                {
                    sendDataObject(DataObject.errorResponse());
                }
            }
        }
        public void sendDataObject(DataObject dataObject)
        {
            try
            {
                _handler.Send(Serializer.serialize(dataObject));
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
