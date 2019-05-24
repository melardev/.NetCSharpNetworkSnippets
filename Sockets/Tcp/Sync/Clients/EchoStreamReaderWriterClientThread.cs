using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.Clients
{
    class EchoStreamReaderWriterClientThread
    {
        private static StreamWriter _streamWriter;
        private static Socket _clientSocket;
        private static Thread _thread;

        private static void ListenConsoleLines()
        {
            string line = "";
            while (!line.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase)
                   && !line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(">$ ");
                line = Console.ReadLine();
                if (line == null)
                    break;


                _streamWriter.WriteLine(line);
                _streamWriter.Flush(); // Don't forget to flush, or Hangs may happen
            }

            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public static void Launch()
        {
            Console.WriteLine("[Client] Write something to send to the Server");
            Console.Out.Flush();

            NetworkStream networkStream = null;
            StreamReader streamReader = null;

            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3002);

                _clientSocket.Connect(ipEndPoint);
                Console.WriteLine("[Client] Connected successfully {0}", _clientSocket.Handle);

                _thread = new Thread(ListenConsoleLines);
                _thread.Start();

                networkStream = new NetworkStream(_clientSocket);
                streamReader = new StreamReader(networkStream);
                _streamWriter = new StreamWriter(networkStream);

                while (true)
                {
                    var receivedLine = streamReader.ReadLine();
                    Console.WriteLine("[Client] Received From Server: {0}", receivedLine);
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                _streamWriter?.Close();
                streamReader?.Close();
                networkStream?.Close();
                _clientSocket?.Close();
            }

            _thread.Abort();
            _thread.Join();
        }
    }
}