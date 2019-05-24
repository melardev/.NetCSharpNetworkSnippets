using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.StreamReaderWriter
{
    class Client
    {
        public static void Launch()
        {
            Console.WriteLine("[Client] Write something to send to the Server");
            Console.Out.Flush();
            var messageToSend = Console.ReadLine();
            if (messageToSend == null)
                return;
            try
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);

                clientSocket.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", clientSocket.Handle);

                NetworkStream networkStream = new NetworkStream(clientSocket);
                StreamReader streamReader = new StreamReader(networkStream);
                StreamWriter streamWriter = new StreamWriter(networkStream);


                streamWriter.WriteLine(messageToSend);
                streamWriter.Flush(); // Don't forget to flush, or Hangs may happen
                var receivedLine = streamReader.ReadLine();

                Console.WriteLine("[Client] Received From Server: {0}", receivedLine);
                Console.WriteLine("[Client] Closing");

                streamWriter.Close();
                streamReader.Close();
                networkStream.Close();
                clientSocket.Close();
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}