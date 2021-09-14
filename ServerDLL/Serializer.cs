using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerDLL
{
    public static class Serializer
    {
        public static byte[] Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, anySerializableObject);
                return memoryStream.ToArray();
            }
        }
    }
}
