using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Async.Basic
{
    class Launcher
    {
        public static void Launch()
        {
            // This snippet shows how to send and receive C# Objects
            // over the network using serialization
            Server server = new Server(false);
            Thread threadServer = new Thread(server.Start);
            threadServer.Start();
            Thread.Sleep(2000);
            Thread threadClient = new Thread(Client.Launch);
            threadClient.Start();

            threadClient.Join();
            threadServer.Join();
        }
    }
}