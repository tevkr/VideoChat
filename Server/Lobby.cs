using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ServerDLL;

namespace Server
{
    public class Lobby
    {
        public List<User> Users;
        private string _id;
        private string _name;
        private int _capacity;
        private string _password;
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
        public Lobby(string name, int capacity, string password, User owner)
        {
            Users = new List<User>();
            Users.Add(owner);
            Guid myuuid = Guid.NewGuid();
            _id = myuuid.ToString();
            _name = name;
            _capacity = capacity;
            _password = password;
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
    }
}
