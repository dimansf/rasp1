using Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Raspil
{
	public class RaspilServer
	{

		AppExchangeServer serv;
		string logName;

		public RaspilServer(string port, string logName = "RaspilServer.log")
		{
			serv = new AppExchangeServer("127.0.0.1", port, 2);
			this.logName = logName;
		}

		public void RunServer()
		{

			Console.WriteLine("Сервер запущен. Ожидание подключений...");
			
			while (true)
			{
				
				try
				{
					var newConn = serv.Accept();
					var threadObject = new Runnable(newConn, serv);
					var th = new Thread(new ThreadStart(threadObject.Run));

					th.Start();
					
				}
				catch (Exception ex)
				{
					WriteLog(ex.Message);
					continue;
				}
			}

			
		}

		private void WriteLog(string msg)
		{
			Console.WriteLine(msg);
			File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), logName), DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : " + msg);
		}

		private class Runnable
		{
			Socket clientSocket;
			AppExchangeServer serv;
			public Runnable(Socket s, AppExchangeServer ss) { serv = ss; clientSocket = s; }


			public void Run()
			{
				//Console.WriteLine(((IPEndPoint)clientSocket.LocalEndPoint).Address + " " + 
				//	((IPEndPoint)clientSocket.LocalEndPoint).Port);
				Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
				// получили данные запроса 
				var message = serv.GetData(clientSocket);

				//WriteLog(message);



				serv.SendString("100\n\r Начинаем просчет комбинаций", clientSocket);
				var result = FormResult(message);



				serv.SendString(result,  clientSocket);
				serv.Close( clientSocket);
			}

			public string FormResult(string req)
			{
				// { order + store }
				dynamic data;
				try
				{
					data = JsonConvert.DeserializeObject(req);
				}
				catch
				{
					throw new Exception("Не удалось сбилдить json запроса / неправильный json");
				}


				int[][] order = TransformJArray((JArray)data.orders);
				int[][] store = TransformJArray((JArray)data.store);


				var widthSaw = (int)data.widthSaw;
				bool optimize, scladMax;
				try
				{
					optimize = (bool)data.optimize;
					scladMax = (bool)data.scladMax;
				}
				catch
				{
					optimize = false;
					scladMax = false;
				}


				////добавим  число строк
				//int x = 1;
				//order = order.Select(el =>
				//{
				//	var t = el[0];
				//	el[0] = el[2];
				//	el[2] = t;
				//	var e1 = el.ToList();
				//	e1.Add(x++);
				//	return e1.ToArray();
				//}).ToArray();


				// начинаем простраивать карту распила
				var raspil = new RaspilOperator(order, store, widthSaw, optimize, scladMax);

				List<string> result = null;
				//string[] zz = null;
				OrderList remain = null;
				long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				result = raspil.Algoritm1();

				Console.WriteLine((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - milliseconds).ToString() + "ms");
				// если остаток есть, отрежем последние элементы(это нумерация строк)
				remain = raspil.ordersRemain != null ? 
					raspil.ordersRemain : 
					null;


				// карта распила построенная по принятым доскам склада
				string raspilMap = JsonConvert.SerializeObject(result);
				// остаток списка всех заказов
				string remainMap = JsonConvert.SerializeObject(remain);

				return $"{{ \"RaspilMap\": {raspilMap},  \"Remain\": {remainMap} }}";
			}


			private int[][] TransformJArray(JArray arr)
			{
				return arr.Select(jv => jv.Select(j => (int)j).ToArray()).ToArray();
			}

			


		}

	}
	 
}
