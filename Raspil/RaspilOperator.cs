
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{

    public class RaspilOperator
    {
		
		/// <summary>
		/// Заказы которые нужно уместить в длинномерах
		///  [id, длина, число]
		/// </summary>
		private int[][] orders;
		/// <summary>
		/// Сами длинномеры
		/// [ид, длина, кол - во, ликвид, макс.обр, номер склада]
		/// </summary>
		int[][] store;
		/// <summary>
		/// Остаток заказов которые вернутся в качестве свойства обьекта
		/// </summary>
		public int[][]  ordersRemain = null;
		/// <summary>
		/// Толщина пила
		/// </summary>
        private int widthSaw = 4;
		/// <summary>
		/// Флаг для специфичного просчета, должен работать когда
		/// заказов одинаковое кол-во и все они входят в распил идеально
		/// </summary>
		private bool singleFlag = false;
		private bool optimize = false;
		private bool scladMax = false;
		/// <summary>
		/// Заданный массив порядка
		/// </summary>
		private (int[] shortMeasures, int[] longMeasures, int[] stock5, int[] stock4, int[] all, int[] stock3) preassignOrderArray = 
			( new[] { 1, 2 }, new[] { 3, 4, 5 }, new[] { 5 }, new[] { 4 }, new[] { 1,2,3,4,5 }, new[] { 3 });

		//private bool liqCond = false;
		public RaspilOperator( int[][] orders, int[][] store, int widhtSaw = 4, bool optimize=false, bool scladMax=false, bool singleFlag = false)
        {
            this.orders = orders;
            this.store = store.Where(el => el[2] > 0).ToArray();
            this.widthSaw = widhtSaw;
			this.singleFlag = singleFlag;
			this.optimize = optimize;
			this.scladMax = scladMax;
		}
		/// <summary>
		/// Публичная обертка для алгоритма 1
		/// </summary>
		/// <returns></returns>
		public List<string> Algoritm1()
		{
			return (List<string>)_Algoritm1();
		}
		/// <summary>
		/// Алгоритм 1
		/// Используются длинномеры по ликвидным условиям
		/// 
		/// Тогда без галки "склад" схема такая
		/// берутся палки с 3,4,5 складов, и начинает считать лучшую карту распила
		/// с галочкой "максимум склад" он начинает с 1,2 склада, когда ничего не будет подходить под условия,
		/// переключится на 3,4 склады, если и там окажется недостаточно, переключится на 5 склад
		/// , и окончательно составил полную карту
		/// </summary>
		/// <param name="intern"></param>
		/// <returns></returns>
		private object _Algoritm1(bool intern = false)
		{
			var raspileMap = new List<List<(int, OneStoreCombinations)>>();
			var k = 0;
			while (orders.Count() != 0)
			{
				k++;
				// 1 of 3
				try
				{	// 1,2 or 3,4,5
					raspileMap.Add(GetRaspileMap(scladMax ? preassignOrderArray.shortMeasures : preassignOrderArray.all));
					DoCut(raspileMap[k - 1]);


				}
				catch
				{
					// 2 of 3
					try
					{
						raspileMap.Add(GetRaspileMap(preassignOrderArray.stock3));
						DoCut(raspileMap[k - 1]);
					}
					catch
					{
						// 3 of 3
						try
						{
							raspileMap.Add(GetRaspileMap(preassignOrderArray.stock4));
							DoCut(raspileMap[k - 1]);
						}
						catch 
						{
							try
							{
								raspileMap.Add(GetRaspileMap(preassignOrderArray.stock5));
								DoCut(raspileMap[k - 1]);
							}
							catch (Exception ex) {
								NotifyAboutZeroRaspil(ex.Message);
								break;
							}
							
						}
					}
				}
				orders = orders.Where(el => el[0] > 0).ToArray();
			}

			if (intern)
			{
				return raspileMap;
			}
			var x = HumanReadableMapRaspil(raspileMap);
			return x;
		}
		/// <summary>
		/// Сначала пытаемся использовать обрезь с условиями ликвидности
		/// Если обрези не хватило на весь заказ, то
		/// переходим на Алгоритм 1
		/// </summary>
		/// <param name="liqCond"> Учитывать ликвидный остаток</param>
		/// <returns></returns>
		//public List<string> Algoritm2(bool liqCond = true)
		//{
		//	var raspileMap = new List<List<(int, OneStoreCombinations)>>();
		//	var k = 0;
		//	// 3,4,5 склады исключить для первого обсчета
		//	// 5 склад будет последним этапом для обсчета 
		//	var notSclads = new int[] { 3, 4, 5 };

		//	while (orders.Count() != 0)
		//	{
		//		//Console.WriteLine($"k = ${k}");
		//		//Console.WriteLine($"${orders.Length}");
		//		k++;
		//		try
		//		{
		//			raspileMap.Add(GetRaspileMap(liqCond, notSclads));
		//			DoCut(raspileMap[k - 1]);
		//		}
		//		catch
		//		{
		//			var alg1Map = (List<List<(int, OneStoreCombinations)>>)_Algoritm1(true);
		//			ExtendRaspilMap(raspileMap, alg1Map);
		//			break;
		//		}


		//		orders = orders.Where(el => el[0] > 0).ToArray();
		//	}
		//	var x = HumanReadableMapRaspil(raspileMap);
		//	return x;
		//}
		/// <summary>
		/// Сначала пытаемся использовать обрезь с условиями неликвидности
		/// Если обрези не хватило на весь заказ, то
		/// переходим на Алгоритм 1
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="store"></param>
		/// <param name="liqCond"></param>
		/// <returns></returns>
		//public List<string> Algoritm3(bool liqCond = false)
		//{
		//	return Algoritm2(liqCond);
		//}
		/// <summary>
		/// Сделать обновление для заказов и складов
		/// Произвести обрезь
		/// </summary>
		/// <param name="raspil">(ид доски, ((длина, ид склада), (остаток, распилы)))</param>
		/// <param name="orders"></param>
		private void DoCut(List<(int remain, OneStoreCombinations combinations)> raspil)
        {
			
            foreach (var rasp in raspil)
            {
                try
                {
					// (id, (len, Nsclad), (remain, list.lis)
					SubtractionOnSclad(rasp);
                    // вычитание палок с заказов
                    // ( ид строки, кол-во, длина)
                    foreach (var ors in rasp.combinations.combinations[0].list.lis)
                    {
                        foreach (var row in orders)
                        {
                            try
                            {
                                if (row[3] == ors.lineNumber)
                                    row[0] -= ors.amount;
                                if (row[0] < 0) throw new Exception("Отрицательное значение доски!");
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.WriteLine("В заказах недостаточно полей для корректной работы !");
                                continue;
                            }

                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        /// <summary>
        /// Вычитание палок со склада
        /// </summary>
        /// <param name="sclad"></param>
        /// <param name="scladId"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private void SubtractionOnSclad((int remain, OneStoreCombinations combs) element)
        {
			var (id, len, nSclad) = (element.remain, element.combs.lenght, element.combs.scladId);
			
            foreach (var row in store)
            {
                if (row[0] == id && 
					row[1] == len &&
					row[5] == nSclad)
                {
                    row[2] -= 1;
					break;
                   
                }
				if (row[2] < 0)
					throw new Exception("Longmeasure from store has negative amount (-1)  from SubtractionOnSclad");
			}
            
            store = store.Where(row => row[2] > 0).ToArray();
        }
        /// <summary>
        /// Вычленение лучших распилов
        /// </summary>
        /// <param name="palki"></param>
        /// <param name="notnumSclad"></param>
        /// <returns></returns>
        private OneStoreCombinations GetBestComparison(List<OneStoreCombinations> palki)
        {
            
            var goodLen = 0;
            var bestList = ( 0, combinations: new CustomList());
			var stickCount = 0;
            var bestPalka = (lenght: 0, scladid: 0);
			var lis = new List<(int, CustomList)>();

			//if (singleFlag)
			//{
			//	foreach (var palka in palki)
			//	{
			//		foreach (var combs in palka.combinations)
			//		{
			//			if (Array.IndexOf(notnumSclad, palka.scladId) == -1)
			//			{
							
			//				if (combs.list.singleFlag &&
			//					goodLen < palka.lenght - combs.remain ||
			//					goodLen == palka.lenght - combs.remain && bestPalka.lenght > palka.lenght)
			//				{
			//					// переписываем полезную нагрузку
			//					goodLen = palka.lenght - combs.remain;
			//					// лист комбинаций
			//					bestList = combs;
			//					// длина, номер склада
			//					bestPalka = (palka.lenght, palka.scladId);
			//				}

			//			}
			//		}
			//	}
			//	lis.Add(bestList);

			//	return new OneStoreCombinations(bestPalka.lenght, bestPalka.scladid, lis);
			//}
			//else {
				foreach (var palka in palki)
				{
					foreach (var combs in palka.combinations)
					{
					
					if (combs.list.GetCountItems() >= stickCount)
					{
						if (goodLen < palka.lenght - combs.remain ||
							goodLen == palka.lenght - combs.remain && bestPalka.lenght > palka.lenght ||
							goodLen == palka.lenght - combs.remain && bestPalka.scladid > palka.scladId)
						{
							// переписываем полезную нагрузку
							goodLen = palka.lenght - combs.remain;
							// лист комбинаций
							bestList = combs;
							// длина, номер склада
							bestPalka = (palka.lenght, palka.scladId);
						}
					}
					}
				}
            
            //}
			
			lis.Add(bestList);

			return new OneStoreCombinations(bestPalka.lenght, bestPalka.scladid, lis);

             
        }
        /// <summary>
        /// Получаем карту распила
        /// для одной итерации цикла
        /// </summary>
        /// <param name="liqCond"></param>
        /// <param name="sclads"></param>
        /// <returns></returns>
        private List<(int, OneStoreCombinations)> GetRaspileMap(int[] exceptionedStock=null)
        {
		

            var mapRaspil = new List<List<(int, (string, int, int))>>();

            var bt = new BagTasker();

            var unicalID = orders.Select(row => row[2]).Distinct().ToList();
								// id , лист комбинаций палок с этим id
            var res = new Dictionary<int, List<OneStoreCombinations>>();

			// обсчет для каждого id 
            unicalID.ForEach(id =>
            {
                res.Add(id, bt.Calculate(
				orders.Where(el => el[2] == id).ToArray(),
				store.Where(el => el[0] == id && Array.IndexOf(exceptionedStock, el[5]) != -1).ToArray(),
				getPercentParam(id),
				widthSaw,
				optimize
				));
			});

            //var ress = new List<(int, ((int, int), (int, CustomList)))>();
            var ress = new List<(int remain, OneStoreCombinations combs)>();

            foreach (var e in res)
            {
                ress.Add((e.Key, GetBestComparison(e.Value)));
            }
            var x = ress.Where(el => el.combs.scladId != 0).ToList();
            if (x.Count == 0)
            {
                throw new Exception("Кончились палки на складе длинномеров и заказов длинномеров");
            }
			
            return x;
        }

		private int getPercentParam(int id) {
			return store.Where(el => el[0] == id && el[1] > 5000).ToArray()[0][4];
		}
		
		/// <summary>
		/// Вызвать функцию вывода уведомления на консоль, что кончились палки на складе
		/// </summary>
		/// <param name="text"></param>
		private void NotifyAboutZeroRaspil(string text)
        {
            Console.WriteLine(text);

			//var li = new List<string>();
			//foreach(var or in orders)
			//{
			//    li.Add(String.Join(",", or));
			//}
			//ordersRemain = li.ToArray();
			ordersRemain = orders.Length  != 0 ? orders : null;
        }
		
        /// <summary>
        /// Расширение Карты распила другой картой
        /// </summary>
        /// <param name="main"></param>
        /// <param name="additionMap"></param>
        private void ExtendRaspilMap(List<List<(int, OneStoreCombinations)>> main, List<List<(int, OneStoreCombinations)>> additionMap)
        {
            foreach (var el in additionMap)
            {
                main.Add(el);
            }
        }
        /// <summary>
        /// Дружелюбное отображение карты распила
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public List<string> HumanReadableMapRaspil(List<List<(int remain, OneStoreCombinations combs)>> main)
        {
            // ид доски, длина, ид склада,  строка что в ней пилится, кол-во дублей
            var map = new List<((int id, int len, int sclaId), string doskiStr, int amount)>();
            foreach (var iterateOne in main)
            {
                foreach (var row in iterateOne)
                {
                    string n = "";
                    row.combs.combinations[0].list.lis.ForEach(el => n += $"({el.amount} * {el.lenght}) ");
                    map.Add(( (row.remain, row.combs.lenght, row.combs.scladId), n, row.combs.combinations[0].remain) );
                }
            }
            // склеивание дублирующихся распилов
            var newM = new Dictionary<((int, int, int), string, int), int>();
            foreach (var row in map)
            {
                if (newM.ContainsKey(row))
                {
                    newM[row] += 1;
                }
                else
                {
                    newM.Add(row, 1);
                }
            }
            // удобочитаемая строка распила
            var x = newM.Select(kp => (kp.Key, kp.Value)).ToList();
            var lis = new List<string>();
            //lis.Add($"(ид доски, длина, ид склада) | (строка распила) | кол-во таких распилов | остаток");
            foreach (var row in x)
            {
                var str = $"({row.Key.Item1.Item1}, {row.Key.Item1.Item2}, {row.Key.Item1.Item3}) | {row.Key.Item2} | {row.Value} | {row.Key.Item3}";
                lis.Add(str);
            }

            return lis;
        }
    }
}