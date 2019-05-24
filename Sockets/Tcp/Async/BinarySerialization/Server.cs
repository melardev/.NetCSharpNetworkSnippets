using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkSnippets.Sockets.Tcp.Async.BinarySerialization
{
    class Server
    {
        // I call it attachment because of my Java background, In Java the object
        // we move around in Asynchronous socket programming is Attachment, each connection
        // state( state is just data) is stored in an object, that object we call it Attachment
        // for this basic scenario, the state we wanna remember for each connection is:
        // - Socket object, to be used for read from it
        // - A HeaderBuffer byte[] that reads the packet header, we should agree both Client and Server
        //      that the header is 8 bytes long and contains the PayloadBuffer Length
        //      A flag to keep track whether the Header is completely read or not
        // - A ReceivingBuffer byte[] that keeps holding the data as it it comes
        // - NumberOfBytesReadSoFar so we know how much remains to be read
        public class ClientConnectionAttachment
        {
            public Socket ClientSocket { get; set; }

            // Needed for Read operations
            public byte[] ReceivingHeaderBuffer { get; set; } = new byte[sizeof(ulong)];
            public byte[] ReceivingPayloadBuffer { get; set; }
            public ulong NumberOfBytesReadSoFar { get; set; }
            public ulong ExpectedReceivingPayloadLength { get; set; }
        }

        public bool Running { get; set; }

        private Thread _cmdThread;
        private bool _listenKeys;

        public Server(bool listenKeys = false)
        {
            this._listenKeys = listenKeys;
        }


        public static void Launch()
        {
            Server server = new Server(true);
            server.Start();
        }


        public void Start()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 3002));
            serverSocket.Listen(0);
            Console.WriteLine("[+] Server started");
            Running = true;

            if (_listenKeys)
            {
                _cmdThread = new Thread(ListenCommandLine);
                _cmdThread.Start();
            }


            AcceptOneClient(serverSocket);

            if (_listenKeys)
            {
                // If we launched the Server Snippet not through Launcher.Launch()
                // then the client is not running in the same process
                // so just ReadLine()
                Console.WriteLine("[Server] Enter to exit the server");
                Console.ReadLine();
            }
            else
            {
                while (Running)
                {
                    // If we have launched the Client in the same Snippet
                    // We could not ReadLine() because that would interfere with the Client
                    // ReadLine() as well so we just sleep.
                    Thread.Sleep(30 * 1000);
                }
            }

            if (_listenKeys)
            {
                _cmdThread.Abort();
                _cmdThread.Join();
            }
        }


        private void ListenCommandLine()
        {
            string line = "";
            do
            {
                line = Console.ReadLine();
                if (line == null)
                    break;
            } while (!line.Equals("quit", StringComparison.OrdinalIgnoreCase));

            Running = false;
        }

        private void AcceptOneClient(Socket serverSocket)
        {
            serverSocket.BeginAccept(OnPendingClientConnection, serverSocket);
        }

        public void OnPendingClientConnection(IAsyncResult result)
        {
            Console.WriteLine("Connection received");
            ClientConnectionAttachment attachment = new ClientConnectionAttachment();
            Socket serverSocket = ((Socket) result.AsyncState);
            attachment.ClientSocket = serverSocket.EndAccept(result);

            // Schedule one server async Accept
            AcceptOneClient(serverSocket);

            // Schedule an async read
            ReadHeader(attachment);
        }

        private void ReadHeader(ClientConnectionAttachment attachment)
        {
            // BeginReceive is asynchronous, so this function call returns immediately
            // First we want the header so begin receiving the header (8 bytes long)
            attachment.ClientSocket.BeginReceive(attachment.ReceivingHeaderBuffer, 0 /*Offset*/,
                attachment.ReceivingHeaderBuffer.Length /* 8 bytes long to read for the header */,
                SocketFlags.None, OnHeaderReceived,
                attachment);
        }

        private void OnHeaderReceived(IAsyncResult result)
        {
            ClientConnectionAttachment attachment = (ClientConnectionAttachment) result.AsyncState;

            // Keep track of how many bytes we have read, notice the + sign
            attachment.NumberOfBytesReadSoFar += (ulong) attachment.ClientSocket.EndReceive(result);

            if (attachment.NumberOfBytesReadSoFar > 0)
            {
                if (attachment.NumberOfBytesReadSoFar == (ulong) attachment.ReceivingHeaderBuffer.Length)
                {
                    // We have read the header, now let's proceed to read the payload

                    // Reset this, because we are gonna use it
                    // to keep track of number of bytes read for the payload, instead of Header
                    attachment.NumberOfBytesReadSoFar = 0;

                    attachment.ExpectedReceivingPayloadLength =
                        BitConverter.ToUInt64(attachment.ReceivingHeaderBuffer, 0);

                    // You have to make sure payloadLength is > 0 and < than what you think would be too big,
                    // just to keep hackers away which I don't for this simple snippet
                    // but keep in mind that serialized C# Objects are big ...

                    // Now a decision, should I create a new byte[] or just reuse the one used for the previous packet?
                    if (attachment.ReceivingPayloadBuffer == null || // If the payload byte[] is null
                        // or its capacity is less than we need
                        (ulong) attachment.ReceivingPayloadBuffer.Length <
                        attachment.ExpectedReceivingPayloadLength)
                    {
                        // Then create a new one
                        attachment.ReceivingPayloadBuffer = new byte[attachment.ExpectedReceivingPayloadLength];
                    }

                    // Begin reading the payload now
                    attachment.ClientSocket.BeginReceive(attachment.ReceivingPayloadBuffer, 0,
                        (int) attachment.ExpectedReceivingPayloadLength, SocketFlags.None, OnPayloadReceived,
                        attachment);
                    return;
                }
                else
                {
                    // We are not done reading the header, keep reading it
                    // For example if have just read 3 bytes, now we are expecting 5 more bytes (because we know
                    // the header should be 8 bytes long, so we begin reading and filling the buffer from offset 3.
                    // So: NumberOfBytesReadSoFar would be 3,
                    // second parameter would be NumberOfBytesReadSoFar(3),
                    // forth parameter would be 5 (because of: 8 - 3).
                    // Hope make sense, it is easy.
                    attachment.ClientSocket.BeginReceive(attachment.ReceivingHeaderBuffer,
                        (int) attachment.NumberOfBytesReadSoFar,
                        (int) ((ulong) attachment.ReceivingHeaderBuffer.LongLength -
                               attachment.NumberOfBytesReadSoFar),
                        SocketFlags.None,
                        OnHeaderReceived, attachment);
                    return;
                }
            }
        }


        private void OnPayloadReceived(IAsyncResult result)
        {
            ClientConnectionAttachment attachment = (ClientConnectionAttachment) result.AsyncState;

            // Keep track of how many bytes we have read, notice the + sign
            attachment.NumberOfBytesReadSoFar += (ulong) attachment.ClientSocket.EndReceive(result);

            if (attachment.NumberOfBytesReadSoFar > 0)
            {
                // If we are here that means the Header was already Read
                // A common bug is to do if(NumberOfBytesReadSoFar < ReceivingPayloadBuffer.Length)
                // that is wrong when reusing the ReceivingPayloadBuffer (which we are in this case)
                // because what would happen if wew are reusing a payload buffer bigger than
                // the currently expected payload size? well, you guessed it, we would be mixing
                // to packets in the same buffer...
                if (attachment.NumberOfBytesReadSoFar < attachment.ExpectedReceivingPayloadLength)
                {
                    // If we have not filled the buffer that means we have to keep reading
                    // Obviously we don't want to override what we have read so far, so adjust
                    // the offset, and also the length because now we have to take into account
                    // what we have read so far
                    attachment.ClientSocket.BeginReceive(attachment.ReceivingPayloadBuffer,
                        (int) attachment.NumberOfBytesReadSoFar, // Offset
                        (int) ((ulong) attachment.ReceivingPayloadBuffer.LongLength -
                               attachment.NumberOfBytesReadSoFar),
                        SocketFlags.None,
                        OnPayloadReceived,
                        attachment);
                }
                else
                {
                    OnPacketReady(attachment);

                    attachment.NumberOfBytesReadSoFar = 0;
                    // Empty the byte[] arrays, this is actually not needed, but it is a good practice
                    Array.Clear(attachment.ReceivingHeaderBuffer, 0, attachment.ReceivingHeaderBuffer.Length);
                    Array.Clear(attachment.ReceivingPayloadBuffer, 0, attachment.ReceivingPayloadBuffer.Length);

                    // Read another packet
                    ReadHeader(attachment);
                }
            }
            else
            {
                // This most likely means socket closed
                Console.WriteLine("Read 0 bytes");
            }
        }

        private void OnPacketReady(ClientConnectionAttachment attachment)
        {
            Notification notification = Notification.DeSerialize(attachment.ReceivingPayloadBuffer);
            Console.WriteLine("Message: {0}\nCreated At {1}", notification.Message, notification.CreatedAt);
        }
    }
}