using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharedLibrary.SerDes
{
    public static class Serializer
    {
        public static byte[] serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
