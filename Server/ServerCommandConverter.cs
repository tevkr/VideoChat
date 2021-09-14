using ServerDLL;

namespace Server
{
    public class ServerCommandConverter
    {
        private ServerCommand _serverCommand;
        public ServerCommand ServerCommand { get { return _serverCommand; } }
        public ServerCommandConverter(byte[] bytes, int length)
        {
            _serverCommand = (ServerCommand)ServerDLL.Deserializer.Deserialize(bytes);
        }
    }
}
