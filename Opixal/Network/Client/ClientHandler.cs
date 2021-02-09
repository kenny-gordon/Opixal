using Opixal.Network.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Opixal.Network.Client
{
    internal static class ClientHandler
    {
        private static ByteBuffer clientBuffer;

        public delegate void Packet(byte[] data);

        public static readonly Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        public static void InitializePackets()
        {
            packets.Add((int)PacketType.Handshake, PacketReceiver.ClientOnReceive);
        }

        public static void HandleData(byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();
            int pLength = 0;

            if (clientBuffer == null)
            {
                clientBuffer = new ByteBuffer();
            }

            clientBuffer.WriteBytes(buffer);
            if (clientBuffer.Count() == 0)
            {
                clientBuffer.Clear();
                return;
            }

            if (clientBuffer.Length() >= 4)
            {
                pLength = clientBuffer.ReadInteger(false);
                if (pLength <= 0)
                {
                    clientBuffer.Clear();
                    return;
                }
            }

            while (pLength > 0 && pLength <= clientBuffer.Length() - 4)
            {
                if (pLength <= clientBuffer.Length() - 4)
                {
                    clientBuffer.ReadInteger();
                    data = clientBuffer.ReadBytes(pLength);
                    HandleDataPackets(data);
                }

                pLength = 0;
                if (clientBuffer.Length() >= 4)
                {
                    pLength = clientBuffer.ReadInteger(false);
                    if (pLength <= 0)
                    {
                        clientBuffer.Clear();
                        return;
                    }
                }
            }

            if (pLength <= 1)
            {
                clientBuffer.Clear();
            }
        }

        private static void HandleDataPackets(byte[] data)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteBytes(data);
                int PacketID = buffer.ReadInteger();

                if (packets.TryGetValue(PacketID, out Packet packet))
                {
                    packet.Invoke(data);
                }
            }
        }
    }

    internal static class PacketReceiver
    {
        public static void ClientOnReceive(byte[] data)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteBytes(data);
                int packetID = buffer.ReadInteger(); // Not Used
                string message = buffer.ReadString();
                Console.WriteLine(message);
            }

            Thread.Sleep(1000); // remove this
            PacketSender.ClientOnSend();
        }
    }

    internal static class PacketSender
    {
        public static void ClientOnSend()
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)PacketType.Handshake);
                buffer.WriteString("Hello Server");
                Client.SendData(buffer.ToArray());
            }
        }
    }
}