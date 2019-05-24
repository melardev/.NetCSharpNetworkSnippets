using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.TcpObjectsUsage
{
    class Launcher
    {
        public static void Launch()
        {
            // Using TcpClient and TcpListener these classes are built on top of
            // Socket classes, consider using this one rather than the raw socket class

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