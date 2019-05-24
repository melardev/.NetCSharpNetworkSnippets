using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.SerializationBytes
{
    class Launcher
    {
        public static void Launch()
        {
            // How to send serialized objects and receive them, but instead of sending
            // the object itself we send the bytes array
            // This snippet is really important because this is how we do asynchronous socket
            // programming, and how we do net programming in general most of the times
            // We first send the packet size so the receiving party knows how much data
            // to expect and maybe prepare a buffer(byte[]) big enough to hold it, Then we send packet payload
            // Take as an exercise the following: Reverse engineer any MMORPG Game,
            // Set a breakpoint on recv() or send() Win32 API calls
            // you will see that all games send first the packet size then the rest :)

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
