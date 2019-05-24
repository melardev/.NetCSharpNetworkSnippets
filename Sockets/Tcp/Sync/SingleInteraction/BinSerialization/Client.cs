using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.BinSerialization
{
    class Client
    {
        public static void Launch()
        {
            var message1 = new Message
            {
                UserId = 14,
                UserName = "MelarDev",
                Contents = "Hi my friend",
                CreatedAt = DateTime.Now
            };
            var message2 = new Message
            {
                UserId = 21,
                UserName = "John",
                Contents = "[3 Days ago] Hi!!",
                CreatedAt = DateTime.Now.AddDays(-3)
            };


            TcpClient client = new TcpClient("127.0.0.1", 3002);
            bool connected = client.Connected;
            if (!connected)
                Console.WriteLine("Not connected");
            IFormatter formatter = new BinaryFormatter();
            NetworkStream networkStream = client.GetStream();

            Console.WriteLine("[Client] Sending two Message Objects to the Server");
            formatter.Serialize(networkStream, message1);
            formatter.Serialize(networkStream, message2);

            networkStream.Close();
            client.Close();

            Console.WriteLine("Finished Client");
        }
    }
}