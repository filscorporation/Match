﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetworkShared.Network;

namespace Assets.Scripts.Match.Networking
{
    /// <summary>
    /// Player connected to the server
    /// </summary>
    public class Client : IConnectionHandler
    {
        public string IP = "192.168.0.14";
        public int Port = 26950;
        private int myId = 0;
        public TCPConnection ClientTCP;
        public NetworkManager Manager;

        public Client(NetworkManager manager)
        {
            ClientTCP = new TCPConnection(this);
            Manager = manager;
        }

        public void ConnectToServer()
        {
            ClientTCP.Connect(IP, Port);
        }

        public void Disconnect()
        {
            ClientTCP.Disconnect();
        }

        public void SendData(int type, object data)
        {
            byte[] bytes = null;
            if (data != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    binaryFormatter.Serialize(stream, data);
                    bytes = stream.ToArray();
                }
            }

            using (Packet packet = new Packet(type))
            {
                if (bytes != null)
                    packet.SetBytes(bytes);
                packet.WriteLength();
                ClientTCP.SendPacket(packet);
            }
        }

        public void ReadPacket(int type, Packet packet)
        {
            byte[] bytes = packet.ReadAllBytes();
            object result = null;

            if (bytes != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    result = binaryFormatter.Deserialize(stream);
                }
            }

            Manager.ClientReceiveData(type, result);
        }

        public void OnDisconnect()
        {

        }
    }
}
