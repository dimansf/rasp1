using Newtonsoft.Json;
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
		public static (int[][], int[][]) staticData()
		{
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
			var store = new int[][] {
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
			return ( orders, store );
		}

		/// <summary>
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
			public int algoritm;
			public bool singleFlag;

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

