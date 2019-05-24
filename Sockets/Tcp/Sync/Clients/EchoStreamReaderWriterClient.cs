using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NetworkSnippets.Sockets.Tcp.Sync.Clients
{
    class EchoStreamReaderWriterClient
    {
        public static void Launch()
        {
            Console.WriteLine("[Client] Write something to send to the Server");
            Console.Out.Flush();

            Socket clientSocket = null;
            NetworkStream networkStream = null;
            StreamReader streamReader = null;
            StreamWriter streamWriter = null;
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);

                clientSocket.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", clientSocket.Handle);

                networkStream = new NetworkStream(clientSocket);
                streamReader = new StreamReader(networkStream);
                streamWriter = new StreamWriter(networkStream);

                while (true)
                {
                    var messageToSend = Console.ReadLine();
                    if (messageToSend == null
                        || messageToSend.Equals("quit", StringComparison.OrdinalIgnoreCase)
                        || messageToSend.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    streamWriter.WriteLine(messageToSend);
                    streamWriter.Flush(); // Don't forget to flush, or Hangs may happen
                    var receivedLine = streamReader.ReadLine();

                    Console.WriteLine("[Client] Received From Server: {0}", receivedLine);
                }

                Console.WriteLine("[Client] Closing");
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                streamWriter?.Close();
                streamReader?.Close();
                networkStream?.Close();
                clientSocket?.Close();
            }
        }
    }
}