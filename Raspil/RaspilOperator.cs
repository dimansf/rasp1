
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

        
        public RaspilOperator( int[][] orders, int[][] store, int widhtSaw = 4)
        {
            this.orders = orders;
            this.store = store.Where(el => el[2] > 0).ToArray();
            this.widthSaw = widhtSaw;
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
		/// </summary>
		/// <param name="intern"></param>
		/// <returns></returns>
		private object _Algoritm1(bool intern = false)
		{
			var raspileMap = new List<List<(int, ((int, int), (int, CustomList)))>>();
			var k = 0;
			while (orders.Count() != 0)
			{
				//Console.WriteLine($"k = ${k}");
				//Console.WriteLine($"${orders.Length}");
				k++;
				try
				{
					// исключить 5 склад из первого расчета, вторым этапом произвести обсчет
					raspileMap.Add(GetRaspileMap(true, new int[] { 1,2,5 }));
					DoCut(raspileMap[k - 1]);
				}
				catch
				{
					try
					{
						// если не найдено лучших распилов, значит нужно производить обсчет для палок с 5 склада
						// иначе они могут вытеснить менее полезные обрезки из 1-4 складов
						var only5Sclad = store.Where(el => el[5] == 5).ToArray();
						raspileMap.Add(GetRaspileMap(true, new int[] { 1, 2, 3, 4 }));
						DoCut(raspileMap[k - 1]);

						
					}
					catch(Exception ex)
					{
						NotifyAboutZeroRaspil(ex.Message);
						break;
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
		public List<string> Algoritm2(bool liqCond = true)
		{
			var raspileMap = new List<List<(int, ((int, int), (int, CustomList)))>>();
			var k = 0;
			// 3,4,5 склады исключить для первого обсчета
			// 5 склад будет последним этапом для обсчета 
			var notSclads = new int[] { 3, 4, 5 };

			while (orders.Count() != 0)
			{
				//Console.WriteLine($"k = ${k}");
				//Console.WriteLine($"${orders.Length}");
				k++;
				try
				{
					raspileMap.Add(GetRaspileMap(liqCond, notSclads));
					DoCut(raspileMap[k - 1]);
				}
				catch
				{
					var alg1Map = (List<List<(int, ((int, int), (int, CustomList)))>>)_Algoritm1(true);
					ExtendRaspilMap(raspileMap, alg1Map);
					break;
				}


				orders = orders.Where(el => el[0] > 0).ToArray();
			}
			var x = HumanReadableMapRaspil(raspileMap);
			return x;
		}
		/// <summary>
		/// Сначала пытаемся использовать обрезь с условиями неликвидности
		/// Если обрези не хватило на весь заказ, то
		/// переходим на Алгоритм 1
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="store"></param>
		/// <param name="liqCond"></param>
		/// <returns></returns>
		public List<string> Algoritm3(bool liqCond = false)
		{
			return Algoritm2(liqCond);
		}
		/// <summary>
		/// Сделать обновление для заказов и складов
		/// Произвести обрезь
		/// </summary>
		/// <param name="raspil">(ид доски, ((длина, ид склада), (остаток, распилы)))</param>
		/// <param name="orders"></param>
		private void DoCut(List<(int, ((int, int), (int, CustomList)))> raspil)
        {
			
            foreach (var rasp in raspil)
            {
                try
                {
					// (id, (len, Nsclad), (remain, list.lis)
					SubtractionOnSclad(rasp);
                    // вычитание палок с заказов
                    // ( ид строки, кол-во, длина)
                    foreach (var ors in rasp.Item2.Item2.Item2.lis)
                    {
                        foreach (var row in orders)
                        {
                            try
                            {
                                if (row[3] == ors.Item1)
                                    row[0] -= ors.Item2;
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
        private void SubtractionOnSclad((int, ((int, int), (int, CustomList))) element)
        {
			var (id, len, nSclad) = (element.Item1, element.Item2.Item1.Item1, element.Item2.Item1.Item2);
			
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
        private ((int, int), (int, CustomList)) GetBestComparison(List<((int, int), List<(int, CustomList)>)> palki, int[] notnumSclad = null)
        {
            notnumSclad = notnumSclad != null ? notnumSclad : new int[0];
            var goodLen = 0;
            var bestList = (0, new CustomList());
            var paramsPalki = (0, 0);

            // ([(откуда вычитать, сколько вычитать), ...], карта распила)
            // номер заказа, число палки
            var fromLi = new List<(int, int)>();
            foreach (var palka in palki)
            {
                foreach (var combs in palka.Item2)
                {
                    if (goodLen < palka.Item1.Item1 - combs.Item1 && Array.IndexOf(notnumSclad, palka.Item1.Item2) == -1 ||
                        goodLen == palka.Item1.Item1 - combs.Item1 && paramsPalki.Item1 > palka.Item1.Item1 && Array.IndexOf(notnumSclad, palka.Item1.Item2) == -1)
                    {
                        // переписываем полезную нагрузку
                        goodLen = palka.Item1.Item1 - combs.Item1;
                        // лист комбинаций
                        bestList = combs;
                        // длина, номер склада
                        paramsPalki = palka.Item1;

                    }
                }
            }


            return (paramsPalki, bestList);
        }
        /// <summary>
        /// Получаем карту распила
        /// для одной итерации цикла
        /// </summary>
        /// <param name="liqCond"></param>
        /// <param name="sclads"></param>
        /// <returns></returns>
        private List<(int, ((int, int), (int, CustomList)))> GetRaspileMap( bool liqCond = true, int[] sclads = null)
        {
		

            var mapRaspil = new List<List<(int, (string, int, int))>>();

            var bt = new BagTasker();

            var unicalID = orders.Select(row => row[2]).Distinct().ToList();
            var res = new Dictionary<int, List<((int, int), List<(int, CustomList)>)>>();

			// обсчет для каждого id 
            unicalID.ForEach(id =>
            {
                res.Add(id, bt.Calculate(
                orders.Where(el => el[2] == id).ToArray(),
                store.Where(el => el[0] == id).ToArray(),
                liqCond,
                widthSaw));
            });

            var ress = new List<(int, ((int, int), (int, CustomList)))>();

            foreach (var e in res)
            {
                ress.Add((e.Key, GetBestComparison(e.Value, sclads)));
            }
              var x = ress.Where(el => el.Item2.Item1 != (0, 0)).ToList();
            if (x.Count == 0)
            {
                throw new Exception("Кончились палки на складе длинномеров и заказов длинномеров");
            }
			
            return x;
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
        private void ExtendRaspilMap(List<List<(int, ((int, int), (int, CustomList)))>> main, List<List<(int, ((int, int), (int, CustomList)))>> additionMap)
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
        public List<string> HumanReadableMapRaspil(List<List<(int, ((int, int), (int, CustomList)))>> main)
        {
            // ид доски, длина, ид склада,  строка что в ней пилится, кол-во дублей
            var map = new List<((int, int, int), string, int)>();
            foreach (var iterateOne in main)
            {
                foreach (var row in iterateOne)
                {
                    string n = "";
                    row.Item2.Item2.Item2.lis.ForEach(el => n += $"({el.Item2} * {el.Item3}) ");
                    map.Add(((row.Item1, row.Item2.Item1.Item1, row.Item2.Item1.Item2), n, row.Item2.Item2.Item1));
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