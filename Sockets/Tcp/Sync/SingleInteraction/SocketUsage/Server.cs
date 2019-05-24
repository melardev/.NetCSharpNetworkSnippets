using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.SocketUsage
{
    class Server
    {
        public static void Launch()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 3002);
            Socket serverSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);

            byte[] receivedBytes = new byte[1024];

            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("[Server] Got a new connection, Handle {0}", clientSocket.Handle);


            int numberOfBytesRead = clientSocket.Receive(receivedBytes, 0, receivedBytes.Length,
                SocketFlags.None);

            // From byte[] to string
            var receivedString = System.Text.Encoding.ASCII.GetString(receivedBytes, 0, numberOfBytesRead);

            Console.WriteLine("[Server] Received from Client Handle {0}: {1}", clientSocket.Handle, receivedString);
            // Just to show GetBytes, we could skip this and just .Write(receivedBytes)
            byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(receivedString);
            clientSocket.Send(bytesToSend, 0, bytesToSend.Length, SocketFlags.None);

            Console.WriteLine("[Server] Closing");

            clientSocket.Close();
            serverSocket.Close();
        }
    }
}