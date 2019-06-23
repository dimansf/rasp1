using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{

	public class RaspilOperator
	{

		public RaspilOperator()
		{
		}
		/// <summary>
		/// 1. Самый лучшая обрезь та, что total - ostatok больше всего, значит доска использовалась по полной 
		/// пример: 1500 - 500 = 1000 или 6000 - 2000 = 4000
		/// получается, что 6000 - 4000 = 2000 полезной нагрузки
		/// 1500 - 1000 = 500 полезной нагрузки
		/// очевидно выбрать стоит выбрать 6000
		/// </summary>
		public void algoritm1(int[][] orders, int[][] store)
		{

			var mapRaspil = new List<List<(int, (string, int, int))>>();
			var bt = new BagTask.BagTasker();
			var unicalID = orders.Select(row => row[0]).Distinct().ToArray();

			while (checkOrders(orders))
			{
				// {ид доски => {длина доски, номер склада => ["комбинация", остаток] } }
				var res = new Dictionary<int, Dictionary<(int, int), List<(string, int)>>>();

				// 1 этап генерация комбинаций для каждого ордера
				unicalID.Select(el =>
				{

					res[el] = bt.calculate(selectByID(orders, el), selectByID(store, el));

					if (res[el].Count == 0)
						res[el] = bt.calculate(selectByID(orders, el), selectByID(store, el), false);

					return 0;
				});

				// 2 выбор лучших элементов
				var bests = new List<(int, (string, int, int))>();
				foreach (var el in res)
				{
					bests.Add((el.Key, getBestCombination(el.Value)));
				}

				// вычесть лучшие просчеты и добавить в карту распила

				mapRaspil.Add(bests);

				subtractBestVectorValues();
			}
		}

		private void subtractBestVectorValues(int[][] orders, int[][] store, List<(int, (string, int, int))> best)
		{											// ид доски => ["комбинация", длина доски, номер склада]
			
			foreach (var el in best) {
				var els = el.Item2.Item1.Split('+');

			}
		}
		private int[][] selectByID(int[][] orders, int id) {
			var ls = new List<int[]>();
			foreach (var el in orders)
			{
				if (el[0] == id)
					ls.Add(el);
			}
			return ls.ToArray();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="combs">
		/// 1 параметр длина 6000, номер склада 1
		/// 2 лист комбинаций заказов ("строка комбинаций", остаток)
		/// </param>
		private (string, int, int) getBestCombination(Dictionary<(int, int), List<(string, int)>> combs)
		{
			// строка комбинации, длина палки, склад
			var bcomb = ("", 0, 0);
			foreach (var row in combs)
			{
				foreach (var el in row.Value)
				{
					// если остаток 1500 - 500 = 1000
					// если 6000 - 1000 = 5000
					// полезный материал
					if (row.Key.Item1 - bcomb.Item2 < row.Key.Item1 - el.Item2)
						bcomb = (el.Item1, row.Key.Item1, row.Key.Item2);
				}
			}
			return bcomb;
		}

		private bool checkOrders(int[][] orders)
		{
			if (orders.Length > 0) return true;
			return false;
		}
		/// <summary>
		/// В первую очередь расходуем всю обрезь со склада, по условиям ликвида, но без условий полезной нагрузки
		/// Остальное по первому алгоритму
		/// </summary>
		public void algoritm2()
		{

		}
		/// <summary>
		/// В первую очередь расходуем обрезь, без условий ликвида и полезной нагрузки
		/// Остальное по первому алгоритму
		/// </summary>
		public void algoritm3()
		{

		}



	}
}
