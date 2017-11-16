using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace WpfSerwerTCP
{
    internal class ClientServerTheard
    {
        Socket clientSocket;
        bool running = true;
        byte[] messageByte = new byte[1024];

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