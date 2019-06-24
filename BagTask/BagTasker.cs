using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagTask
{
	public class BagTasker
	{
		/// <summary>
		/// 
		/// </summary>
		int[][] orders;
		/// <summary>
		/// 
		/// </summary>
		int total;
		/// <summary>
		/// 
		/// </summary>
		int ostatok;
		/// <summary>
		/// 
		/// </summary>
		int liquid;
		/// <summary>
		/// 
		/// </summary>
		List<(string, int)> conds;
		/// <summary>
		/// 
		/// </summary>
		int wSaw;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="store"></param>
		/// <param name="liqCondition"></param>
		/// <returns></returns>
		public Dictionary<(int, int), List<(string, int)>> calculate(int[][] orders, int[][] store, bool liqCondition = true)
		{
			var dat = new Dictionary<(int, int), List<(string, int)>>();

			store.Select(el =>
			{
				//[ид, длина, кол - во, ликвид, макс.обр, номер склада]
				var temp = calc(orders, el[1]);
				if (liqCondition)
				{
					temp = LiqSelect(temp, el[3], el[4]);
				}
				dat[(el[1], el[5])] = temp;

				return 0;
			});
			
			return dat;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="combs"></param>
		/// <param name="liq"></param>
		/// <param name="obr"></param>
		/// <returns></returns>
		private List<(string, int)> LiqSelect(List<(string, int)> combs , int liq, int obr)
		{
			var res = new List<(string, int)>();
			foreach (var el in combs)
			{
				if (el.Item2 >= liq || el.Item2 <= obr)
					res.Add(el);
			}
			return res;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="total"></param>
		/// <param name="widthWa"></param>
		/// <returns></returns>
		private List<(string, int)> calc(int[][] orders, int total, int widthWa = 4)
		{
			this.orders = orders.Select(el => new int[] { el[0], el[1] + widthWa }).ToArray();
			this.total = total;


			// [строка сложенных элементов, остаток от доски]
			conds = new List<(string, int)>();
			Deeper(0, 0, conds, "", 0);

			return conds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="depth"></param>
		/// <param name="currSum"></param>
		/// <param name="conds"></param>
		/// <param name="condStr"></param>
		/// <param name="counter"></param>
		private void Deeper(int depth, int currSum, List<(string, int)> conds, string condStr, int counter)
		{ 

			//[ "остаток от доски", ["строка заказа", число в доске]]
			if (depth >= orders.Length)
			{
				return;
			}
			for (var i = 0; i <= orders[depth][0]; i++)
			{
				var s = orders[depth][1] * i + currSum;
				if (s > this.total) return;
				var forAppend = condStr;

				if (i != 0 )
				{
					if (depth > 0 && condStr.Length > 0)
						forAppend = condStr + " + " + this.orders[depth][1]+ $"({this.orders[depth][3]})" + "*" + i ;
					else
					{
						forAppend = condStr + this.orders[depth][1] + $"({this.orders[depth][3]})"+ '*' + i;
					}
				}
				// forAppend = condsStr + this.orders[depth][1] + '*' + i ;
				if (counter + i != 0)
					if (!HasKey(forAppend))
						conds.Add( (forAppend, total - s));

				Deeper(depth + 1, s, conds, forAppend, counter + i);
			}
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyStr"></param>
		/// <returns></returns>
		public bool HasKey(string keyStr)
		{
			var flag = false;
			conds.Select(element => {
				if (element.Item1 == keyStr)
				{
					flag = true;
					return 0;
				}
				return 0;
			});
			return flag;
		}
	}
}
	
