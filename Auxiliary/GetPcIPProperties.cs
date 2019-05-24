using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace NetworkSnippets.Auxiliary
{
    class GetPcIpProperties
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

                    Console.WriteLine(networkInterface.Name);
                    Console.WriteLine("\tPhysical Address: {0}", physicalAddressString);
                    Console.WriteLine("\tOperational Status: {0}", networkInterface.OperationalStatus);
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    IPv4InterfaceProperties ipv4Properties = ipProperties.GetIPv4Properties();

                    UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;

                    foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
                    {
                        Console.WriteLine("\tUnicast Address: {0}", unicastAddress.Address);
                        Console.WriteLine("\tMask: {0}", unicastAddress.IPv4Mask);
                    }

                    IPAddressCollection dhcServerAddresses = ipProperties.DhcpServerAddresses;


                    foreach (IPAddress dhcServerAddress in dhcServerAddresses)
                    {
                        Console.WriteLine("\tDHCP Server Address: {0}", dhcServerAddress);
                    }

                    Console.WriteLine("\tIndex: {0}\n\tIsDhcpEnabled: {1}",
                        ipv4Properties.Index,
                        ipv4Properties.IsDhcpEnabled);

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