using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkSnippets.Sockets.Tcp.Async.BinarySerialization
{
    [Serializable]
    public class Notification
    {
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public byte[] Serialize()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, this);
            return memoryStream.GetBuffer();
        }

        public static Notification DeSerialize(byte[] buffer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            /*
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(buffer, 0, buffer.Length);
            memoryStream.Seek(0, 0);
            */
            MemoryStream memoryStream = new MemoryStream(buffer);
            return (Notification) formatter.Deserialize(memoryStream);
        }
    }
}