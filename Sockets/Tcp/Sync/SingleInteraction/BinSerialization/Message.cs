using System;

namespace NetworkSnippets.Sockets.Tcp.Sync.SingleInteraction.BinSerialization
{
    [Serializable]
    class Message
    {
        public double UserId { get; set; }
        public string UserName { get; set; }
        public string Contents { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}