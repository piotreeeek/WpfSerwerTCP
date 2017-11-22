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
        const int BUFFER_SIZE = 1024;
        Socket clientSocket;
        bool running = true;
        byte[] messageByte = new byte[BUFFER_SIZE];
        string path = "D:\\politechnika\\semestr_7\\sieciowe\\przykladowe\\serwer";

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
                    //xx = messageByte.Clone() as byte[];
                    string message = Encoding.UTF8.GetString(sendByte);
                    if (message.Length == 0)
                    {
                        this.running = false;
                        Console.WriteLine("Klient " + this.clientSocket.RemoteEndPoint.ToString() + " rozłaczony.");
                        Serwer.AddStatement("Klient " + this.clientSocket.RemoteEndPoint.ToString() + " rozłaczony.\n");
                    }
                    else if(message.ToUpper() == "LIST")
                    {
                        Console.WriteLine("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message);
                        Serwer.AddStatement("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message + "\n");
                        string[] entries = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                        string list = "";
                        foreach(string entry in entries)
                        {
                            Console.WriteLine(Path.GetFileName(entry));
                            list = list + Path.GetFileName(entry) + "\n";

                        }
                        Byte[] listBytes = Encoding.UTF8.GetBytes(list);
                        s.Send(listBytes);
                    }
                    else if (message.Split(' ').First().ToUpper() == "GET")
                    {
                        Console.WriteLine("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message);
                        Serwer.AddStatement("Wiadomośc od klienta " + s.RemoteEndPoint.ToString() + " : " + message + "\n");
                        string fileName = path + "/" +  message.Split(' ')[1];

                        if (File.Exists(fileName)) 
                        {
                            byte[] buffer = null;
                            FileStream fs = new FileStream(fileName, FileMode.Open);
                            int bufferCount = Convert.ToInt32(Math.Ceiling((double)fs.Length / (double)BUFFER_SIZE));
                            string headerStr = "Content-length:" + fs.Length.ToString() + "\r\nFilename:" + Path.GetFileName(fileName) + "\r\n";
                            byte[] header = Encoding.UTF8.GetBytes(headerStr);
                            s.Send(header);

                            for (int i = 0; i < bufferCount; i++)
                            {
                                buffer = new byte[BUFFER_SIZE];
                                int size = fs.Read(buffer, 0, BUFFER_SIZE);
                                s.Send(buffer, size, SocketFlags.Partial);

                            }
                            Console.WriteLine("wysłano");
                            //FileStream file = File.Open(path + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                        else
                        {
                            Byte[] messageBytes = Encoding.UTF8.GetBytes("NO_FILE");
                            s.Send(messageBytes);
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