﻿using System;
using System.Configuration;
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
            }
            else
            {
                Console.WriteLine("Wrong command.");
            }
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
