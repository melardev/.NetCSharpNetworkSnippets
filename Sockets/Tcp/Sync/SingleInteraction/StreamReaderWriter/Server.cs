using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.StreamReaderWriter
{
    class Server
    {
        public static void Launch()
        {
            IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Any, 3002);

            Socket serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(ipEndpoint);
            serverSocket.Listen(0);
            Console.WriteLine("[Server] Accepting a client");

            Socket clientSocket = serverSocket.Accept();

            IPEndPoint connectedClientEndpoint = (IPEndPoint) clientSocket.RemoteEndPoint;
            Console.WriteLine("[Server] Client connected, Address: {0}:{1}",
                connectedClientEndpoint.Address, connectedClientEndpoint.Port);

            NetworkStream networkStream = new NetworkStream(clientSocket);
            StreamReader streamReader = new StreamReader(networkStream);
            StreamWriter streamWriter = new StreamWriter(networkStream);
            
            try
            {
                var receivedLine = streamReader.ReadLine();
                Console.WriteLine("[Server] Received from Client {0}", receivedLine);
                streamWriter.WriteLine(receivedLine);
                streamWriter.Flush(); // Don't forget to flush, or hangs may happen
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
            }

            Console.WriteLine("[Server] Closing");

            streamWriter.Close();
            streamReader.Close();
            networkStream.Close();
            clientSocket.Close();
            serverSocket.Close();
        }
    }
}