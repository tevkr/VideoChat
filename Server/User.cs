using ServerDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Server
{
    public class User
    {
        private string _id;
        private string _userName; // Имя пользователя
        private Socket _handler; // Сокет для прослушивания пользователя
        private Thread _userThread; // Поток для прослушивания
        public User(Socket socket)
        {
            Guid myuuid = Guid.NewGuid();
            _id = myuuid.ToString();
            _handler = socket;
            _userThread = new Thread(listner);
            _userThread.IsBackground = true;
            _userThread.Start();
        }
        public string Id
        {
            get { return _id; }
        }
        public string UserName
        {
            get { return _userName; }
        }
        private void listner()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[8000];
                    int bytesRec = _handler.Receive(buffer);
                    ServerCommandConverter serverCommandConverter = new ServerCommandConverter(buffer, bytesRec);
                    ServerCommand serverCommand = serverCommandConverter.ServerCommand;
                    handleCommand(serverCommand);
                }
                catch 
                {
                    Server.EndUser(this); return; 
                }
            }
        }
        public void End()
        {
            try
            {
                _handler.Close();
                try
                {
                    _userThread.Abort();
                }
                catch { }
            }
            catch (Exception e) { Console.WriteLine("Error with end: {0}.", e.Message); }
        }
        private void handleCommand(ServerCommand serverCommand)
        {
            if(serverCommand.Command == ServerCommand.Commands.ChangeUserName)
            {
                _userName = serverCommand.UserName;
                SendResponseToUser(ServerDLL.ServerResponse.Responses.Success);
                return;
            }
        }
        public void SendResponseToUser(ServerDLL.ServerResponse.Responses response)
        {
            try
            {
                Thread.Sleep(10000);
                int bytesSent = _handler.Send(ServerDLL.Serializer.Serialize(ServerResponse.CreateServerResponse(response)));
            }
            catch (Exception e) { Console.WriteLine("Error with send command: {0}.", e.Message); Server.EndUser(this); }
        }
    }
}
