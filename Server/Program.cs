using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        private static string _serverHost; // IP сервера
        private static int _serverPort; // Port свервера
        private static Thread _serverThread; // Поток для прослушивания подключений новых пользователей
        static void Main(string[] args)
        {
            _serverHost = ConfigurationManager.AppSettings.Get("ServerHost"); // localhost
            _serverPort = int.Parse(ConfigurationManager.AppSettings.Get("ServerPort")); // 48654

            _serverThread = new Thread(startServer);
            _serverThread.IsBackground = true;
            _serverThread.Start();
            while (true) // Цикл для ввода команд
                handlerCommands(Console.ReadLine());
        }
        private static void handlerCommands(string cmd)
        {
            cmd = cmd.ToLower();
            if (cmd.Contains("/users")) // Получить список всех пользователей
            {
                if (Server.Users.Count == 0)
                    Console.WriteLine("No users:(");
                for (int i = 0; i < Server.Users.Count; i++)
                    Console.WriteLine("[{0}]: {1}", Server.Users[i].Id, Server.Users[i].UserName);
                return;
            }
            if (cmd.Contains("/lobbies")) // Получить список всех пользователей
            {
                if (Server.lobbies.Count == 0)
                    Console.WriteLine("No lobbies:(");
                for (int i = 0; i < Server.lobbies.Count; i++)
                {
                    Console.WriteLine("{0}. [{1}]: {2}", (i + 1), Server.lobbies[i].Id, Server.lobbies[i].Name);
                    Console.Write("\tUsers: {");
                    for (int j = 0; j < Server.lobbies[i].UsersCount - 1; j++)
                    {
                        Console.Write("{0} ,", Server.lobbies[i].Users[j].UserName);
                    }
                    Console.Write("{0}", Server.lobbies[i].Users.Last().UserName);
                    Console.WriteLine("}");
                }
                return;
            }
            Console.WriteLine("Wrong command.");
        }
        private static void startServer()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverPort);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);// Сокет на udp
            socket.Bind(ipEndPoint); // Связывает объект Socket с локальной конечной точкой.
            socket.Listen(1000); // Устанавливает объект Socket в состояние прослушивания. 1000 - Максимальная длина очереди ожидающих подключений.
            Console.WriteLine("Server has been started on IP: {0}.", ipEndPoint);
            while (true)
            {
                try
                {
                    Socket user = socket.Accept(); // Cинхронно извлекает из очереди запросов на подключение прослушивающего сокета первый ожидающий запрос на подключение.
                    Server.NewUser(user);
                }
                catch (Exception exp) { Console.WriteLine("Error: {0}", exp.Message); }
            }
        }
    }
}
