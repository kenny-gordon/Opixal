﻿using Opixal.Network.Client;
using Opixal.Network.Server;
using System;

namespace Opixal
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            // Start a Server on Port 5000
            Server.InitializeNetwork(5000);

            // Connect to a Server at "localhost" on Port 5000
            Client.InitializeNetwork("localhost", 5000);

            Console.ReadLine();
        }

        #endregion Methods
    }
}
