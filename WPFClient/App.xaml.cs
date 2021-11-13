using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.MVVM.ViewModel;

namespace WPFClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string LocalUserId;

        [STAThread] 
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Server.connectTcp();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Environment.Exit(Environment.ExitCode);
        }
    }
}
