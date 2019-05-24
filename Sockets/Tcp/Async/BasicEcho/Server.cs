using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Async.BasicEcho
{
    class Server
    {
        class Attachment
        {
            public Socket ClientSocket { get; set; }
            public byte[] Buffer { get; } = new byte[1024];
        }

        private bool Running { get; set; }
        private Thread _cmdThread;
        private readonly bool _listenKeys;

        public Server(bool listenKeys)
        {
            _listenKeys = listenKeys;
        }

        public static void Launch()
        {
            Server server = new Server(true);
            server.Start();
        }

        private static void ListenCommandLine()
        {
            string line = "";
            do
            {
                line = Console.ReadLine();
                if (line == null)
                    break;
            } while (!line.Equals("quit", StringComparison.OrdinalIgnoreCase));
        }

        public void Start()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 3002));
            serverSocket.Listen(0);
            Console.WriteLine("[+] Server started");
            Running = true;

            if (_listenKeys)
            {
                _cmdThread = new Thread(ListenCommandLine);
                _cmdThread.Start();
            }

            AcceptOneClient(serverSocket);

            if (_listenKeys)
            {
                // If we launched the Server Snippet not through Launcher.Launch()
                // then the client is not running in the same process
                // so just ReadLine()
                Console.WriteLine("[Server] Enter to exit the server");
                Console.ReadLine();
            }
            else
            {
                while (Running)
                {
                    // If we have launched the Client in the same Snippet
                    // We could not ReadLine() because that would interfere with the Client
                    // ReadLine() as well so we just sleep.
                    Thread.Sleep(30 * 1000);
                }
            }

            if (_listenKeys)
            {
                _cmdThread.Abort();
                _cmdThread.Join();
            }
        }

        private void AcceptOneClient(Socket serverSocket)
        {
            serverSocket.BeginAccept(OnPendingClientConnection, serverSocket);
        }

        public void OnPendingClientConnection(IAsyncResult result)
        {
            Console.WriteLine("[Server] Connection received");
            Attachment attachment = new Attachment();
            Socket serverSocket = ((Socket) result.AsyncState);
            attachment.ClientSocket = serverSocket.EndAccept(result);

            attachment.ClientSocket.BeginReceive(attachment.Buffer,
                0, attachment.Buffer.Length, SocketFlags.None, OnMessageReceived,
                attachment);

            AcceptOneClient(serverSocket);
        }

        private void OnMessageReceived(IAsyncResult asyncResult)
        {
            Attachment attachment = (Attachment) asyncResult.AsyncState;

            int numberOfBytesRead = attachment.ClientSocket.EndReceive(asyncResult);
            if (numberOfBytesRead > 0)
            {
                string message = Encoding.UTF8.GetString(attachment.Buffer, 0, numberOfBytesRead);
                Console.WriteLine("[Server] Received From Client: {0}",
                    message);

                attachment.ClientSocket.BeginReceive(attachment.Buffer, 0,
                    attachment.Buffer.Length, SocketFlags.None,
                    OnMessageReceived,
                    attachment);


                byte[] messageCopy = Encoding.UTF8.GetBytes(message);
                // Echo message back
                attachment.ClientSocket.BeginSend(messageCopy, 0, messageCopy.Length,
                    SocketFlags.None,
                    OnMessageSent, null);
            }
            else
            {
                attachment.ClientSocket.Shutdown(SocketShutdown.Both);
                attachment.ClientSocket.Close();
            }
        }


        public void OnMessageSent(IAsyncResult asyncResult)
        {
        }
    }
}