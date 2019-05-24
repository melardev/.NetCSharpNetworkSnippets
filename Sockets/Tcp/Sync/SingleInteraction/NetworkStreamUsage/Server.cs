using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.NetworkStreamUsage
{
    class Server
    {
        public static void Launch()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 3002);

            Socket serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);


            Socket clientSocket = serverSocket.Accept();
            NetworkStream networkStream = new NetworkStream(clientSocket, FileAccess.ReadWrite);
            byte[] receivingBuffer = new byte[1024];

            int numberOfBytesReceived = networkStream.Read(receivingBuffer, 0, receivingBuffer.Length);

            string message = System.Text.Encoding.UTF8.GetString(receivingBuffer);

            Console.WriteLine("[Server] Received from Client: {0}", message);

            // Echo it back
            networkStream.Write(receivingBuffer, 0, numberOfBytesReceived);

            networkStream.Close();
            clientSocket.Close();
            serverSocket.Close();
        }
    }
}