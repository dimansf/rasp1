﻿using DataMock;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{
    public class RaspilServer
    {
        IPEndPoint ipPoint;
        Socket listenSocket;
        int countConnection;
        
        public int Port
        {
            get { return ipPoint.Port; }
            set { }
        }
        public RaspilServer(int port, int cc = 2)
        {
            ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            countConnection = cc;
        }
        
        public void runServer()
        {

            // связываем сокет с локальной точкой, по которой будем принимать данные
            listenSocket.Bind(ipPoint);

            // начинаем прослушивание
            listenSocket.Listen(countConnection);

            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            var connectionPool = new List<Socket>();
            
            while (true)
            {
                try
                {
                    connectionPool.Add(listenSocket.Accept());
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = connectionPool.Last().Receive(data);
                        builder.Append(Encoding.Unicode.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Unicode, data, 0, bytes), 0, bytes));
                    } while (connectionPool.Last().Available > 0);

                    //Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                  

                    // отправляем ответ
                    connectionPool.Last().Send(System.Text.Encoding.UTF8.GetBytes("Запрос получен. Ожидайте работу алгоритма"));

                    data = Encoding.Convert(Encoding.Unicode, Encoding.UTF8,  Encoding.Unicode.GetBytes(formResult(builder.ToString())));
                    connectionPool.Last().Send(data);
                    // закрываем сокет
                    connectionPool.Last().Shutdown(SocketShutdown.Both);
                    connectionPool.Last().Close();
                }
                catch (Exception ex)
                {
                    if(connectionPool.Count > countConnection)
                    {
                        foreach(var cn in connectionPool)
                        {
                            cn.Close();
                        }

                        connectionPool = new List<Socket>();
                        continue;
                    } else
                    {
                        Console.WriteLine(ex.Message);
                        connectionPool.Last().Close();
                        connectionPool.Remove(connectionPool.Last());
                        continue;
                    }
                }
            }

        }

        private string formResult(string req)
        {
            // ордеры + store
            dynamic data;
            try
            {
                data = JsonConvert.DeserializeObject(req);
            } catch
            {
                throw new Exception("Не удалось сбилдить json запроса / неправильный json");
            }


            int[][] ord = data.orders;
            int[][] store = data.store;

            // добавим  число строк
            var prom = ord.Select(el => el.ToList()).ToArray();
            int x = 0;
            prom.Select((el) => { el.Add(++x); return 0; }).ToArray();
            
            var orders = prom.Select(el => el.ToArray()).ToArray();


            //var (orders, store) = Data.autoGeneratedData(30, 60, 5);

            var raspil = new RaspilOperator(orders, store);
            var xx = raspil.Algoritm2();
            var zz = raspil.Orders;

            string raspilMap = JsonConvert.SerializeObject(xx);
            string ostk = JsonConvert.SerializeObject(zz);

            return $"{{ \"raspileMap\": {raspilMap},  \"ostatok\": {ostk} }}";
        }

      
       
    }
}
