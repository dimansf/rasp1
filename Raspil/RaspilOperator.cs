
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
		private readonly int countOfOrders = 0;

		/// <summary>
		/// Сами длинномеры
		/// [ид, длина, кол - во, ликвид, макс.обр, номер склада]
		/// </summary>
		private StoreList store;
		private readonly int countOfStores = 0;
		
	
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
			this.countOfOrders = this.orders.TotalCount();
			this.store = new StoreList(store.Where(el => el[2] > 0).ToArray());
			this.countOfStores = new OrderList(store).TotalCount();
			
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
		public (RaspilMap, OrderList) Algoritm()
		{
			var raspileMap = new RaspilMap();

			RaspilMapCircle(raspileMap, scladMax ? preassignOrderArray.shortMeasures : preassignOrderArray.all);
			RaspilMapCircle(raspileMap, preassignOrderArray.stock3);
			RaspilMapCircle(raspileMap, preassignOrderArray.stock4);
			RaspilMapCircle(raspileMap, preassignOrderArray.stock5);



			if (orders.Count() != 0) {
				return (raspileMap, orders);
			}

			var (or, st) = raspileMap.GetTotalBoardsInMap(countOfOrders, countOfStores);
			if (or == countOfOrders) {
				Console.WriteLine("Success");
			}
			return (raspileMap, null);
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
						raspileMap.Add(kp.Key, new List<OneBoardCombinations>() { kp.Value });
						orders.Subtract(kp.Value);
						store.Subtract(kp.Value);
						return 0;
					}).ToArray();
				}
				else
				{
					break;
				}

			}
		}

		
		
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
					orders.Where(el => el.id == id).ToArray(),
					store.Where(el => el.id == id && Array.IndexOf(exceptionedStock, el.numRepos) != -1).ToArray(),
					store.FindLongMeasure(id),
					widthSaw,
					optimize);

				OneBoardCombinations boardComb = null;
				bagTasker.Calculate().Select(boardCombinations =>
				{
					var res = boardCombinations.GetBestOneBoardCombination();
					if (boardComb == null)
						boardComb = res;
					if (boardComb.GetBestPercantage() < res.GetBestPercantage())
					{
						boardComb = res;
					}
					return 0;
				}).ToArray();

				if (boardComb != null)
					bestComparisons.Add(id, boardComb);

			});




			if (bestComparisons.Count == 0)
			{
				return (false, bestComparisons);
			}

			return (true, bestComparisons);
		}



		

		
	}


}