using Opixal.Logging;
using Opixal.Network.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Opixal.Network.Server
{
    internal static class ClientObjectManager
    {
        #region Fields

        public static readonly Dictionary<int, ClientObject> client = new Dictionary<int, ClientObject>();

        #endregion Fields

        #region Methods

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

        #endregion Methods
    }

    internal class ClientObject
    {
        #region Fields

        public ByteBuffer buffer;
        public int connectionID;
        public TcpClient socket;
        public NetworkStream stream;
        private byte[] receiveBuffer;

        #endregion Fields

        #region Methods

        public void Start()
        {
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            stream = socket.GetStream();
            receiveBuffer = new byte[4096];
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);

            //Console.WriteLine("Incoming connection from '{0}' received.", socket.Client.RemoteEndPoint);
            Global.LogManager.LogInfo(message: $"Incoming connection from {socket.Client.RemoteEndPoint} received.", type: typeof(ClientObject));
        }

        private void CloseConnection()
        {
            //Console.WriteLine("Connection from '{0}' has been terminated.", socket.Client.RemoteEndPoint);
            Global.LogManager.LogInfo(message: $"Connection from {socket.Client.RemoteEndPoint} has been terminated.", type: typeof(ClientObject));

            socket.Close();
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

        #endregion Methods
    }

    internal class Server
    {
        #region Fields

        private static TcpListener serverSocket;

        #endregion Fields

        #region Methods

        public static void InitializeNetwork(int port)
        {
            //Console.WriteLine("Initializing Packets");
            Global.LogManager.LogInfo(message: "Initializing Packets", type: typeof(Server));
            ServerHandler.InitializePackets();

            serverSocket = new TcpListener(IPAddress.Any, port);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            //Console.WriteLine("Server Started on {0}", serverSocket.LocalEndpoint);
            Global.LogManager.LogInfo(message: $"Server Started on {serverSocket.LocalEndpoint}", type: typeof(Server));
        }

        private static void OnClientConnect(IAsyncResult asyncResult)
        {
            TcpClient client = serverSocket.EndAcceptTcpClient(asyncResult);
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            ClientObjectManager.CreateNewConnection(client);
        }

        #endregion Methods
    }
}
