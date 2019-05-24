using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkSnippets.Sockets.Tcp.Async.Basic
{
    /// <summary>
    /// Client Snippet for Asynchronous socket programming, this is absolutely not the way to go
    /// This is only intended for demonstration of the core concepts BUT this is wrong wrong wrong
    /// There are many things to take care about when using Asynchronous sockets, this works
    /// great a simple example, but in real world applications this has to be improved a lot
    /// I have other snippets where I show the right way of doing Asynchronous sockets.
    /// </summary>
    class Client
    {
        public Socket ClientSocket { get; set; }

        public static void Launch()
        {
            Client client = new Client();
            client.Start();
        }

        public void Start()
        {
            Console.WriteLine("[+] Trying to connect, wait a moment please");

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.BeginConnect(new IPEndPoint(IPAddress.Loopback, 3002), OnConnectionCallback, ClientSocket);
        }

        public void OnConnectionCallback(IAsyncResult result)
        {
            ClientSocket = (Socket) result.AsyncState;
            ClientSocket.EndConnect(result);

            ListenCommandLine();
        }

        private void ListenCommandLine()
        {
            string line = "";

            do
            {
                Console.WriteLine("Write something to sent to the server");
                line = Console.ReadLine();
                if (line == null)
                    break;


                ClientSocket.BeginSend(Encoding.UTF8.GetBytes(line), 0, line.Length, SocketFlags.None,
                    OnMessageSent, null);
            } while (!line.Equals("quit", StringComparison.OrdinalIgnoreCase));

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }

        private void OnMessageSent(IAsyncResult asyncResult)
        {
        }
    }
}