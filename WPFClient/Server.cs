using ServerDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient
{
    public static class Server
    {
        private const string _serverHost = "127.0.0.1";
        private const int _serverPort = 9933;
        private static Socket _serverSocket;
        public static Socket ServerSocket { get { return _serverSocket; } }
        public static void ConnectTCP()
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverPort);
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(ipEndPoint);
            }
            catch { MessageBox.Show("Невозможно подключиться к серверу!"); }
        }
        public static void SendTCP(ServerCommand serverCommand)
        {
            byte[] bytes = ServerDLL.Serializer.Serialize(serverCommand);
            try
            {
                int bytesSent = _serverSocket.Send(bytes);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            
        }
        public static ServerDLL.ServerResponse.Responses listenToServerResponse()
        {
            while (_serverSocket.Connected)
            {
                byte[] buffer = new byte[8196];
                int bytesRec = _serverSocket.Receive(buffer);
                return ((ServerDLL.ServerResponse)ServerDLL.Deserializer.Deserialize(buffer)).Response;
            }
            return ServerDLL.ServerResponse.Responses.Error;
        }
    }
}
