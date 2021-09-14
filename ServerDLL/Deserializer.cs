using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
