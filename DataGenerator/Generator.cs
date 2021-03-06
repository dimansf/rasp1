﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DataGenerator
{

	/// <summary>
	/// Контейнер для генерации
	/// 1 - int[][] заказов и складов
	/// 2 - json-string заказов и складов
	/// </summary>
	public class Generator

	{
		/// <summary>
		/// Перечень id для генерации склада и заказов
		/// Расшифровка для ЗАКАЗОВ
		/// [ кол-во, длина, ид ]
		/// Расшифровка Для СКЛАДОВ
		/// [ ид, длина, кол-во, ликвид, макс. обр, номер склада ]
		///
		/// </summary>
		private static int[] id = new[] { 40, 400, 130, 90, 70, 88, 99, 77, 78, 79, 72 };

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string case1 = "{ \"optimize\": true, \"orders\": [ [         8021,             3500,             1         ]     ],     \"scladMax\": true,      \"store\": [    		[              8021,              6000,              10000,              500,              4,              5          ]      ],      \"widthSaw\": 3  }  ";
		public static string case2 = "{\"optimize\":true,\"orders\":[[9607,825,44],[9027,825,1],[64,825,1],[1033,825,1],[106,890,1],[2367,1200,30],[3359,1935,2]],\"scladMax\":true,\"store\":[[1033,2916,1,310,4,2],[1033,311,1,310,4,1],[1033,313,1,310,4,1],[1033,314,4,310,4,1],[1033,317,1,310,4,1],[1033,319,2,310,4,1],[1033,322,2,310,4,1],[1033,331,1,310,4,1],[1033,334,1,310,4,1],[1033,335,1,310,4,1],[1033,337,1,310,4,1],[1033,338,2,310,4,1],[1033,340,1,310,4,1],[1033,343,1,310,4,1],[1033,347,1,310,4,1],[1033,348,3,310,4,1],[1033,352,1,310,4,1],[1033,353,2,310,4,1],[1033,354,1,310,4,1],[1033,355,1,310,4,1],[1033,357,1,310,4,1],[1033,358,2,310,4,1],[1033,359,1,310,4,1],[1033,363,1,310,4,1],[1033,364,1,310,4,1],[1033,366,1,310,4,1],[1033,368,1,310,4,1],[1033,371,1,310,4,1],[1033,374,1,310,4,1],[1033,375,1,310,4,1],[1033,376,1,310,4,1],[1033,377,2,310,4,1],[1033,378,5,310,4,1],[1033,380,1,310,4,1],[1033,384,1,310,4,1],[1033,385,2,310,4,1],[1033,386,1,310,4,1],[1033,388,2,310,4,1],[1033,390,1,310,4,1],[1033,392,1,310,4,1],[1033,393,1,310,4,1],[1033,398,1,310,4,1],[1033,404,1,310,4,1],[64,407,1,1000,4,1],[1033,416,1,310,4,1],[1033,417,2,310,4,1],[1033,419,2,310,4,1],[1033,423,2,310,4,1],[1033,425,2,310,4,1],[1033,428,1,310,4,1],[1033,429,3,310,4,1],[1033,433,4,310,4,1],[1033,435,1,310,4,1],[1033,436,1,310,4,1],[1033,439,2,310,4,1],[1033,444,1,310,4,1],[1033,445,1,310,4,1],[1033,446,1,310,4,1],[1033,447,1,310,4,1],[1033,449,24,310,4,1],[1033,455,1,310,4,1],[1033,462,2,310,4,1],[1033,464,1,310,4,1],[1033,466,2,310,4,1],[1033,475,1,310,4,1],[1033,485,1,310,4,1],[1033,491,2,310,4,1],[1033,492,1,310,4,1],[1033,499,1,310,4,1],[1033,502,2,310,4,1],[1033,503,1,310,4,1],[1033,504,1,310,4,1],[64,508,2,1000,4,1],[1033,508,1,310,4,1],[1033,509,1,310,4,1],[64,514,1,1000,4,1],[1033,518,1,310,4,1],[1033,520,1,310,4,1],[1033,538,1,310,4,1],[1033,542,1,310,4,1],[1033,555,1,310,4,1],[1033,582,1,310,4,1],[1033,618,1,310,4,1],[1033,697,1,310,4,1],[2367,885,3,2000,4,1],[9027,1063,1,1000,4,1],[106,1343,1,1300,4,1],[64,1387,1,1000,4,1],[106,1402,1,1300,4,1],[106,1424,1,1300,4,1],[106,1443,1,1300,4,1],[64,1454,1,1000,4,1],[64,1468,1,1000,4,1],[64,1606,1,1000,4,1],[3359,1809,1,1300,4,1],[3359,2050,1,1300,4,1],[64,2128,1,1000,4,1],[3359,2319,28,1300,4,1],[3359,2394,2,1300,4,1],[106,2402,1,1300,4,1],[3359,2434,4,1300,4,1],[106,2631,1,1300,4,1],[106,2676,1,1300,4,1],[9027,2904,1,1000,4,1],[106,3054,1,1300,4,1],[9027,3230,1,1000,4,1],[9027,3252,1,1000,4,1],[2367,5200,15,2000,4,1],[64,6000,12,1000,4,4],[64,1500,0,1000,4,3],[3359,5000,0,1300,4,3],[64,6000,5,1000,4,3],[106,6000,3,1300,4,3],[1033,6000,9,310,4,3],[2367,6000,0,2000,4,3],[3359,6000,6,1300,4,3],[9027,6000,4,1000,4,3],[9607,6000,75,2000,4,3],[2367,6010,0,2000,4,3],[64,6000,10000,1000,4,5],[3359,6000,10000,1300,4,5],[64,6000,10000,1000,4,5],[106,6000,10000,1300,4,5],[1033,6000,10000,310,4,5],[2367,6000,10000,2000,4,5],[3359,6000,10000,1300,4,5],[9027,6000,10000,1000,4,5],[9607,6000,10000,2000,4,5],[2367,6000,10000,2000,4,5]],\"widthSaw\":3}";
		/// Реальны пример из vorotasuper
			/// 
			/// </summary>
			/// <returns></returns>
		public static (string, string) getOrderStore(int variant = 3)
		{
			var orders = new string[] {
				 "{ order:[ [ 9025, 771, 5 ], [ 64, 771, 5 ], [ 9607, 771, 200 ], [ 99, 836, 5 ], [ 3359, 1780, 10 ] ]}",
				 "{ order: [ [ 9025, 771, 5 ], [ 64, 771, 5 ], [ 9607, 771, 200 ], [ 99, 836, 5 ], [ 3359, 1780, 10 ] ] }",
			/*специфичный заказ*/	 "{ order: [ [ 3411, 350, 12 ], [ 3411, 360, 12 ], [ 3411, 390, 12 ], [ 3411, 1276, 12 ], [ 3411, 1400, 6 ], [ 3411, 1600, 6 ], [ 3411, 2000, 6 ], [ 3411, 2200, 6 ]]}"

		};
			var stores = new string[] {
				"{store: [[ 3359, 1065, 1, 1300, 4, 1 ], [ 9607, 1194, 2, 1000, 4, 1 ], [ 9025, 1215, 2, 1000, 4, 1 ], [ 9025, 1287, 1, 1000, 4, 1 ], [ 64, 1312, 1, 1000, 4, 1 ], [ 99, 1364, 1, 1300, 4, 1 ], [ 99, 1434, 1, 1300, 4, 1 ], [ 64, 1500, 0, 1000, 4, 4 ], [ 99, 2029, 1, 1300, 4, 1 ], [ 9607, 2250, 1, 1000, 4, 1 ], [ 3359, 2434, 9, 1300, 4, 1 ], [ 9025, 3240, 0, 1000, 4, 4 ], [ 9025, 4602, 1, 1000, 4, 1 ], [ 3359, 4790, 1, 1300, 4, 1 ], [ 3359, 5000, 0, 1300, 4, 4 ], [ 64, 6000, 15, 1000, 4, 4 ], [ 99, 6000, 3, 1300, 4, 4 ], [ 3359, 6000, 7, 1300, 4, 4 ], [ 9025, 6000, 4, 1000, 4, 4 ], [ 9607, 6000, 37, 1000, 4, 4 ], [ 64, 1500, 10000, 1000, 4, 5 ], [ 9025, 3240, 10000, 1000, 4, 5 ], [ 3359, 5000, 10000, 1300, 4, 5 ], [ 64, 6000, 10000, 1000, 4, 5 ], [ 99, 6000, 10000, 1300, 4, 5 ], [ 3359, 6000, 10000, 1300, 4, 5 ], [ 9025, 6000, 10000, 1000, 4, 5 ], [ 9607, 6000, 10000, 1000, 4, 5 ] ]}",
				"{store: [ [     64,    383,    1,    1000,    4,    1        ],        [    64,    407,    1,    1000,    4,    1        ],        [    64,    501,    1,    1000,    4,    1        ],        [    64,    508,    2,    1000,    4,    1        ],        [    64,    514,    1,    1000,    4,    1        ],        [    99,    948,    1,    1300,    4,    1        ],        [    9025,    1215,    2,    1000,    4,    1        ],        [    9025,    1287,    1,    1000,    4,    1        ],        [    3359,    1371,    1,    1300,    4,    1        ],        [    99,    1434,    1,    1300,    4,    1        ],        [    64,    1500,    0,    1000,    4,    4        ],        [    3359,    2434,    4,    1300,    4,    1        ],        [    9025,    3240,    0,    1000,    4,    4        ],        [    3359,    5000,    0,    1300,    4,    4        ],        [    64,    6000,    10,    1000,    4,    4        ],        [    99,    6000,    0,    1300,    4,    4        ],        [    3359,    6000,    7,    1300,    4,    4        ],        [    9025,    6000,    5,    1000,    4,    4        ],        [    9607,    6000,    40,    1000,    4,    4        ],        [    64,    1500,    10000,    1000,    4,    5        ],        [    9025,    3240,    10000,    1000,    4,    5        ],        [    3359,    5000,    10000,    1300,    4,    5        ],        [    64,    6000,    10000,    1000,    4,    5        ],        [    99,    6000,    10000,    1300,    4,    5        ],        [    3359,    6000,    10000,    1300,    4,    5        ],        [    9025,    6000,    10000,    1000,    4,    5        ],        [    9607,    6000,    10000,    1000,    4,    5        ]    ] }",
				"{store: [ [ 3411, 6000, 12, 1200, 4, 4 ], [ 3411, 6000, 10000, 1200, 4, 5 ]]}"
			};
			//var orders1 = "{ order:[ [ 9025, 771, 5 ], [ 64, 771, 5 ], [ 9607, 771, 200 ], [ 99, 836, 5 ], [ 3359, 1780, 10 ] ]}";
			//var store1 = "{ store: [[ 3359, 1065, 1, 1300, 4, 1 ], [ 9607, 1194, 2, 1000, 4, 1 ], [ 9025, 1215, 2, 1000, 4, 1 ], [ 9025, 1287, 1, 1000, 4, 1 ], [ 64, 1312, 1, 1000, 4, 1 ], [ 99, 1364, 1, 1300, 4, 1 ], [ 99, 1434, 1, 1300, 4, 1 ], [ 64, 1500, 0, 1000, 4, 4 ], [ 99, 2029, 1, 1300, 4, 1 ], [ 9607, 2250, 1, 1000, 4, 1 ], [ 3359, 2434, 9, 1300, 4, 1 ], [ 9025, 3240, 0, 1000, 4, 4 ], [ 9025, 4602, 1, 1000, 4, 1 ], [ 3359, 4790, 1, 1300, 4, 1 ], [ 3359, 5000, 0, 1300, 4, 4 ], [ 64, 6000, 15, 1000, 4, 4 ], [ 99, 6000, 3, 1300, 4, 4 ], [ 3359, 6000, 7, 1300, 4, 4 ], [ 9025, 6000, 4, 1000, 4, 4 ], [ 9607, 6000, 37, 1000, 4, 4 ], [ 64, 1500, 10000, 1000, 4, 5 ], [ 9025, 3240, 10000, 1000, 4, 5 ], [ 3359, 5000, 10000, 1300, 4, 5 ], [ 64, 6000, 10000, 1000, 4, 5 ], [ 99, 6000, 10000, 1300, 4, 5 ], [ 3359, 6000, 10000, 1300, 4, 5 ], [ 9025, 6000, 10000, 1000, 4, 5 ], [ 9607, 6000, 10000, 1000, 4, 5 ] ]}";

			//var orders2 = "{ order: [ [     9025,     771,     5 ], [     64,     771,     5 ], [     9607,     771,     200 ], [     99,     836,     5 ], [     3359,     1780,     10 ]     ] }";
			//var store2 = "{store: [ [     64,    383,    1,    1000,    4,    1        ],        [    64,    407,    1,    1000,    4,    1        ],        [    64,    501,    1,    1000,    4,    1        ],        [    64,    508,    2,    1000,    4,    1        ],        [    64,    514,    1,    1000,    4,    1        ],        [    99,    948,    1,    1300,    4,    1        ],        [    9025,    1215,    2,    1000,    4,    1        ],        [    9025,    1287,    1,    1000,    4,    1        ],        [    3359,    1371,    1,    1300,    4,    1        ],        [    99,    1434,    1,    1300,    4,    1        ],        [    64,    1500,    0,    1000,    4,    4        ],        [    3359,    2434,    4,    1300,    4,    1        ],        [    9025,    3240,    0,    1000,    4,    4        ],        [    3359,    5000,    0,    1300,    4,    4        ],        [    64,    6000,    10,    1000,    4,    4        ],        [    99,    6000,    0,    1300,    4,    4        ],        [    3359,    6000,    7,    1300,    4,    4        ],        [    9025,    6000,    5,    1000,    4,    4        ],        [    9607,    6000,    40,    1000,    4,    4        ],        [    64,    1500,    10000,    1000,    4,    5        ],        [    9025,    3240,    10000,    1000,    4,    5        ],        [    3359,    5000,    10000,    1300,    4,    5        ],        [    64,    6000,    10000,    1000,    4,    5        ],        [    99,    6000,    10000,    1300,    4,    5        ],        [    3359,    6000,    10000,    1300,    4,    5        ],        [    9025,    6000,    10000,    1000,    4,    5        ],        [    9607,    6000,    10000,    1000,    4,    5        ]    ] }";
			if (variant == 1)
			{
				return (orders[0], stores[0]);
			}
			if (variant == 2)
			{
				return (orders[1], stores[1]);
			}
			if (variant == 3)
			{
				return (orders[2], stores[2]);
			}
			return (null, null);
		}
	
	
		public static (int[][], int[][]) GenerateJsonOrderStore(int variant=1)
		{

			
			dynamic data1, data2;
			try
			{
				var (or, st) = getOrderStore(variant);
				data1 = JsonConvert.DeserializeObject(or);
				data2 = JsonConvert.DeserializeObject(st);
			}
			catch
			{
				throw new Exception("Не удалось сбилдить json запроса / неправильный json");
			}

			 var orderA = ((JArray)data1.order).Select(jv => jv.Select(j => (int)j).ToArray()).ToArray();
			 var storeA = ((JArray)data2.store).Select(jv => jv.Select(j => (int)j).ToArray()).ToArray();

			return (orderA, storeA);

		}

		//public static int[][] GenerateFromJsonFile(string path)
		//{
		//}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lenOrders"> Число элементолв в заказе</param>
		/// <param name="lenStores"> Число элементов для каждого склада, всего *4 элементов</param>
		/// <param name="minLenObr">Минимальная обрещь в процентах</param>
		/// <param name="longShortFlag">Флаг, генерировать ли длинномеры или только обрезки</param>
		/// <param name="noNumLines">Без нумерации строк заказов</param>
		/// <returns></returns>
		/// 
			public static (int[][], int[][]) AutoGeneratedData2(int lenOrders = 30, int lenStores = 60,
				int minLenObr = 5, int longShortFlag = 1, bool noNumLines = false)
			{

				var orders = new List<int[]>();
				var store = new List<int[]>();

				var rand = new Random();

				var lords = rand.Next(20, lenOrders);
				
				// order
				// генерация заказов
				for (int i = 0; i < lords; i++)
				{   // [число палок, длина палок, ид палок]
					if (noNumLines)
						orders.Add(new int[] { rand.Next(1, 20), rand.Next(minLenObr, 22) * 100,
							id[rand.Next(100) % id.Length] });
					else
						orders.Add(new int[] { rand.Next(1, 20), rand.Next(minLenObr, 22) * 100,
							id[rand.Next(100) % id.Length], i + 1 });

				}
			// store
			
			if (longShortFlag == 1) {

				// 1
				// склад обрези
				for (int i = 0; i < lenStores / 5; i++)
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

				// 2
				// склад будущей обрези
				for (int i = 0; i < lenStores / 5; i++)
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
			}

			if (longShortFlag == 2)
			{
				// 3 
				// склад длинномеров
				for (int i = 0; i < lenStores - 5; i++)
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
			
				// 4 
				//склад будущих длинномеров
				for (int i = 0; i < lenStores - 5; i++)
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
				{   // [число палок, длина палок, ид палок]
					if (noNumLines)
						orders.Add(new int[] { rand.Next(1, 20), rand.Next(minLenObr, 22) * 100, id[rand.Next(100) % id.Length] });
					else
						orders.Add(new int[] { rand.Next(1, 20), rand.Next(minLenObr, 22) * 100, id[rand.Next(100) % id.Length], i + 1 });

				}

				// генерация склада
				// склад обрези
				for (int i = 0; i < lenStores / 5; i++)
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
				for (int i = 0; i < lenStores / 5; i++)
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
				if (!withoutLongMeasures)
				{


					//склад длинномеров  
					for (int i = 0; i < lenStores - 5; i++)
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
					for (int i = 0; i < lenStores - 5; i++)
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
				return (orders.ToArray(), store.ToArray());
			}
		}



		public class ClientData
		{
			public int[][] orders;
			public int[][] store;
			public int widthSaw;
			
			public bool scladMax;
			public bool optimize;

		}

		public class JsonPack
		{
			public static string packGeneratedData(int[][] order, int[][] store)
			{
				var jw = new JsonWrapper
				{
					order = order,
					store = store
				};

				string ord = JsonConvert.SerializeObject(jw);


				var x = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
				File.WriteAllText(x, ord);
				return x;

			}
		}

		public class JsonWrapper
		{
			public int[][] order;
			public int[][] store;
		}
	}

