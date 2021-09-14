using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Server
{
    public static class Server
    {
        public static List<User>Users = new List<User>();
        public static List<Lobby>lobbies = new List<Lobby>();

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
                user.End();
                Users.Remove(user);
                Console.WriteLine("User {0} has been disconnected.", user.UserName);
            }
            catch (Exception e) { Console.WriteLine("Error with endClient: {0}.", e.Message); }
        }
        public static Lobby NewLobby(string lobbyName, int lobbyCapacity, string lobbyPassowrd, User owner)
        {
            Lobby lobby = new Lobby(lobbyName, lobbyCapacity, lobbyPassowrd, owner);
            lobbies.Add(lobby);
            Console.WriteLine($"New lobby [{lobbies.Last().Id}]:{lobbies.Last().Name} was created.");
            return lobby;
        }
        public static void EndLobby(Lobby lobby)
        {
            Console.WriteLine($"Lobby [{lobbies.Last().Id}]:{lobbies.Last().Name} was deleted.");
            lobbies.RemoveAll(l => l.Id == lobby.Id);
        }
    }
}
