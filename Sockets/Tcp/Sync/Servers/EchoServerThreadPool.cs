using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.Servers
{
    class EchoServerThreadPool
    {
        private static void ListenQuitCommand(object argument)
        {
            string instruction;
            do
            {
                instruction = Console.ReadLine();
            } while (instruction != null && instruction.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase));
        }

        public static void Launch()
        {
            // Definitely I would never use this kind of code
            // ThreadPools are used to execute short lived tasks in a thread managed by the ThreadPool
            // and not to keep running a Network Client Interaction which most likely takes a lot of time

            TcpListener serverListener = new TcpListener(IPAddress.Loopback, 3002);
            serverListener.Start();

            Console.WriteLine("[+] Accepting Clients");
            while (true)
            {
                /* // "Async" way, For Async Snippets look the Async Folder
                while (!serverListener.Pending())
                {
                    Thread.Sleep(2000);
                }
                */

                ThreadPool.QueueUserWorkItem(new WaitCallback(ListenQuitCommand));
                TcpClient client = serverListener.AcceptTcpClient();
                ClientConnection clientClientConnection = new ClientConnection
                {
                    Client = client
                };
                ThreadPool.QueueUserWorkItem(clientClientConnection.Interact);
            }
        }

        class ClientConnection
        {
            public TcpClient Client { get; set; }

            public void Interact(object threadArgument)
            {
                byte[] receivingBuffer = new byte[1024];
                NetworkStream networkStream = Client.GetStream();
                networkStream.Write(receivingBuffer, 0, receivingBuffer.Length);
                receivingBuffer = new byte[1024];
                while (true)
                {
                    var numberOfBytesReceived = networkStream.Read(receivingBuffer, 0, receivingBuffer.Length);
                    if (numberOfBytesReceived == 0)
                        break;

                    string message = Encoding.UTF8.GetString(receivingBuffer, 0, numberOfBytesReceived);
                    if (message.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                        break;
                    Console.WriteLine("[Server] Received from {0}: {1}", Client.Client.Handle, message);
                    // Echoing it back
                    networkStream.Write(receivingBuffer, 0, numberOfBytesReceived);
                }

                networkStream.Close();
                Client.Close();
            }
        }
    }
}