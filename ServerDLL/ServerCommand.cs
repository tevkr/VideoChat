using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerDLL
{
    [Serializable]
    public class ServerCommand
    {
        private ServerCommand() { }
        public enum Commands
        {
            ChangeUserName
        }
        private Commands _command;
        public Commands Command
        {
            get { return _command; }
            set { _command = value; }
        }
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public static ServerCommand changeUserNameServerCommand(string newUserName)
        {
            ServerCommand result = new ServerCommand();
            result.Command = Commands.ChangeUserName;
            result.UserName = newUserName;
            return result;
        }
    }
}
