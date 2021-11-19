using SharedLibrary.SerDes;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using DataObject = SharedLibrary.Data.DataObject;

namespace WPFClient
{
    public static class Server
    {
        private const string _serverHost = "192.168.1.214";
        private const int _serverTcpPort = 9933;
        private static int _serverUdpPort;
        public static void setUdpPort(int serverUdpPort)
        {
            _serverUdpPort = serverUdpPort;
            remoteUdpIpEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverUdpPort);
        }
        public static int getUdpPort()
        {
            return _serverUdpPort;
        }
        public static void clearUdpPort()
        {
            _serverUdpPort = -1;
            remoteUdpIpEndPoint = null;
        }
        private static UdpClient _udpClient;
        private static IPEndPoint remoteTcpIpEndPoint;
        private static IPEndPoint remoteUdpIpEndPoint;
        private static Socket _serverTcpSocket;
        static Server()
        {
            _serverUdpPort = -1;
            _udpClient = new UdpClient();
            remoteTcpIpEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverTcpPort);
            remoteUdpIpEndPoint = null;
            _serverTcpSocket = null;
        }
        public static void connectTcp()
        {
            try
            {
                _serverTcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverTcpSocket.Connect(remoteTcpIpEndPoint);
            }
            catch { MessageBox.Show("Невозможно подключиться к серверу. (TCP)"); throw; }
        }
        public static void sendTcp(DataObject dataObject)
        {
            try
            {
                _serverTcpSocket.Send(Serializer.serialize(dataObject));
            }
            catch
            {
                MessageBox.Show("Ошибка при отправке данных по TCP на сервер.");
                throw;
            }
        }
        public static void sendUdp(DataObject dataObject)
        {
            if (_serverUdpPort == -1)
                return;
            try
            {
                var bytes = Serializer.serialize(dataObject);
                _udpClient.SendAsync(bytes, bytes.Length, remoteUdpIpEndPoint);
            }
            catch
            {
                MessageBox.Show("Ошибка при отправке данных по UDP на сервер.");
                throw;
            }
        }
        public static DataObject listenToServerTcpResponse()
        {
            try
            {
                while (_serverTcpSocket.Connected)
                {
                    byte[] buffer = new byte[8000];
                    _serverTcpSocket.Receive(buffer);
                    return Deserializer.deserialize(buffer);
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
