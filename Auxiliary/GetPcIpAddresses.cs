using System;
using System.Net;

namespace NetworkSnippets.Auxiliary
{
    class GetPcIpAddresses
    {
        public static void Launch()
        {
            // How to retrieve all IP Addresses available in this PC
            // Without iterating through Network Interfaces which I showed in
            // another snippet (GetPcIpProperties)

            string strHostName = Dns.GetHostName();

            Console.WriteLine($"Host Name: {strHostName}");

            IPAddress[] ips = Dns.GetHostAddresses(strHostName);

            foreach (IPAddress ip in ips)
            {
                Console.WriteLine(ip);
            }
        }
    }
}