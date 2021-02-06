using Opixal.Network.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Opixal.Network.Server
{
    internal static class ServerHandler
    {
        public delegate void Packet(int connectionID, byte[] data);

        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        public static void InitializePackets()
        {
            packets.Add((int)PacketType.Handshake, PacketReceiver.ServerOnReceive);
        }

        public static void HandleData(int connectionID, byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();
            int pLength = 0;

            if (ClientObjectManager.client[connectionID].buffer == null)
            {
                ClientObjectManager.client[connectionID].buffer = new ByteBuffer();
            }

            ClientObjectManager.client[connectionID].buffer.WriteBytes(buffer);
            if (ClientObjectManager.client[connectionID].buffer.Count() == 0)
            {
                ClientObjectManager.client[connectionID].buffer.Clear();
                return;
            }

            if (ClientObjectManager.client[connectionID].buffer.Length() >= 4)
            {
                pLength = ClientObjectManager.client[connectionID].buffer.ReadInteger(false);
                if (pLength <= 0)
                {
                    ClientObjectManager.client[connectionID].buffer.Clear();
                    return;
                }
            }

            while (pLength > 0 && pLength <= ClientObjectManager.client[connectionID].buffer.Length() - 4)
            {
                if (pLength <= ClientObjectManager.client[connectionID].buffer.Length() - 4)
                {
                    ClientObjectManager.client[connectionID].buffer.ReadInteger();
                    data = ClientObjectManager.client[connectionID].buffer.ReadBytes(pLength);
                    HandleDataPackets(connectionID, data);
                }

                pLength = 0;
                if (ClientObjectManager.client[connectionID].buffer.Length() >= 4)
                {
                    pLength = ClientObjectManager.client[connectionID].buffer.ReadInteger(false);
                    if (pLength <= 0)
                    {
                        ClientObjectManager.client[connectionID].buffer.Clear();
                        return;
                    }
                }
            }

            if (pLength <= 1)
            {
                ClientObjectManager.client[connectionID].buffer.Clear();
            }
        }

        private static void HandleDataPackets(int connectionID, byte[] data)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteBytes(data);
                int PacketID = buffer.ReadInteger();

                if (packets.TryGetValue(PacketID, out Packet packet))
                {
                    packet.Invoke(connectionID, data);
                }
            }
        }
    }

    internal static class PacketReceiver
    {
        public static void ServerOnReceive(int connectionID, byte[] data)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteBytes(data);
                int packetID = buffer.ReadInteger(); // Not Used
                string message = buffer.ReadString();
                Console.WriteLine(message);
            }

            Thread.Sleep(1000); // remove this
            PacketSender.ServerOnSend(connectionID);
        }
    }

    internal static class PacketSender
    {
        public static void ServerOnSend(int connectionID)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)PacketType.Handshake);
                buffer.WriteString("Hello Client");
                ClientObjectManager.SendDataTo(connectionID, buffer.ToArray());
            }
        }
    }
}