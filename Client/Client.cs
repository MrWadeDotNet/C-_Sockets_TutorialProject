using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using ServerData;

namespace Client
{
    class Client
    {
        public static Socket master;
        public static string userName;
        public static string id;

       


        static void Main(string[] args)
        {
        A: Console.Clear();

            Console.WriteLine("Please enter a user name: ");
            userName = Console.ReadLine();
            Console.WriteLine("Enter host IP address: ");
            string ip = Console.ReadLine();
          //  Console.WriteLine("Enter Port: ");
        //    string port = Console.ReadLine();


             master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            IPEndPoint ipAddr = new IPEndPoint(IPAddress.Parse(ip), 5252);

            try {
                master.Connect(ipAddr);
            }
            catch
            {
                Console.WriteLine("Unable to connect to remote host\r\n");
                Thread.Sleep(1000);
                goto A;
            }
            //Start thread
            Thread t = new Thread(DataIn);
            t.Start();

            while (true)
            {
                Console.WriteLine("::>");
                string input = Console.ReadLine();

                Packet p = new Packet(PacketType.Chat, id);
                p.getData.Add(userName);
                p.getData.Add(input);
                master.Send(p.toBytes());

            }
        }



        static void DataIn()
        {
            byte[] Buffer;
            int readBytes;
         

            while (true)
            {
                try     
                {
                
                   Buffer = new byte[master.SendBufferSize];
                   readBytes = master.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        DataManager(new Packet(Buffer));
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Server disconnected!");
                }
            }
        }
        static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Registration:
                    id = p.getData[0];
                    break;
                case PacketType.Chat:
                    ConsoleColor c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkBlue;

                    Console.WriteLine(p.getData[0] + " : " + p.getData[1]);
                    Console.ForegroundColor = c;
                    break;
            }
        
        }

    }
}
