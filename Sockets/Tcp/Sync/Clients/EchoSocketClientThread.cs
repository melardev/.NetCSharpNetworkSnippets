using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.Clients
{
    class EchoSocketClientThread
    {
        private static Thread _thread;
        private static Socket _clientSocket;

        private static void ListenConsoleLines()
        {
            string line = "";
            while (!line.Equals("quit", StringComparison.OrdinalIgnoreCase)
                   && !line.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                line = Console.ReadLine();
                Console.WriteLine(">$ ");
                var messageToSend = Console.ReadLine();
                if (messageToSend == null)
                    break;

                byte[] bytes = Encoding.UTF8.GetBytes(messageToSend);
                _clientSocket.Send(bytes);
            }

            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public static void Launch()
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);

            byte[] receivingBuffer = new byte[1024];

            try
            {
                _clientSocket.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", _clientSocket.Handle);

                _thread = new Thread(ListenConsoleLines);
                _thread.Start();

                // Interaction loop
                while (true)
                {
                    int numberOfBytesReceived = _clientSocket.Receive(receivingBuffer);
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
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
            }

            _thread.Abort();
            _thread.Join();
        }
    }
}