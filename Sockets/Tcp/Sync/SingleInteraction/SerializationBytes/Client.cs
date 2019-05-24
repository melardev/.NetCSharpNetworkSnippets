using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.SerializationBytes
{
    class Client
    {
        public static void Launch()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);

                Message message = new Message
                {
                    From = "Melardev",
                    Subject = "CSharp serialization",
                    Body = "Binary Serialization applied to CSharp Network programming",
                    CreatedAt = DateTime.Now
                };


                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();

                using (Socket clientSock = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.IP))
                {
                    try
                    {
                        clientSock.Blocking = true;
                        clientSock.Connect(ipEndPoint);

                        Console.WriteLine("[Client] Connected successfully, trying to send Message serialized");
                        Console.Out.Flush();

                        formatter.Serialize(memoryStream, message);
                        byte[] serializedObject = memoryStream.ToArray();

                        // Prepend the packet Size Length First so the Server knows how much to expect
                        clientSock.SendTo(BitConverter.GetBytes((ulong) serializedObject.Length), ipEndPoint);
                        clientSock.SendTo(serializedObject, ipEndPoint);

                        clientSock.Close();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("[Client] {0}", exception.ToString());
                        Console.Out.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}