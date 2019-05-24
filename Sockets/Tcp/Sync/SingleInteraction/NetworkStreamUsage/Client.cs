using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.NetworkStreamUsage
{
    class Client
    {
        static public void Launch()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 3002);

            Socket socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            Console.WriteLine("Write something to be sent to the server");
            string message = Console.ReadLine();
            if (message == null)
                return;

            socket.Connect(ipEndPoint);

            NetworkStream networkStream = new NetworkStream(socket, FileAccess.ReadWrite);
            if (!socket.Connected)
            {
                Console.WriteLine("Error connecting to server");
                return;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            networkStream.Write(messageBytes, 0, messageBytes.Length);


            byte[] receivingBuffer = new byte[1024];
            int numberOfBytesReceived = networkStream.Read(receivingBuffer, 0, receivingBuffer.Length);

            Console.WriteLine("[Client] Received from the server: {0}", Encoding.UTF8.GetString(receivingBuffer));

            networkStream.Close();
            socket.Close();
        }
    }
}