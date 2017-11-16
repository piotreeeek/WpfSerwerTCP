using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WpfSerwerTCP
{
    class Serwer
    {
        Socket s;
        ArrayList clientList;

        public Serwer()
        {
            
        }

        public bool Start(int port)
        {
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientList = new ArrayList();
            try
            {

                Console.WriteLine(port);
                s.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
            }
            catch (SocketException)
            {
                return false;
            }
            s.Listen(5);
            Thread workThread = new Thread(this.Work);
            workThread.Start();
            return true;
        }

        public void Stop()
        {
            s.Close();
            foreach (Socket cs in clientList)
            {
                cs.Close();
            }

            
        }

        public void Work()
        {
            for (; ; )
            {
                try
                {
                    Socket cli = s.Accept();
                    clientList.Add(cli);
                    ClientServerTheard clientServerTheard = new ClientServerTheard(cli);
                    Thread thread = new Thread(clientServerTheard.Run);
                    thread.Start();
                    Serwer.ModifyClientCount(1);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
        }

        public static void ModifyClientCount(int i)
        {
            MainWindow.main.ModifyClientCount(i);
        }

        public static void AddStatement(string text)
        {
            MainWindow.main.AddStatement(text);
        }


        //    s.Listen(5);

        //    int counter = 0;
        //    for (; ; )
        //    {
        //        Socket cli = s.Accept();
        //        clientList.Add(cli);
        //        ClientServerTheard clientServerTheard = new ClientServerTheard(cli);
        //        Thread thread = new Thread(clientServerTheard.Run);
        //        thread.Start();
        //    }
        //}
    }
}

