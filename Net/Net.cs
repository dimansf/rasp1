using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Net
{
    public class AppExchangeClient
    {
        private Socket listenSocket;
        string ip;
        string port;
       

        public AppExchangeClient(string ip, string port)
        {
            this.ip = ip; this.port = port;
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));

        }

        public AppExchangeClient Send(string st)
        {
            //Console.WriteLine(BitConverter.ToString(Encoding.UTF8.GetBytes(st)));
            Console.WriteLine(st.Count());
            listenSocket.Send(Encoding.UTF8.GetBytes(st));
            return this;
        }

        public string Receive()
        {
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[56000]; // буфер для получаемых данных

            do
            {
                bytes = listenSocket.Receive(data);
                builder.Append(Encoding.UTF8.GetString( data, 0, bytes));
                //builder.Append(Encoding.Unicode.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Unicode, data, 0, bytes), 0, bytes));
            } while (listenSocket.Available > 0);
           
            return builder.ToString();
        }
    }

    public class AppExchangeServer {


        private Socket listenSocket;
        public string ip { get; }
        public string port { get; }
        private Socket conns;


        public AppExchangeServer(string ip, string port, int numConns = 1)
        {
            this.ip = ip; this.port = port;

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // связываем сокет с локальной точкой, по которой будем принимать данные
            listenSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));

            listenSocket.Listen(numConns);
        }

        public void Accept()
        {
            conns =  listenSocket.Accept();
        }

        public string GetData()
        {
            
            // получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[56000]; // буфер для получаемых данных

            do
            {
                bytes = conns.Receive(data);
                builder.Append(Encoding.UTF8.GetString(data,  0, bytes));
                

            } while (conns.Available > 0);

            Console.WriteLine(builder.ToString().Count());
            return builder.ToString();
        }
        public void SendString(string sr)
        {
            conns.Send(Encoding.UTF8.GetBytes(sr));
        }

        public void Close()
        {
            conns.Close();
        }

    }
   
}
