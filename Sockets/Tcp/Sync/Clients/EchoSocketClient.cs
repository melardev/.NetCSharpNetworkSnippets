using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkSnippets.Sockets.Tcp.Sync.Clients
{
    class EchoSocketClient
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
                clientSocket.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", clientSocket.Handle);
                while (true)
                {
                    Console.WriteLine("$");
                    var messageToSend = Console.ReadLine();

                    if (messageToSend == null
                        || messageToSend.Equals("quit", StringComparison.OrdinalIgnoreCase)
                        || messageToSend.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    clientSocket.Send(Encoding.ASCII.GetBytes(messageToSend));

                    int numberOfBytesReceived = clientSocket.Receive(receivingBuffer);
                    // Convert byte[] to string
                    var stringData = Encoding.ASCII.GetString(receivingBuffer, 0, numberOfBytesReceived);
                    Console.WriteLine("[Client] Received from Server {0}", stringData);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.WriteLine("[Client] Closing");
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}