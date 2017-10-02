using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    public static class Extentions
    {
        public static byte[] ToByteArray(this object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
    }
}
