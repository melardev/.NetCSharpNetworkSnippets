using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.SocketUsage
{
    class Launcher
    {
        public static void Launch()
        {
            // How to use Socket classes, this is the lowest Network programming API 
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