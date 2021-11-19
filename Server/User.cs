using System;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Server.Database;
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
            _id = "unknown";
            _userName = "guest";
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
            catch { }
        }
        private void handleCommand(DataObject dataObject)
        {
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.signUpRequest)
            {
                SignUp signUp = dataObject.dataObjectInfo as SignUp;
                if (String.IsNullOrEmpty(signUp.user.userName))
                {
                    sendDataObject(DataObject.errorResponse("Empty username."));
                    return;
                }
                if (String.IsNullOrEmpty(signUp.user.password))
                {
                    sendDataObject(DataObject.errorResponse("Empty password."));
                    return;
                }
                if (signUp.user.password.Length <= 2)
                {
                    sendDataObject(DataObject.errorResponse("Short password."));
                    return;
                }
                if (PostgreService.userExists(signUp.user.userName))
                {
                    sendDataObject(DataObject.errorResponse("Username is already taken."));
                    return;
                }
                _id = Guid.NewGuid().ToString();
                _userName = signUp.user.userName;
                PostgreService.addUser(_id, _userName, signUp.user.password);
                sendDataObject(DataObject.userInfoResponse(JsonConvert.SerializeObject(this)));
                return;
            }
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.loginInRequest)
            {
                LoginIn loginIn = dataObject.dataObjectInfo as LoginIn;
                if (!PostgreService.userExists(loginIn.user.userName))
                {
                    sendDataObject(DataObject.errorResponse("There is no user with given username."));
                    return;
                }
                if (!PostgreService.correctCredentials(loginIn.user.userName, loginIn.user.password))
                {
                    sendDataObject(DataObject.errorResponse("Incorrect credentials."));
                    return;
                }
                var user = PostgreService.getUserIdAndUsername(loginIn.user.userName);
                _id = user.Item1;
                _userName = user.Item2;
                sendDataObject(DataObject.userInfoResponse(JsonConvert.SerializeObject(this)));
                return;
            }
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.createLobbyRequest)
            {
                CreateLobby createLobby = dataObject.dataObjectInfo as CreateLobby;
                if (createLobby.lobby.capacity < 2  || createLobby.lobby.capacity > 8 || String.IsNullOrEmpty(createLobby.lobby.name))
                {
                    sendDataObject(DataObject.errorResponse("Problem with capacity or lobby name."));
                    return;
                }
                _currentLobby = Server.newLobby(createLobby.lobby.name, createLobby.lobby.capacity,
                    createLobby.lobby.password, this);
                if (_currentLobby == null)
                {
                    sendDataObject(DataObject.errorResponse("Too many lobbies have been created."));
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
            if (dataObject.dataObjectType == DataObject.DataObjectTypes.getLobbiesRequest)
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
                            sendDataObject(DataObject.errorResponse("Wrong password."));
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
                    sendDataObject(DataObject.errorResponse("Unable to connect to the lobby."));
                }
            }
        }
        public void sendDataObject(DataObject dataObject)
        {
            try
            {
                _handler.Send(Serializer.serialize(dataObject));
            }
            catch
            {
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
