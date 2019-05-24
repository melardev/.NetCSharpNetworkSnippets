using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.SocketUsage
{
    class Client
    {
        public static void Launch()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);

            byte[] receivingBuffer = new byte[1024];

            try
            {
                Console.WriteLine("[Client] Write something to send to the Server");
                var messageToSend = Console.ReadLine();
                if (messageToSend == null)
                    return;

                clientSocket.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", clientSocket.Handle);
                clientSocket.Send(Encoding.ASCII.GetBytes(messageToSend));

                int numberOfBytesReceived = clientSocket.Receive(receivingBuffer);
                // Convert byte[] to string
                var stringData = Encoding.ASCII.GetString(receivingBuffer, 0, numberOfBytesReceived);
                Console.WriteLine("[Client] Received from Server {0}", stringData);

                Console.WriteLine("[Client] Closing");
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}