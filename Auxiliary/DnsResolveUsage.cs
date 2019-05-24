using System;
using System.Net;

namespace NetworkSnippets.Auxiliary
{
    class DnsResolveUsage
    {
        public static void Launch()
        {
            IPHostEntry result = Dns.Resolve("localhost");
            foreach (var address in result.AddressList)
                Console.WriteLine(address);
        }
    }
}