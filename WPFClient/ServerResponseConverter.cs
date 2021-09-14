using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerDLL;

namespace WPFClient
{
    public class ServerResponseConverter
    {
        private ServerResponse _serverResponse;
        public ServerResponse ServerResponse { get { return _serverResponse; } }
        public ServerResponseConverter(byte[] bytes, int length)
        {
            _serverResponse = (ServerResponse)ServerDLL.Deserializer.Deserialize(bytes);
        }
    }
}
