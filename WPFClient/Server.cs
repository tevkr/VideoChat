using ServerDLL;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace WPFClient
{
    public static class Server
    {
        public const string _serverHost = "127.0.0.1";
        public const int _serverPort = 9933;
        private static int _serverPortUDP = -1;
        private static UdpClient udpClient = new UdpClient();
        private static IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverPort);
        private static IPEndPoint ipEndPointUDP = null;
        private static Socket _serverSocket;
        public static Socket ServerSocket { get { return _serverSocket; } }
        public static void SetUDPPort(int port)
        {
            _serverPortUDP = port;
            ipEndPointUDP = new IPEndPoint(IPAddress.Parse(_serverHost), _serverPortUDP);
        }
        public static int GetUDPPort()
        {
            return _serverPortUDP;
        }
        public static void ClearUDPPort()
        {
            _serverPortUDP = -1;
            ipEndPointUDP = null;
        }
        public static void ConnectTCP()
        {
            try
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(ipEndPoint);
            }
            catch { MessageBox.Show("Невозможно подключиться к серверу. (TCP)"); throw; }
        }
        public static void ConnectUDP()
        {
            try
            {
                udpClient.Connect(ipEndPoint);
            }
            catch { MessageBox.Show("Невозможно подключиться к серверу. (UDP)"); throw; }
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
        public static void SendUDP(ServerCommand serverCommand)
        {
            if (_serverPortUDP == -1)
                return;
            byte[] bytes = ServerDLL.Serializer.Serialize(serverCommand);
            try
            {
                udpClient.Send(bytes, bytes.Length, ipEndPointUDP);
            }
            catch
            {
                MessageBox.Show("Ошибка при отправке данных по UDP на сервер.");
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
