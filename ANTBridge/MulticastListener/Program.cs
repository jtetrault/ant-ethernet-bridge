using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MulticastListener
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient client = new UdpClient();
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 2222);
            client.Client.Bind(localEp);

            IPAddress multicastAddress = IPAddress.Parse("239.0.0.222");
            client.JoinMulticastGroup(multicastAddress);

            while (true)
            {
                byte[] message = client.Receive(ref localEp);
                Console.WriteLine("Received " + BitConverter.ToString(message));
            }
        }
    }
}
