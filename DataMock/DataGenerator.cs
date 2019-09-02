﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataMock
{
	public static class Data
	{
		/*	[22, 800, 1],
			[18, 500, 1],
			[8, 1700, 2],
			[3, 2100, 5],
			[10, 1400, 1],
			[12, 800, 2],
			[30, 1600, 1],
			[9, 760, 2],
			[10, 5600, 3],
			[4, 760, 3]*/



		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static int[][][] staticData() {
			var orders = new int[][] {
				new []{ 22, 800, 400, 1 },
				new []{ 18, 500, 130, 2 },
				new []{8, 1700, 90, 3 },
				new []{3, 2100, 90, 4 },
				new []{10, 1400, 40, 5 },
				new []{12, 800, 400, 6 },
				new []{30, 1600, 90, 7 },
				new []{13, 900, 130, 8 },
				new []{10, 5600, 90, 9 },
				new []{4, 760, 40, 10 },
				new []{8, 840, 130, 11 },
				new []{9, 540, 400, 12 },
			};
			// [ид, длина, кол-во, ликвид, макс. обр, номер склада ]
			var globStore = new int[][] {
				new [] { 400, 1550, 30, 500, 1},
				new [] { 400, 700, 10, 700, 1},
				new [] { 400, 550, 10, 500, 2},
				new [] { 400, 2100, 13, 500, 2},
				new [] { 130, 2400, 7, 600, 1},
				new [] { 90, 1400, 11, 600, 1},
				new [] { 90, 900, 1, 600, 1},
				new [] { 90, 700, 8, 500, 1},
				new [] { 90, 1300, 4, 500, 2},
				new [] { 90, 3300, 3, 500, 2},
				new [] { 40, 1300, 3, 500, 1},
				new [] { 40, 700, 3, 500, 1},
				new [] { 40, 600, 3, 500, 1},
				new [] { 40, 1600, 3, 500, 1},
				new [] { 40, 6000, 20, 700, 3},
				new [] { 40, 6000, 20, 700, 3},
				new [] { 130, 6000, 10, 700, 3},
				new [] { 90, 6000, 2, 700, 3},
				new [] { 90, 6000, 2, 700, 3},
			};
			return new[] { orders, globStore };
		}

		private static int[] id = new[] { 40, 400, 130, 90, 70, 88, 99, 77, 78, 79, 72 };


		public static int[][] GeneratedData2FroFile(string filepath){
		{
				dynamic data;
				try
				{
					data = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "input.txt")));
				}
				catch
				{
					throw new Exception("Не удалось сбилдить json запроса / неправильный json");
				}
				
				JArray arr = (JArray)data;
				return arr.Select(jv => jv.Select(j => (int)j).ToArray()).ToArray();

		}

		public static (int[][], int[][]) autoGeneratedData2(int lenOrders = 30, int lenStores = 60, int minLenObr = 5, bool withoutLongMeasures = false, bool noNumLines = false)
		{

			var orders = new List<int[]>();
			var store = new List<int[]>();

			var rand = new Random();

			var lords = rand.Next(20, lenOrders);

			// генерация заявок
			for (int i = 0; i < lords; i++)
			{   // [число палок, длина палок, ид палок]
				if (noNumLines)
					orders.Add(new int[] { rand.Next(1, 20), rand.Next(minLenObr, 22) * 100, id[rand.Next(100) % id.Length] });
				else
					orders.Add(new int[] { rand.Next(1, 20), rand.Next(minLenObr, 22) * 100, id[rand.Next(100) % id.Length], i + 1 });

			}
			//склад будущих длинномеров 
			for (int i = 0; i < lenStores * 5; i++)
			{   //[ид, длина, кол-во, ликвид, макс. обр, номер склада ]
				store.Add(new int[] {
					id[rand.Next(100) %  id.Length],
					6000 - rand.Next(0,1) * 500,
					900,
					rand.Next(4, 7) * 100,
					rand.Next(3, 7),
					5
				});
			}

			return (orders.ToArray(), store.ToArray());
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="lenOrders"></param>
		/// <param name="lenStores"></param>
		/// <returns></returns>
		public static (int[][], int[][]) autoGeneratedData(int lenOrders = 30, int lenStores = 60, int minLenObr = 5, bool withoutLongMeasures = false, bool noNumLines = false)
		{
			

			var orders = new List<int[]>();
			var store = new List<int[]>();

			var rand = new Random();

			var lords = rand.Next(20, lenOrders);

			// генерация заявок
			for (int i = 0; i < lords; i++)
			{	// [число палок, длина палок, ид палок]
                if(noNumLines)
				    orders.Add(new int[] { rand.Next(1,20), rand.Next(minLenObr, 22)*100, id[rand.Next(100) % id.Length]});
                else
				    orders.Add(new int[] { rand.Next(1,20), rand.Next(minLenObr, 22)*100, id[rand.Next(100) % id.Length], i + 1 });

			}

			// генерация склада
			// склад обрези
			for (int i = 0; i < lenStores/5; i++)
			{   //[ид, длина, кол-во, ликвид, макс. обр, номер склада ]
				store.Add(new int[] {
					id[rand.Next(100) %  id.Length],
					rand.Next(4, 45) * 100,
					rand.Next(10,20),
					rand.Next(4, 7) * 100,
					rand.Next(3, 7),
					1
				});
			}
			
			 //склад будущей обрези 
			for (int i = 0; i < lenStores/5; i++)
			{   //[ид, длина, кол-во, ликвид, макс. обр, номер склада ]
				store.Add(new int[] {
					id[rand.Next(100) %  id.Length],
					rand.Next(4, 45) * 100,
					rand.Next(1,20),
					rand.Next(4, 7) * 100,
					rand.Next(3, 7),
					2
				});
			}
            if(!withoutLongMeasures)
            {

          
			//склад длинномеров  
			for (int i = 0; i < lenStores-5; i++)
			{   //[ид, длина, кол-во, ликвид, макс. обр, номер склада ]
				store.Add(new int[] {
					id[rand.Next(100) %  id.Length],
					6000 - rand.Next(0,1) * 500,
					rand.Next(5,20),
					rand.Next(4, 7) * 100,
					rand.Next(3, 7),
					3
				});
			}
			//склад будущих длинномеров 
			for (int i = 0; i < lenStores-5; i++)
			{   //[ид, длина, кол-во, ликвид, макс. обр, номер склада ]
				store.Add(new int[] {
					id[rand.Next(100) %  id.Length],
					6000 - rand.Next(0,1) * 500,
					rand.Next(5,20),
					rand.Next(4, 7) * 100,
					rand.Next(3, 7),
					4
				});
			}
            }
            return (orders.ToArray(), store.ToArray() );
		}
	}



    public class ClientData
    {
        public int [][] orders;
        public int[][] store;
        public int widthSaw;
        public int algoritm;

    }
}
