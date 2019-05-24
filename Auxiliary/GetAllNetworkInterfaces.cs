using System;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace NetworkSnippets.Auxiliary
{
    class GetAllNetworkInterfaces
    {
        public static void Launch()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();

                    // Add : each two numbers
                    string physicalAddressString = Regex.Replace(physicalAddress.ToString(), ".{2}", "$0:");

                    // Remove trailing ":"; from 20:10:BB: to 20:10:BB
                    if (physicalAddressString.Length > 0)
                        physicalAddressString = physicalAddressString.Substring(0, physicalAddressString.Length - 1);

                    Console.WriteLine("Physical Address: {0}", physicalAddressString);
                    Console.WriteLine("Operational Status: {0}", networkInterface.OperationalStatus);
                    Console.Out.Flush();
                }
            }
            else
            {
                Console.WriteLine("No network interfaces available on your System.");
            }
        }
    }
}