using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SharedLibrary.Data;

namespace SharedLibrary.SerDes
{
    public static class Deserializer
    {
        public static DataObject deserialize(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return (new BinaryFormatter()).Deserialize(ms) as DataObject;
        }
    }
}
