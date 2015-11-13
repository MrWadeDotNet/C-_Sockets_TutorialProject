using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerData
{   [Serializable]
    public class Packet
    {
        public List<string> getData;
        public int packetInt;
        public bool packetBool;
        public string senderID;
        public PacketType packetType;

        public Packet(PacketType type, string senderID)
        {
            getData = new List<string>();
            this.senderID = senderID;
            this.packetType = type;

        }

        public Packet(byte[] packetbytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetbytes);

            Packet p = (Packet)bf.Deserialize(ms);
            ms.Close();
            this.getData = p.getData;
            this.packetInt = p.packetInt;
            this.packetBool = p.packetBool;
            this.senderID = p.senderID;

        }

        public byte[] toBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            ms.Close();

            return bytes;
        }

        public static string getIPAddr()
        {
            IPAddress[] ip = Dns.GetHostAddresses(Dns.GetHostName());

            // ERROR ip returning APIPA address 
            // Need to loop over ip Array and pick out addresses that start with 192 

            string localhost = "127.0.0.1";
            //filter IPv4 foreach 
            foreach (IPAddress i in ip)
                {
                    if(i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                           {
                              return i.ToString();
                            }
                   }
            return localhost;

        }
    }

    public enum PacketType
    {
        Registration,
        Chat

    }
}
