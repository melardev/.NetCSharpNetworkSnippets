using System;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.NetworkStreamUsage
{
    class Launcher
    {
        public static void Launch()
        {
            // The TcpObjectsUsage Snippet also used NetworkStream, but this Snippet
            // shows you how you can use Raw socket classes and wrap them into NetworkStream
            // objects for better abstraction

            Thread threadServer = new Thread(Server.Launch);
            threadServer.Start();
            Thread.Sleep(2000);
            Thread threadClient = new Thread(Client.Launch);
            threadClient.Start();

            threadClient.Join();
            threadServer.Join();

            Console.Out.Flush();
        }
    }
}