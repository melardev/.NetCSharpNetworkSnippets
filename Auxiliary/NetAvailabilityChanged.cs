using System;
using System.Net.NetworkInformation;

namespace NetworkSnippets.Auxiliary
{
    class NetAvailabilityChanged
    {
        private static void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                Console.WriteLine("Network Is Available");
            }
            else
            {
                Console.WriteLine("Network Is Not Available");
            }
        }

        public static void Launch()
        {
            // Attach a listener to network availability changes
            NetworkChange.NetworkAvailabilityChanged +=
                new NetworkAvailabilityChangedEventHandler(NetworkAvailabilityChanged);
        }
    }
}