using Opixal.Network.Client;
using Opixal.Network.Server;
using System;

namespace Opixal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Server.InitializeNetwork(5000);
            Client.InitializeNetwork("localhost", 5000);

            Console.ReadLine();
        }
    }
}