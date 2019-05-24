using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.TcpObjectsUsage
{
    class Server
    {
        public static void Launch()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 3002);
            server.Start();

            byte[] receivedBytes = new byte[1024];

            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("[Server] Got a new connection, Handle {0}", client.Client.Handle);
            NetworkStream stream = client.GetStream();
            
            int numberOfBytesRead = stream.Read(receivedBytes, 0, receivedBytes.Length);

            // From byte[] to string
            var receivedString = System.Text.Encoding.ASCII.GetString(receivedBytes, 0, numberOfBytesRead);

            Console.WriteLine("[Server] Received from Client Handle {0}: {1}", client.Client.Handle, receivedString);
            // Just to show GetBytes, we could skip this and just .Write(receivedBytes)
            byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(receivedString);
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            Console.WriteLine("[Server] Closing");
            client.Close();
            server.Stop();
        }
    }
}