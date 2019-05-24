using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.StreamReaderWriter
{
    class Launcher
    {
        public static void Launch()
        {
            // This snippet is great for learning how to send and receive lines
            // however, I would not send and receive lines in my applications,
            // absolutely not. I prefer to send raw bytes instead of strings
            // delimited by new line(\n)
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