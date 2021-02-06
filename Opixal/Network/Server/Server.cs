using Opixal.Network.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Opixal.Network.Server
{
    internal class Server
    {
        private static TcpListener serverSocket;

        public static void InitializeNetwork(int port)
        {
            Console.WriteLine("Initializing Packets");
            ServerHandler.InitializePackets();

            serverSocket = new TcpListener(IPAddress.Any, port);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            Console.WriteLine("Server Started on {0}", serverSocket.LocalEndpoint);
        }

        private static void OnClientConnect(IAsyncResult asyncResult)
        {
            TcpClient client = serverSocket.EndAcceptTcpClient(asyncResult);
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            ClientObjectManager.CreateNewConnection(client);
        }
    }

    internal class ClientObject
    {
        public int connectionID;
        public TcpClient socket;
        public NetworkStream stream;
        private byte[] receiveBuffer;
        public ByteBuffer buffer;

        public void Start()
        {
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            stream = socket.GetStream();
            receiveBuffer = new byte[4096];
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            Console.WriteLine("Incoming connection from '{0}' received.", socket.Client.RemoteEndPoint);
        }

        private void OnReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                int length = stream.EndRead(asyncResult);
                if (length <= 0)
                {
                    CloseConnection();
                    return;
                }

                byte[] newBytes = new byte[length];
                Array.Copy(receiveBuffer, newBytes, length);
                ServerHandler.HandleData(connectionID, newBytes);
                stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            }
            catch (Exception)
            {
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine("Connection from '{0}' has been terminated.", socket.Client.RemoteEndPoint);
            socket.Close();
        }
    }

    internal static class ClientObjectManager
    {
        public static readonly Dictionary<int, ClientObject> client = new Dictionary<int, ClientObject>();

        public static void CreateNewConnection(TcpClient tempClient)
        {
            ClientObject newClient = new ClientObject();
            newClient.socket = tempClient;
            newClient.connectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port;
            newClient.Start();
            client.Add(newClient.connectionID, newClient);
            PacketSender.ServerOnSend(newClient.connectionID);
        }

        public static void SendDataTo(int connectionID, byte[] data)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
                buffer.WriteBytes(data);
                client[connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            }
        }
    }
}
