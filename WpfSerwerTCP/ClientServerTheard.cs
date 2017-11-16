using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WpfSerwerTCP
{
    internal class ClientServerTheard
    {
        Socket clientSocket;
        bool running = true;
        byte[] messageByte = new byte[1024];
        string path = "D:/politechnika/semestr_7/sieciowe/przykladowe";

        public ClientServerTheard(Socket s)
        {
            clientSocket = s;
        }

        public void Run()
        {
            Socket s = this.clientSocket;
            Console.WriteLine("Nowe połączenie z " + s.RemoteEndPoint.ToString());
            Serwer.AddStatement("Nowe połączenie z " + s.RemoteEndPoint.ToString() + "\n");
            try
            {
                while (this.running)
                {
                    int length = s.Receive(messageByte);
                    byte[] sendByte = new List<byte>(messageByte).GetRange(0, length).ToArray();
                    string message = Encoding.UTF8.GetString(sendByte);
                    if (message.Length == 0)
                    {
                        this.running = false;
                        Console.WriteLine("Klient " + this.clientSocket.RemoteEndPoint.ToString() + " rozłaczony.");
                        Serwer.AddStatement("Klient " + this.clientSocket.RemoteEndPoint.ToString() + " rozłaczony.\n");
                    }
                    else if(message.ToUpper() == "LIST")
                    {
                        s.Send(sendByte);
                        Console.WriteLine("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message);
                        Serwer.AddStatement("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message + "\n");
                        string[] entries = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                        string list = "";
                        foreach(string entry in entries)
                        {
                            Console.WriteLine(Path.GetFileName(entry));
                            list = list + Path.GetFileName(entry) + "\n";

                        }

                    }
                    else if (message.Split(' ').First().ToUpper() == "SHOW")
                    {
                        s.Send(sendByte);
                        Console.WriteLine("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message);
                        Serwer.AddStatement("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message + "\n");
                        string fileName = message.Split(' ')[1];

                        if (File.Exists(path + "/" + fileName)) 
                        {
                            byte[] fileBytes = File.ReadAllBytes(path + "/" + fileName);
                            StringBuilder sb = new StringBuilder();

                            foreach (byte b in fileBytes)
                            {
                                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                            }
                            Console.WriteLine(sb);
                            //FileStream file = File.Open(path + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }

                        string[] entries = Directory.GetFiles("D:/politechnika/semestr_7/sieciowe/przykladowe", "*", SearchOption.TopDirectoryOnly);
                        foreach (string entry in entries)
                        {
                            Console.WriteLine(Path.GetFileName(entry));
                            Console.WriteLine("dupa");
                        }
                    }
                    else
                    {
                        s.Send(sendByte);
                        Console.WriteLine("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message);
                        Serwer.AddStatement("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message + "\n");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Klient " + this.clientSocket.RemoteEndPoint.ToString() + " rozłaczony.");
                Serwer.AddStatement("Klient " + this.clientSocket.RemoteEndPoint.ToString() + " rozłaczony.\n");
            }
            try
            {
                this.clientSocket.Close();
                Serwer.ModifyClientCount(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}