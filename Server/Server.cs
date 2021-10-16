using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Server
{
    public static class Server
    {
        public static SortedDictionary<int, bool> availableUDPPorts = new SortedDictionary<int, bool>();
        public static List<User>Users = new List<User>();
        public static List<Lobby>lobbies = new List<Lobby>();
        static Server()
        {
            for (int i = 0, port = 9934; i < 100; i++, port++)
                availableUDPPorts.Add(port, true);
        }
        public static void NewUser(Socket handle)
        {
            try
            {
                User newUser = new User(handle);
                Users.Add(newUser);
                Console.WriteLine("New user connected: {0}", handle.RemoteEndPoint);
            }
            catch (Exception e) { Console.WriteLine("Error with addNewUser: {0}.", e.Message); }
        }
        public static void EndUser(User user)
        {
            try
            {
                if (Users.Contains(user))
                {
                    user.End();
                    Users.Remove(user);
                    Console.WriteLine("User {0} has been disconnected.", user.UserName);
                }
            }
            catch (Exception e) { Console.WriteLine("Error with endClient: {0}.", e.Message); }
        }

        private static int findAvailablePort()
        {
            foreach (var availableUDPPortsElement in availableUDPPorts)
            {
                if (availableUDPPortsElement.Value == true)
                    return availableUDPPortsElement.Key;
            }
            return -1;
        }
        private static void setUDPPortAvailability(int port, bool availability)
        {
            availableUDPPorts[port] = availability;
        }

        public static Lobby NewLobby(string lobbyName, int lobbyCapacity, string lobbyPassowrd, User owner)
        {
            if (lobbies.Count <= 100)
            {
                var availablePort = findAvailablePort();
                if (availablePort != -1)
                {
                    Lobby lobby = new Lobby(lobbyName, lobbyCapacity, lobbyPassowrd, owner, availablePort);
                    lobbies.Add(lobby);
                    setUDPPortAvailability(availablePort, false);
                    Console.WriteLine($"New lobby [{lobbies.Last().Id}]:{lobbies.Last().Name} was created.");
                    return lobby;
                }
            }
            return null;
        }
        public static void EndLobby(Lobby lobby)
        {
            var lobbyToRemove = lobbies.Find(l => l.Id == lobby.Id);
            if (lobbyToRemove != null)
            {
                Console.WriteLine($"Lobby [{lobbyToRemove.Id}]:{lobbyToRemove.Name} was deleted.");
                setUDPPortAvailability(lobbyToRemove.UDPPort, true);
                lobbyToRemove.getUdpClient().Close();
                lobbies.RemoveAll(l => l.Id == lobby.Id);
            }
        }
    }
}
