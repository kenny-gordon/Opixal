using Opixal.Network.Shared;
using System;
using System.Net.Sockets;

namespace Opixal.Network.Client
{
    internal static class Client
    {
        private static TcpClient clientSocket;
        private static NetworkStream clientStream;
        private static byte[] receiveBuffer;

        public static void InitializeNetwork(string serverAddr, int serverPort)
        {
            ClientHandler.InitializePackets();
            ClientConnect(serverAddr, serverPort);
        }

        public static void ClientConnect(string serverAddr, int serverPort)
        {
            clientSocket = new TcpClient();
            clientSocket.ReceiveBufferSize = 4096;
            clientSocket.SendBufferSize = 4096;
            receiveBuffer = new byte[4096 * 2];
            clientSocket.BeginConnect(serverAddr, serverPort, new AsyncCallback(ClientConnectCallback), clientSocket);
        }

        private static void ClientConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                clientSocket.EndConnect(asyncResult);
                if (clientSocket.Connected)
                {
                    clientSocket.NoDelay = true;
                    clientStream = clientSocket.GetStream();
                    clientStream.BeginRead(receiveBuffer, 0, 4096 * 2, ReceiveCallback, null);
                    PacketSender.ClientOnSend();
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                int bytesRead = clientStream.EndRead(asyncResult);
                if (bytesRead <= 0)
                {
                    return;
                }

                byte[] newBytes = new byte[bytesRead];
                Array.Copy(receiveBuffer, newBytes, bytesRead);
                ClientHandler.HandleData(newBytes);
                clientStream.BeginRead(receiveBuffer, 0, 4096 * 2, ReceiveCallback, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SendData(byte[] data)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0) + 1));
                buffer.WriteBytes(data);
                clientStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            }
        }

        public static void Disconect()
        {
            clientSocket.Close();
        }
    }
}