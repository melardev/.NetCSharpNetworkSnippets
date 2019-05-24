using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.BinSerialization
{
    class Server
    {
        public static void Launch()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 3002);
            server.Start();
            TcpClient client = server.AcceptTcpClient();
            NetworkStream networkStream = client.GetStream();
            IFormatter formatter = new BinaryFormatter();

            Message firstMessage = (Message) formatter.Deserialize(networkStream);
            Message secondMessage = (Message) formatter.Deserialize(networkStream);
            Console.WriteLine("{0}: {1} {2}",
                firstMessage.UserName,
                firstMessage.Contents,
                firstMessage.CreatedAt.ToString());


            Console.WriteLine("{0}: {1} {2}",
                secondMessage.UserName,
                secondMessage.Contents,
                secondMessage.CreatedAt.ToString());

            networkStream.Close();
            server.Stop();

            Console.WriteLine("Finished Server");
        }
    }
}