
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
		private OrderList orders;
		/// <summary>
		/// Сами длинномеры
		/// [ид, длина, кол - во, ликвид, макс.обр, номер склада]
		/// </summary>
		private StoreList store;
		/// <summary>
		/// Остаток заказов которые вернутся в качестве свойства обьекта
		/// </summary>
		public OrderList ordersRemain;
		/// <summary>
		/// Толщина пила
		/// </summary>
		private readonly int widthSaw = 4;


		private readonly bool optimize = false;
		private readonly bool scladMax = false;
		/// <summary>
		/// Заданный массив порядка
		/// </summary>
		private (int[] shortMeasures, int[] longMeasures, int[] stock5, int[] stock4, int[] all, int[] stock3)
			preassignOrderArray = (
				new[] { 1, 2 },
				new[] { 3, 4, 5 },
				new[] { 5 },
				new[] { 4 },
				new[] { 1, 2, 3, 4, 5 },
				new[] { 3 });


		public RaspilOperator(int[][] orders, int[][] store, int widthSaw = 4, bool optimize = false, bool scladMax = false)
		{
			this.orders = new OrderList(orders);
			this.store = new StoreList(store.Where(el => el[2] > 0).ToArray());
			this.ordersRemain =  new OrderList();
			this.widthSaw = widthSaw;

			this.optimize = optimize;
			this.scladMax = scladMax;
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
		public List<string> Algoritm1()
		{
			var raspileMap = new RaspilMap();

			RaspilMapCircle(raspileMap, scladMax ? preassignOrderArray.shortMeasures : preassignOrderArray.all);
			RaspilMapCircle(raspileMap, preassignOrderArray.stock3);
			RaspilMapCircle(raspileMap, preassignOrderArray.stock4);
			RaspilMapCircle(raspileMap, preassignOrderArray.stock5);



			if (orders.Count() != 0)
				ordersRemain = orders;


			//var x = HumanReadableMapRaspil(raspileMap);
			return new List<string>();
		}

		private void RaspilMapCircle(RaspilMap raspileMap, int[] stocks)
		{
			while (orders.Count() != 0)
			{
				var (flag, res) = GetRaspileMap(stocks);

				if (flag)
				{
					res.Select(kp =>
					{
						raspileMap.Add(kp.Key, kp.Value);
						orders.Subtract(kp.Value);
						store.Subtract(kp.Value);
						return 0;
					});
				}
				else
				{
					break;
				}

			}
		}

		
		/*/// <summary>
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
				if (row.id == id &&
					row.len == len &&
					row.numRepos == nSclad)
				{
					row.count -= 1;
					break;

				}
				if (row.count < 0)
					throw new Exception("Longmeasure from store has negative amount (-1)  from SubtractionOnSclad");
			}

			store = new StoreList(store.Where(row => row.count > 0));
		}
		/// <summary>
		/// Вычленение лучших распилов
		/// </summary>
		/// <param name="palki"></param>
		/// <param name="notnumSclad"></param>
		/// <returns></returns>
		private OneStoreCombinations GetBestComparison(List<OneStoreCombinations> palki)
		{

			var goodLen = 0.0;
			var bestList = (0, combinations: new CustomList());

			var bestPalka = (lenght: 0, scladid: 0);
			var lis = new List<(int, CustomList)>();


			foreach (var palka in palki)
			{
				foreach (var combs in palka)
				{
					// выключим условие по числу палок
					var currPersentage = Math.Round(Convert.ToDouble(((double)palka.lenght - (double)combs.remain) / (double)palka.lenght), 3);
					if (goodLen < currPersentage ||
						goodLen == currPersentage && bestPalka.lenght < palka.lenght ||
						goodLen == currPersentage && bestPalka.scladid > palka.scladId)
					{
						// переписываем полезную нагрузку
						goodLen = currPersentage;
						// лист комбинаций
						bestList = combs;
						// длина, номер склада
						bestPalka = (palka.lenght, palka.scladId);
					}

				}
			}

			//}

			lis.Add(bestList);

			return new OneStoreCombinations(bestPalka.lenght, bestPalka.scladid, lis);


		}*/
		/// <summary>
		/// Получаем карту распила
		/// для одной итерации цикла
		/// </summary>

		/// <returns></returns>

		private (bool flag, Dictionary<int, OneBoardCombinations> result) GetRaspileMap(int[] exceptionedStock = null)
		{

			var unicalID = orders.Select(board => board.id).Distinct().ToList();

			// id , лист комбинаций палок с этим id

			var bestComparisons = new Dictionary<int, OneBoardCombinations>();

			// обсчет для каждого id 
			unicalID.ForEach(id =>
			{
				var bagTasker = new BagTasker(
					orders.Where(el => el.id == id),
					store.Where(el => el.id == id && Array.IndexOf(exceptionedStock, el.numRepos) != -1),
					widthSaw,
					optimize);

				OneBoardCombinations boardComb = null;
				bagTasker.Calculate().Select(boardCombinations =>
				{
					var res = boardCombinations.getBestOneBoardCombination();
					if (boardComb == null)
						boardComb = res;
					if (boardComb.getBestPercantage() < res.getBestPercantage())
					{
						boardComb = res;
					}
					return 0;
				});

				if (boardComb != null)
					bestComparisons.Add(id, boardComb);

			});




			if (bestComparisons.Count == 0)
			{
				return (false, bestComparisons);
			}

			return (true, bestComparisons);
		}



		/// <summary>
		/// Вызвать функцию вывода уведомления на консоль, что кончились палки на складе
		/// </summary>
		/// <param name="text"></param>
		private void NotifyAboutZeroRaspil(string text)
		{
			Console.WriteLine(text);


			ordersRemain = orders.Count() != 0 ? orders : null;
		}

		/*/// <summary>
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
					row.combs[0].list.ForEach(el => n += $"({el.amount} * {el.lenght}) ");
					map.Add(((row.remain, row.combs.lenght, row.combs.scladId), n, row.combs[0].remain));
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
		}*/
	}


}