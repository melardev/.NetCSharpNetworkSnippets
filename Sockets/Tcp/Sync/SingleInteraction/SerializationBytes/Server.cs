using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.SerializationBytes
{
    class Server
    {
        public static void Launch()
        {
            TcpListener serverListener = new TcpListener(IPAddress.Loopback, 3002);
            serverListener.Start();

            Socket clientSock = serverListener.AcceptSocket();

            byte[] packetLengthBytes = new byte[sizeof(ulong)];
            clientSock.Receive(packetLengthBytes);

            ulong packetLength = BitConverter.ToUInt64(packetLengthBytes, 0);

            // Create a buffer that may hold that packet
            byte[] clientData = new byte[packetLength];

            ulong receivedBytesLen = 0;

            // Keep reading up to the point where we have read the whole packet
            // For this snippet we rely blindly on what the client told us about
            // the packetLength, in real apps we must check if its valid(not big, nor <= 0)
            while (receivedBytesLen < packetLength)
            {
                receivedBytesLen = receivedBytesLen + (ulong) clientSock.Receive(clientData,
                                       (int) (receivedBytesLen),
                                       (int) (packetLength - receivedBytesLen),
                                       SocketFlags.None
                                   );
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream(clientData);

            // Notice the difference, in other snippet we used Deserialize(NetworkStream)
            // That took care of knowing how much bytes we are gonna receive for the Object
            // But now we use MemoryStream with a fix sized byte array as a backend
            // We prepared the byte array according to the size that the client told us
            Message message = (Message) binaryFormatter.Deserialize(memoryStream);

            Console.WriteLine("[Server] Received From Client a Message that got deserialized successfully");
            Console.WriteLine("From: {0}\nSubject: {1}\nBody: {2}", message.From,
                message.Subject, message.Body);

            serverListener.Stop();
            clientSock.Close();
            memoryStream.Close();
        }
    }
}