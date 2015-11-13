using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace Server
{
    class Server
    {
        static List<ClientData> Clients = new List<ClientData>();
        public static Socket listenerSocket;
       // static List<ClientData> _clients;
    




        static void Main(string[] args) 
        {
            Console.WriteLine("Starting Server...");


            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();



        } //Start server
          // Listener for Listening for Clients to Connect
          //Listens connections inside of Thread allows for multi-connections


        public static void ListenThread()
        {
           // List<ClientData> Clients;

            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int port = 5252;

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.1.141"), port); //Parse(Packet.getIPAddr()
            listenerSocket.Bind(ip);

            while (true)
            {
                listenerSocket.Listen(0);
                Server.Clients.Add(new ClientData(listenerSocket.Accept()));

            }
        }


        //Client Data thread to receive data from each client individually 

        public static void DataIn(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;
            byte[] Buffer;
            int readBytes;

            while (true)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet packet = new Packet(Buffer);
                        DataManager(packet);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Client Disconnected\r\n");
                }
            }
        }
        //data manager
        public static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Chat:
                    foreach(ClientData c in Clients)
                    {
                        c.clientSocket.Send(p.toBytes());
                    }
                    break;
            }
        }
    }



    class ClientData
    {
        public Socket clientSocket;
        public Thread clientThread;
        public string id;

        public ClientData()
        {
            //create unique User ID
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.DataIn);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();

        }
        public ClientData(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            clientThread = new Thread(Server.DataIn);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "server");
            p.getData.Add(id);

            clientSocket.Send(p.toBytes());
        }
    }
}
