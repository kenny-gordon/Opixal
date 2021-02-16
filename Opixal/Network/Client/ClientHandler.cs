using Opixal.Network.Shared;
using System.Collections.Generic;
using System.Threading;

namespace Opixal.Network.Client
{
    internal static class ClientHandler
    {
        #region Fields

        public static readonly Dictionary<int, Packet> packets = new Dictionary<int, Packet>();
        private static ByteBuffer clientBuffer;

        #endregion Fields

        #region Delegates

        public delegate void Packet(byte[] data);

        #endregion Delegates

        #region Methods

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

        public static void InitializePackets()
        {
            packets.Add((int)PacketType.Handshake, PacketReceiver.ClientOnReceive);
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

        #endregion Methods
    }

    internal static class PacketReceiver
    {
        #region Methods

        public static void ClientOnReceive(byte[] data)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteBytes(data);
                int packetID = buffer.ReadInteger(); // Not Used
                string message = buffer.ReadString();
                //Console.WriteLine(message);
                Global.LogManager.Log(new Logging.LogEntry(Logging.LoggingEventType.DEBUG, message));
            }

            Thread.Sleep(1000); // remove this
            PacketSender.ClientOnSend();
        }

        #endregion Methods
    }

    internal static class PacketSender
    {
        #region Methods

        public static void ClientOnSend()
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)PacketType.Handshake);
                buffer.WriteString($"Client Sent a Package of type {PacketType.Handshake}");
                Client.SendData(buffer.ToArray());
            }
        }

        #endregion Methods
    }
}
