using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkSnippets.Sockets.Tcp.Sync.Clients
{
    class EchoTcpClient
    {
        public static void Launch()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);
            TcpClient tcpClient = new TcpClient(ipEndPoint);
            byte[] receivingBuffer = new byte[1024];

            NetworkStream clientStream = null;
            try
            {
                clientStream = tcpClient.GetStream();
                tcpClient.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", tcpClient.Client.Handle);

                while (true)
                {
                    Console.WriteLine(">$ ");
                    var messageToSend = Console.ReadLine();

                    if (messageToSend == null
                        || messageToSend.Equals("quit", StringComparison.OrdinalIgnoreCase)
                        || messageToSend.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    byte[] bytes = Encoding.ASCII.GetBytes(messageToSend);
                    clientStream.Write(bytes, 0, bytes.Length);

                    int numberOfBytesReceived = clientStream.Read(receivingBuffer, 0, receivingBuffer.Length);

                    // Convert byte[] to string
                    var stringData = Encoding.ASCII.GetString(receivingBuffer, 0, numberOfBytesReceived);
                    Console.WriteLine("[Client] Received from Server {0}", stringData);

                    Console.WriteLine("[Client] Closing");
                    tcpClient.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                clientStream?.Close();
                tcpClient.Close();
            }
        }
    }
}