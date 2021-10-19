using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Server
{
    public static class Server
    {
        private static SortedDictionary<int, bool> _availableUDPPorts; // Доступные UDP порты
        private static List<User> _users; // Список подключенных пользователей
        public static List<User> users
        {
            get
            {
                return _users;
            }
        }
        private static List<Lobby> _lobbies; // Список созданных лобби
        public static List<Lobby> lobbies
        {
            get
            {
                return _lobbies;
            }
        }

        static Server()
        {
            _availableUDPPorts = new SortedDictionary<int, bool>();
            _users = new List<User>();
            _lobbies = new List<Lobby>();
            for (int i = 0, port = 9934; i < 100; i++, port++) // Заполнение availableUDPPorts портами [9934; 10034]
                _availableUDPPorts.Add(port, true);
        }

        private static int findAvailablePort()
        {
            foreach (var availableUDPPortsElement in _availableUDPPorts)
            {
                if (availableUDPPortsElement.Value == true)
                    return availableUDPPortsElement.Key;
            }
            return -1;
        }
        private static void setUDPPortAvailability(int port, bool availability)
        {
            _availableUDPPorts[port] = availability;
        }

        public static void newUser(Socket handler)
        {
            try
            {
                User newUser = new User(handler);
                _users.Add(newUser);
                Console.WriteLine("New user connected: {0}", handler.RemoteEndPoint);
            }
            catch (Exception e) { Console.WriteLine("Error with addNewUser: {0}.", e.Message); }
        }
        public static void endUser(User user)
        {
            try
            {
                if (!_users.Contains(user)) return;
                user.end();
                _users.Remove(user);
                Console.WriteLine("User {0} has been disconnected.", user.userName);
            }
            catch (Exception e) { Console.WriteLine("Error with endClient: {0}.", e.Message); }
        }

        public static Lobby newLobby(string lobbyName, int lobbyCapacity, string lobbyPassword, User owner)
        {
            if (lobbies.Count >= 100) return null;

            var availablePort = findAvailablePort();
            if (availablePort == -1) return null;

            Lobby lobby = new Lobby(lobbyName, lobbyCapacity, lobbyPassword, owner, availablePort);
            lobbies.Add(lobby);
            setUDPPortAvailability(availablePort, false);
            Console.WriteLine($"New lobby [{lobbies.Last().id}]:{lobbies.Last().name} was created.");
            return lobby;
        }
        public static void endLobby(Lobby lobby)
        {
            var lobbyToRemove = lobbies.Find(l => l.id == lobby.id);
            if (lobbyToRemove == null) return;

            Console.WriteLine($"Lobby [{lobbyToRemove.id}]:{lobbyToRemove.name} was deleted.");
            setUDPPortAvailability(lobbyToRemove.udpPort, true);
            lobbyToRemove.udpClientListener.Close();
            lobbies.RemoveAll(l => l.id == lobby.id);
        }
    }
}
