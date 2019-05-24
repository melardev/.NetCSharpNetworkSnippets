using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace NetworkSnippets.Sockets.Tcp.Async.BinarySerialization
{
    public class Client
    {
        // TODO: Reuse the write buffer

        public class Attachment
        {
            public Socket ClientSocket { get; set; }

            // Needed for Write operations
            public bool IsHeaderSent { get; set; } = false;
            public byte[] WriteHeaderBuffer { get; set; }
            public byte[] WritePayloadBuffer { get; set; }
            public ulong NumberOfBytesWrittenSoFar { get; set; }
            public ulong ExpectedWritePayloadLength { get; set; }
        }

        public Attachment AttachmentObject { get; set; }
        public Queue<Notification> PacketQueue { get; set; } = new Queue<Notification>();
        public bool IsAlreadySendingPacket { get; set; }

        public static void Launch()
        {
            Client client = new Client();
            client.Start();
        }

        public void Start()
        {
            Console.WriteLine("[+] Trying to connect, wait a moment please");

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(new IPEndPoint(IPAddress.Loopback, 3002), OnConnectionCallback, clientSocket);
        }

        public void OnConnectionCallback(IAsyncResult result)
        {
            AttachmentObject = new Attachment();
            AttachmentObject.ClientSocket = (Socket) result.AsyncState;
            AttachmentObject.ClientSocket.EndConnect(result);

            ListenCommandLine();
        }

        private void ListenCommandLine()
        {
            string line = "";
            do
            {
                line = Console.ReadLine();
                if (line == null)
                    break;

                Notification notification = new Notification
                {
                    Message = line,
                    CreatedAt = DateTime.Now,
                };

                SendNotificationThreadSafe(notification);
            } while (!line.Equals("quit", StringComparison.OrdinalIgnoreCase));

            AttachmentObject.ClientSocket.Shutdown(SocketShutdown.Both);
            AttachmentObject.ClientSocket.Close();
        }

        public void SendNotificationThreadSafe(Notification notification)
        {
            bool shouldScheduleAWriteOperation = false;
            lock (PacketQueue)
            {
                if (!IsAlreadySendingPacket)
                {
                    shouldScheduleAWriteOperation = true;
                    IsAlreadySendingPacket = true;
                }
                else
                {
                    PacketQueue.Enqueue(notification);
                }
            }

            if (shouldScheduleAWriteOperation)
            {
                SendNotificationUnsafe(notification);
            }
        }

        /// <summary>
        /// This should only be called when you are sure, there are no other packets already being sent
        /// and you have made the appropriate thread synchronization stuff, this method does not do that for you.
        /// </summary>
        /// <param name="notification"></param>
        private void SendNotificationUnsafe(Notification notification)
        {
            AttachmentObject.WritePayloadBuffer = notification.Serialize();
            AttachmentObject.ExpectedWritePayloadLength = (ulong) AttachmentObject.WritePayloadBuffer.LongLength;

            // The header buffer contains the payload size
            AttachmentObject.WriteHeaderBuffer = BitConverter.GetBytes(AttachmentObject.ExpectedWritePayloadLength);

            // Begin sending the header first
            AttachmentObject.ClientSocket.BeginSend(AttachmentObject.WriteHeaderBuffer,
                0,
                AttachmentObject.WriteHeaderBuffer.Length, SocketFlags.None, OnSendHeaderCallback, AttachmentObject);
        }

        public void OnSendHeaderCallback(IAsyncResult result)
        {
            Attachment attachment = (Attachment) result.AsyncState;
            AttachmentObject.NumberOfBytesWrittenSoFar += (ulong) AttachmentObject.ClientSocket.EndSend(result);

            if (AttachmentObject.NumberOfBytesWrittenSoFar < (ulong) AttachmentObject.WriteHeaderBuffer.LongLength)
            {
                // We have not finished sending the header, keep sending it
                AttachmentObject.ClientSocket.BeginSend(AttachmentObject.WriteHeaderBuffer,
                    (int) AttachmentObject.NumberOfBytesWrittenSoFar,
                    (int) ((ulong) AttachmentObject.WriteHeaderBuffer.LongLength -
                           AttachmentObject.NumberOfBytesWrittenSoFar),
                    SocketFlags.None, OnSendHeaderCallback, AttachmentObject);
            }
            else
            {
                // The header was sent completely
                AttachmentObject.IsHeaderSent = true;
                AttachmentObject.NumberOfBytesWrittenSoFar = 0;
                // We have sent the header, begin sending the payload now
                AttachmentObject.ClientSocket.BeginSend(AttachmentObject.WritePayloadBuffer,
                    0,
                    (int) AttachmentObject.ExpectedWritePayloadLength,
                    SocketFlags.None, OnSendPayloadCallback, AttachmentObject);
            }
        }

        private void OnSendPayloadCallback(IAsyncResult asyncResult)
        {
            Attachment attachment = (Attachment) asyncResult.AsyncState;
            AttachmentObject.NumberOfBytesWrittenSoFar += (ulong) AttachmentObject.ClientSocket.EndSend(asyncResult);

            if (AttachmentObject.NumberOfBytesWrittenSoFar < AttachmentObject.ExpectedWritePayloadLength)
            {
                // Write buffer has not been sent completely
                // send the remaining
                AttachmentObject.ClientSocket.BeginSend(AttachmentObject.WritePayloadBuffer,
                    (int) AttachmentObject.NumberOfBytesWrittenSoFar,
                    (int) (AttachmentObject.ExpectedWritePayloadLength - AttachmentObject.NumberOfBytesWrittenSoFar),
                    SocketFlags.None, OnSendHeaderCallback, AttachmentObject);
            }
            else
            {
                Console.WriteLine("Packet sent");

                // If there are more notifications to send then take one and send it
                // Thread synchronization is VERY important in Asynchronous socket programming, why?
                // what would happen if one thread is sending a Packet, and before finishing sending it
                // another thread is also sending a packet, you will end up merging two packets
                // and the server would not know what to do with that, if the server is poorly written it may even
                // crash
                IsAlreadySendingPacket = false;
                SendNotificationIfAvailable();
            }
        }

        private void SendNotificationIfAvailable()
        {
            Notification notification = null;
            lock (PacketQueue)
            {
                if (!IsAlreadySendingPacket && PacketQueue.Count > 0)
                {
                    notification = PacketQueue.Dequeue();
                    IsAlreadySendingPacket = true;
                }
            }

            if (notification != null)
                SendNotificationUnsafe(notification);
        }
    }
}