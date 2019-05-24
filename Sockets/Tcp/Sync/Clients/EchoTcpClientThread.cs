using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.Clients
{
    class EchoTcpClientThread
    {
        private static Thread _thread;
        private static NetworkStream _clientStream;

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
                byte[] bytes = Encoding.ASCII.GetBytes(messageToSend);
                _clientStream.Write(bytes, 0, bytes.Length);
            }

            _clientStream.Close();
        }

        public static void Launch()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);
            TcpClient tcpClient = new TcpClient(ipEndPoint);
            byte[] receivingBuffer = new byte[1024];

            try
            {
                _clientStream = tcpClient.GetStream();
                tcpClient.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", tcpClient.Client.Handle);

                _thread = new Thread(ListenConsoleLines);
                _thread.Start();

                while (true)
                {
                   
                    int numberOfBytesReceived = _clientStream.Read(receivingBuffer, 0, receivingBuffer.Length);

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

            _thread.Abort();
            _thread.Join();
        }
    }
}