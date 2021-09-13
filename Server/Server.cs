using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public static class Server
    {
        public static List<User>Users = new List<User>();

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
    }
}
