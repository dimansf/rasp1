using BagTasker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagTask
{
	public class BagTasker
	{
        int[] dlinnomerScladId = new int[] { 3, 4 };
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
		List<(int, CustomList)> conds;
        /// <summary>
        /// На этому вроне нет id досок, просто заказы и массив складов
        /// 2 уровень
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="store"></param>
        /// <param name="liqCondition"></param>
        /// <returns> Лист [доска => комбинация]</returns>
        public List<((int, int), List<(int, CustomList)>)> calculate(int[][] orders, int[][] store, bool liqCondition = true, int widthSaw=4)
		{
			var dat = new List<((int,int), List<(int, CustomList)>)>();

			store.Select(el =>
			{
				//[ид, длина, кол - во, ликвид, макс.обр, номер склада]
				var temp = calc(orders, el[1], widthSaw);
				if (liqCondition)
				{
					temp = LiqSelect(temp, el);
				}
				dat.Add( ((el[1], el[5]), temp) );

				return 0;
			}).ToList();
			
			return dat;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="combs"></param>
		/// <param name="liq"></param>
		/// <param name="obr"></param>
		/// <returns></returns>
		private List<(int, CustomList)> LiqSelect(List<(int, CustomList)> combs , int[] els)
		{
			var res = new List<(int, CustomList)>();
			foreach (var el in combs)
			{
				if (el.Item1>= els[3] || el.Item1 <= els[1]/100 * els[4]) 
					res.Add(el);
                else if(el.Item2.lis.Count() == 1 && el.Item2.lis[0].Item2 == 1 && els[4] == dlinnomerScladId[0] || els[4] == dlinnomerScladId[1] && el.Item2.lis[0].Item3 + els[3] > els[1])
                {
                    res.Add(el);
                }
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
		private List<(int, CustomList)> calc(int[][] orders, int total, int widthWa = 4)
		{
			this.orders = orders.Select(el => new int[] { el[0], el[1] + widthWa, el[2], el[3] }).ToArray();
			this.total = total;
			// [строка сложенных элементов, остаток от доски]
			conds = new List<(int, CustomList)>();
            int x = 0;
			Deeper(0, ref x, conds, new CustomList(), 0);

			return conds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="depth"></param>
		/// <param name="currSum"></param>
		/// <param name="conds"> Массив комбинаций главный </param>
		/// <param name="condStr"> переформировать в лист</param>
		/// <param name="counter"></param>
		private void Deeper(int depth, ref int currSum, List<(int, CustomList)> conds, CustomList cond,  int  counter)
		{
            var cond2 = new CustomList(cond);
            //[ "остаток от доски", ["ид заказа", число в доске]]
            if (depth >= orders.Length)
			{
				return;
			}
			for (var i = 0; i <= orders[depth][0]; i++)
			{
				var s = orders[depth][1] * i + currSum;
				if (s > this.total) return;
				// ид заказа, число досок
				if (i > 0)
				{
                    if(!keyExist(conds, this.total - s))
                    {
                        cond2.Add((orders[depth][3], i, orders[depth][1]));
                        conds.Add((this.total - s, cond2));
                    }
                        
				}
				Deeper(depth + 1, ref s, conds, cond2, counter + i);
			}
			
		}

        private bool keyExist(List<(int, CustomList)> conds, int ostk)
        {
            foreach(var cn in conds)
            {
                if (cn.Item1 == ostk)
                    return true;
            }
            return false;
        }
      
    }
}
	
