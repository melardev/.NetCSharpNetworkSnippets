using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.BinSerialization
{
    class Launcher
    {
        public static void Launch()
        {
            // This snippet shows how to send and receive C# Objects
            // over the network using serialization
            Thread threadServer = new Thread(Server.Launch);
            threadServer.Start();
            Thread.Sleep(2000);
            Thread threadClient = new Thread(Client.Launch);
            threadClient.Start();

            threadClient.Join();
            threadServer.Join();
            
        }
    }
}