using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServerDLL
{
    public static class Deserializer
    {
        public static object Deserialize(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
                return (new BinaryFormatter()).Deserialize(memoryStream);
        }
    }
}
