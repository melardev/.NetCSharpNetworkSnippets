using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.Servers
{
    class EchoServerThread
    {
        private static TcpListener _serverListener;

        private static void ListenQuitCommand()
        {
            string instruction;
            do
            {
                instruction = Console.ReadLine();
            } while (instruction != null && !instruction.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase));

            _serverListener.Stop();
        }

        public static void Launch()
        {
            // Test this server with the below line in your command prompt
            // $ ncat -v localhost 3002
            _serverListener = new TcpListener(IPAddress.Loopback, 3002);
            _serverListener.Start();

            Console.WriteLine("[+] Accepting Clients");
            List<ClientConnection> clients = new List<ClientConnection>();
            Thread cmdThread;
            while (true)
            {
                /* // "Async" way, For Async Snippets look the Async Folder
                while (!serverListener.Pending())
                {
                    Thread.Sleep(2000);
                }
                */

                cmdThread = new Thread(ListenQuitCommand);
                cmdThread.Start();

                try
                {
                    TcpClient client = _serverListener.AcceptTcpClient();


                    ClientConnection clientClientConnection = new ClientConnection
                    {
                        TcpClientObject = client,
                    };

                    clients.Add(clientClientConnection);

                    clientClientConnection.RunningThread = new Thread(clientClientConnection.Interact);
                    clientClientConnection.RunningThread.Start();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }

            cmdThread.Abort();
            cmdThread.Join();
            foreach (ClientConnection client in clients)
            {
                client.TcpClientObject.Close();
                client.RunningThread.Join();
            }
        }

        class ClientConnection
        {
            public TcpClient TcpClientObject { get; set; }
            public Thread RunningThread { get; set; }

            public void Interact(object threadArgument)
            {
                NetworkStream networkStream = null;
                try
                {
                    byte[] receivingBuffer = new byte[1024];
                    networkStream = TcpClientObject.GetStream();
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
                        Console.WriteLine("[Server] Received from {0}: {1}", TcpClientObject.Client.Handle, message);
                        // Echoing it back
                        networkStream.Write(receivingBuffer, 0, numberOfBytesReceived);
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    networkStream?.Close();
                    TcpClientObject.Close();
                }
            }
        }
    }
}