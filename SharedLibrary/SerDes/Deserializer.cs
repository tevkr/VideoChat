using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SharedLibrary.Data;

namespace SharedLibrary.SerDes
{
    static class Deserializer
    {
        public static DataObject Deserialize(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return (new BinaryFormatter()).Deserialize(ms) as DataObject;
        }
    }
}
