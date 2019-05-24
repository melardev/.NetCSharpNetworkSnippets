using System;
using System.Net;

namespace NetworkSnippets.Auxiliary
{
    class DnsGetHostEntryUsage
    {
        public static void Launch()
        {
            string hostname = "google.com";
            try
            {
                IPAddress[] addresses = Dns.GetHostEntry(hostname).AddressList;
                Console.WriteLine("Resolution for {0}", hostname);

                foreach (IPAddress address in addresses)
                {
                    Console.WriteLine("{0}", address);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: {0}", exception.Message);
            }
        }
    }
}