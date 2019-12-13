
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Raspil
{
	public class BagTasker
	{
		readonly OrderList orders;
		readonly StoreList storeList;
		
		readonly int widthSaw;
		readonly bool liqCondition;
		


		public BagTasker(IEnumerable<OrderBoard> orders, IEnumerable<StoreBoard> storeList,  int widthSaw = 3, bool liqCondition = false)
		{
			this.widthSaw = widthSaw;
			
			this.orders = new OrderList(orders);
			this.storeList = new StoreList(storeList);



			this.liqCondition = liqCondition;
			
			// [строка сложенных элементов, остаток от доски]
		
		}
		/// <summary>
		/// На этом уровне нет id досок, просто заказы и массив складов
		/// 2 уровень
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="store"></param>
		/// <param name="liqCondition"></param>
		/// <returns> (длина доски , ИД склада ), [структура с картой подбора всех заказов]</returns>
		public List<OneBoardCombinations> Calculate()
		{

			var listCombs = new List<OneBoardCombinations>();
			// для каждой  палки на складе в кол-ве  > 0
			storeList.ForEach(storeStick =>
			{
				var combs = new OneBoardCombinations(storeStick);
				Deeper(0, combs, new OrderList());
				// 1
				combs.WidthSawSelect(widthSaw);
				// 2
				var x = liqCondition  ? combs.LiquidSelect(storeList.FindLongMeasure(storeStick.id)) : -1;
				
				// карта комбинаций для конкретной доски 
				// длина доски , ИД склада , структура с картой подбора всех заказов
				listCombs.Add(combs);
				
			});

			return listCombs;
		}

	
		/// <summary>
		/// Главная рекурсивая функция
		/// </summary>
		/// <param name="depth"> Кол-во заказов</param>


		/// <param name="cond"> Карта завазов что в данный момент дают сумму</param>

		private void Deeper(int depth, OneBoardCombinations combs, OrderList rememberedList)
		{
			if (depth >= orders.Count())
			{
				return;
			}


			for (var i = 0; i <= orders[depth].count; i++)
			{
				var currentList = new OrderList(rememberedList.Clone() as OrderList);



				if (orders[depth].len * i + currentList.Summlen() > combs.board.len) return;

				if (i > 0)
				{
					var board = orders[depth].Clone() as OrderBoard;
					board.count = i;
					// номер строки , кол-во, длина заказа
					currentList.Add(board);
					combs.Add(currentList);

				}
				Deeper(depth + 1, combs, currentList);
			}

		}



	}
}

