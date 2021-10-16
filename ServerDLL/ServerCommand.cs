using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ServerDLL
{
    [Serializable]
    public class ServerCommand
    {
        private ServerCommand() { }
        public enum Commands
        {
            ChangeUserName,
            CreateLobby,
            LeaveLobby,
            GetLobbies,
            JoinLobby,
            NewFrame
        }
        private Commands _command;
        public Commands Command
        {
            get { return _command; }
            set { _command = value; }
        }
        // Смена ника-----------------------
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        // ---------------------------------

        // Создание лобби-------------------
        private string _lobbyName;
        public string LobbyName
        {
            get { return _lobbyName; }
            set { _lobbyName = value; }
        }
        private int _lobbyCapacity;
        public int LobbyCapacity
        {
            get { return _lobbyCapacity; }
            set { _lobbyCapacity = value; }
        }
        private string _lobbyPassword;
        public string LobbyPassword
        {
            get { return _lobbyPassword; }
            set { _lobbyPassword = value; }
        }
        // ---------------------------------

        // Присоединение к лобби-------------------
        private string _lobbyId;
        public string LobbyId
        {
            get { return _lobbyId; }
            set { _lobbyId = value; }
        }
        // ---------------------------------

        // Новый фрейм-------------------
        private string _userId;

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        private Bitmap _frame;
        public Bitmap Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }
        private byte[] _frameBytes;
        public byte[] FrameBytes
        {
            get { return _frameBytes; }
            set { _frameBytes = value; }
        }
        // ---------------------------------

        // Создание команды сервера (для клиента)
        public static ServerCommand changeUserNameServerCommand(string newUserName)
        {
            ServerCommand result = new ServerCommand();
            result.Command = Commands.ChangeUserName;
            result.UserName = newUserName;
            return result;
        }
        public static ServerCommand createLobbyCommand(string lobbyName, int lobbyCapacity, string lobbyPassword)
        {
            ServerCommand result = new ServerCommand();
            result.Command = Commands.CreateLobby;
            result.LobbyName = lobbyName;
            result.LobbyCapacity = lobbyCapacity;
            result.LobbyPassword = lobbyPassword;
            return result;
        }
        public static ServerCommand leaveLobbyCommand()
        {
            ServerCommand result = new ServerCommand();
            result.Command = Commands.LeaveLobby;
            return result;
        }
        public static ServerCommand getLobbiesCommand()
        {
            ServerCommand result = new ServerCommand();
            result.Command = Commands.GetLobbies;
            return result;
        }
        public static ServerCommand joinLobbyCommand(string lobbyId, string lobbyPassword)
        {
            ServerCommand result = new ServerCommand();
            result.Command = Commands.JoinLobby;
            result.LobbyId = lobbyId;
            result.LobbyPassword = lobbyPassword;
            return result;
        }

        public static ServerCommand newFrameCommand(Bitmap newFrame, string userId)
        {
            ServerCommand result = new ServerCommand();
            result.Command = Commands.NewFrame;
            result.UserId = userId;
            //result.Frame = newFrame;
            try
            {
                using (var ms = new MemoryStream())
                {
                    newFrame.Save(ms, ImageFormat.Jpeg);
                    result.FrameBytes = ms.ToArray();
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }
        // ---------------------------------
    }
}
