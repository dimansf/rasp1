﻿using Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Raspil
{
    public class RaspilServer
    {

        AppExchangeServer serv;
        string logName;

        public RaspilServer(string port, string logName = "server.log")
        {
            serv = new AppExchangeServer("127.0.0.1", port);
            this.logName = logName;
        }

        public void RunServer()
        {

            WriteLog("Сервер запущен. Ожидание подключений...");
            string result;
            while (true)
            {
                result = "";
                try
                {
                    serv.Accept();
                    // получили данные запроса 
                    var message = serv.GetData();

                    WriteLog(message);
                    

                    
                    serv.SendString("C1 - Принято в обработку");
                    result = FormResult(message);
                       
                    

                    serv.SendString(result);
                    serv.Close();
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                    serv.SendString(ex.Message);
                    serv.Close();
                    continue;
                }
            }

        }

        private string FormResult(string req)
        {
            // { order + store }
            dynamic data;
            try
            {
                data = JsonConvert.DeserializeObject(req);
            } catch
            {
                throw new Exception("Не удалось сбилдить json запроса / неправильный json");
            }


            int[][] order = TransformJArray((JArray)data.orders);
            int[][] store = TransformJArray((JArray)data.store);


            int widthSaw = (int)data.widthSaw;

            //добавим  число строк
            int x = 1;
            order = order.Select(el => { var e1 = el.ToList(); e1.Add(x++); return e1.ToArray(); }).ToArray();
           
           
            // начинаем простраивать карту распила
            var raspil = new RaspilOperator(order, store, widthSaw);

            List<string> xx = null;
            string[] zz = null;

            switch ((int)data.algoritm)
            {
                case 1:
                    {
                        xx = raspil.Algoritm1(); zz = raspil.Orders;
                        break;
                    }
                case 2:
                    {
                        xx = raspil.Algoritm2(); zz = raspil.Orders;
                        break;
                    }
                case 3:
                    {
                        xx = raspil.Algoritm3(); zz = raspil.Orders;
                        break;
                    }
                default:
                    return "{\"алгоритм\": \"Неизвестный алгоритм!\"}";
            }


            string raspilMap = JsonConvert.SerializeObject(xx);
            string ostk = JsonConvert.SerializeObject(zz);

            return $"{{ \"raspileMap\": {raspilMap},  \"ostatok\": {ostk} }}";
        }


        private  int[][] TransformJArray(JArray arr)
        {
           return arr.Select(jv => jv.Select(j => (int)j).ToArray()).ToArray();
        }
      
        private void WriteLog(string msg)
        {
            Console.WriteLine(msg);
            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), logName), DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : " + msg);
        }
       
    }
}
