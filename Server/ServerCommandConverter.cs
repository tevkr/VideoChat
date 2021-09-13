using ServerDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
