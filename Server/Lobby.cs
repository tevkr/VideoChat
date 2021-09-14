using System;
using System.Collections.Generic;

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
            Users.RemoveAll(u => u.Id == user.Id);
            if (Users.Count == 0)
                Server.EndLobby(this);
        }
    }
}
