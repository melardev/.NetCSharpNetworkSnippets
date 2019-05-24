using System;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.SerializationBytes
{
    [Serializable]
    class Message
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}