using System;

namespace NetworkSnippets
{
    class Program
    {
        static void Main(string[] args)
        {
            // Auxiliary
            // Auxiliary.DnsResolveUsage.Launch();
            // Auxiliary.AnyAddress.Launch();
            // Auxiliary.DnsGetHostEntryUsage.Launch();
            // Auxiliary.GetAllNetworkInterfaces.Launch();
            // Auxiliary.GetPcIpProperties.Launch();
            // Auxiliary.NetAvailabilityChanged.Launch();
            // Auxiliary.GetPcIpAddresses.Launch();


            // Single interaction snippets are intended for really basic networking stuff
            // Ideal for absolute beginners, but hey, you have to study this quickly and
            // go to advanced stuff, because this snippets are really useless in real world
            // scenarios

            // Sockets.Tcp.Sync.SingleInteraction.SocketUsage.Launcher.Launch();
            // Sockets.Tcp.Sync.SingleInteraction.TcpObjectsUsage.Launcher.Launch();
            // Sockets.Tcp.Sync.SingleInteraction.NetworkStreamUsage.Launcher.Launch();
            // Sockets.Tcp.Sync.SingleInteraction.StreamReaderWriter.Launcher.Launch();
            // Sockets.Tcp.Sync.SingleInteraction.BinSerialization.Launcher.Launch();
            // Sockets.Tcp.Sync.SingleInteraction.SerializationBytes.Launcher.Launch();

            // Now comes the interesting part, these Servers handle many clients
            // and these clients keep interacting with the server. I recommend installing
            // Ncat which ships with Nmap, to be able to play with the Servers and Clients

            // Servers:
            // To interact with the servers you can spawn from your terminal window a ncat
            // instance: ncat -v localhost 3002
            // -v means verbose
            // localhost 3002 means connect to localhost , at port 3002

            // Sockets.Tcp.Sync.Servers.EchoServerThread.Launch();
            // Sockets.Tcp.Sync.Servers.EchoServerThreadPool.Launch();

            // Clients
            // To itneract with the clients you spawn a testing server in your terminal window with:
            // $ ncat -k -lv 3002
            // -l means listen(server mode)
            // -v means verbose
            // 3002 is listen at port 3002


            // Sockets.Tcp.Sync.Clients.EchoSocketClient.Launch();
            // Sockets.Tcp.Sync.Clients.EchoSocketClientThread.Launch();

            // Sockets.Tcp.Sync.Clients.EchoStreamReaderWriterClient.Launch();
            // Sockets.Tcp.Sync.Clients.EchoStreamReaderWriterClientThread.Launch();

            // Sockets.Tcp.Sync.Clients.EchoTcpClient.Launch();
            // Sockets.Tcp.Sync.Clients.EchoTcpClientThread.Launch();


            // Asynchronous, this is difficult, do not try to begin from here if
            // you don't understand the basics already

            // Sockets.Tcp.Async.Basic.Launcher.Launch();
            // Sockets.Tcp.Async.BasicEcho.Launcher.Launch();
            // Sockets.Tcp.Async.BinarySerialization.Launcher.Launch();

            Console.ReadLine();
        }
    }
}