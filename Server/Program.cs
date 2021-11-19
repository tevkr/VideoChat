using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.Database;

namespace Server
{
    class Program
    {
        private static string _serverHost; // IP сервера
        private static int _serverPort; // Port свервера
        private static Thread _serverThread; // Поток для прослушивания подключений новых пользователей
        static void Main(string[] args)
        {
            if (!PostgreService.testConnection())
            {
                Console.WriteLine("Can't connect to PostreSQL Server.");
                return;
            }
            PostgreService.init();
            _serverHost = ConfigurationManager.AppSettings.Get("ServerHost");
            _serverPort = int.Parse(ConfigurationManager.AppSettings.Get("ServerPort"));

            _serverThread = new Thread(startServer);
            _serverThread.IsBackground = true;
            _serverThread.Start();
            while (true) // Цикл для прослушки команд в консоли
                handlerCommands(Console.ReadLine());
        }
        private static void handlerCommands(string cmd)
        {
            cmd = cmd.ToLower();
            if (cmd.Contains("/users")) // Получить список всех пользователей
            {
                if (Server.users.Count == 0)
                    Console.WriteLine("No users:(");
                for (int i = 0; i < Server.users.Count; i++)
                    Console.WriteLine("[{0}]: {1}", Server.users[i].id, Server.users[i].userName);
                return;
            }
            if (cmd.Contains("/lobbies")) // Получить список всех пользователей
            {
                if (Server.lobbies.Count == 0)
                    Console.WriteLine("No lobbies:(");
                for (int i = 0; i < Server.lobbies.Count; i++)
                {
                    Console.WriteLine("{0}. [{1}]: {2}", (i + 1), Server.lobbies[i].id, Server.lobbies[i].name);
                    Console.Write("\tUsers: {");
                    for (int j = 0; j < Server.lobbies[i].users.Count - 1; j++)
                    {
                        Console.Write("{0} ,", Server.lobbies[i].users[j].userName);
                    }
                    Console.Write("{0}", Server.lobbies[i].users.Last().userName);
                    Console.WriteLine("}");
                }
                return;
            }
            Console.WriteLine("Wrong command.");
        }
        private static void startServer()
        {
            IPEndPoint localIpEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverPort);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);// Сокет на tcp
            socket.Bind(localIpEndPoint); // Связывает объект Socket с локальной конечной точкой.
            socket.Listen(1000); // Устанавливает объект Socket в состояние прослушивания. 1000 - Максимальная длина очереди ожидающих подключений.
            Console.WriteLine("Server has been started on IP: {0}.", localIpEndPoint);
            while (true)
            {
                try
                {
                    Socket user = socket.Accept(); // Синхронно извлекает из очереди запросов на подключение прослушивающего сокета первый ожидающий запрос на подключение.
                    Server.newUser(user);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Error: {0}", exp.Message);
                }
            }
        }
    }
}
