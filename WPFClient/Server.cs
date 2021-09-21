using ServerDLL;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace WPFClient
{
    public static class Server
    {
        public const string _serverHost = "192.168.1.214";
        public const int _serverPort = 9933;
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
            catch { MessageBox.Show("Невозможно подключиться к серверу."); throw; }
        }
        public static void SendTCP(ServerCommand serverCommand)
        {
            byte[] bytes = ServerDLL.Serializer.Serialize(serverCommand);
            try
            {
                int bytesSent = _serverSocket.Send(bytes);
            }
            catch
            {
                MessageBox.Show("Ошибка при отправке данных по TCP на сервер.");
                throw;
            }
            
        }
        public static byte[] listenToServerResponse()
        {
            try
            {
                while (_serverSocket.Connected)
                {
                    //_serverSocket.ReceiveTimeout = 1000;
                    byte[] buffer = new byte[8196];
                    int bytesRec = _serverSocket.Receive(buffer);
                    return buffer;
                }
                return null;
            }
            catch
            {
                MessageBox.Show("Ошибка при прослушивании TCP сокета.");
                throw;
            }
        }
    }
}
