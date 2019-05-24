using System;
using System.Net;

namespace NetworkSnippets.Auxiliary
{
    class AnyAddress
    {
        public static void Launch()
        {
            // Any means All interfaces, This is considered a security issue,
            // If you create a server you wanna make sure where do you bind it
            // If you server needs only to be bound to localhost then use IPAddress.Loopback
            // Using IPAddress means your server will be available from all Network interaces
            // available in your PC
            IPAddress anyAddress = IPAddress.Any;
            Console.WriteLine("The Any Address is: {0}", anyAddress.ToString());
        }
    }
}