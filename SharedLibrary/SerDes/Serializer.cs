using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SharedLibrary.SerDes
{
    static class Serializer
    {
        public static byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
