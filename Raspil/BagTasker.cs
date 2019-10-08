
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Raspil
{
    public class BagTasker
    {

		/// <summary>
		/// 
		/// store  - - [ид, длина, кол - во, ликвид, макс.обр, номер склада]
		/// orders -- []
		/// </summary>
		int[][] orders;
		
        /// <summary>
        /// 
        /// </summary>
        int total = 0;

		
        /// <summary>
        /// 
        /// </summary>
        List<(int, CustomList)> conds;

		public BagTasker() {
			
		}
		/// <summary>
		/// На этом уровне нет id досок, просто заказы и массив складов
		/// 2 уровень
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="store"></param>
		/// <param name="liqCondition"></param>
		/// <returns> (длина доски , ИД склада ), [структура с картой подбора всех заказов]</returns>
		public List<OneStoreCombinations> Calculate(int[][] orders, int[][] store, int longMeaser = 6000, int widthSaw=0, bool liqCondition=false)
        {
						//(длина доски, ИД склада),[структура с картой подбора всех заказов]
			//var dat = new List<((int, int), List<(int, CustomList)>)>();
			var dat = new List<OneStoreCombinations>();
			// для каждой  палки на складе в кол-ве  > 0
			store.Select(storeStick =>
            {
                var temp = Calc(orders, storeStick[1]);
				// 1
				temp = WidthSawSelect(temp, widthSaw);
				// 2
				if (liqCondition)
				{
					temp = LiqSelect(temp, storeStick, longMeaser);
				}
				// карта комбинаций для конкретной доски 
				// длина доски , ИД склада , структура с картой подбора всех заказов
				dat.Add(new OneStoreCombinations(storeStick[1], storeStick[5], temp));
                return 0;
            }).ToList();

            return dat;
        }

		private List<(int, CustomList)> WidthSawSelect(List<(int, CustomList)> combs, int widthSaw) {
			// вычитание толщины пила
			return combs.Select(el => {
				// частный случай когда доска в точности совпадает с длиной распила и ничего не нужно резать
				// 2376 - 2376 = 0
				//if (el.Item1 == 0 && el.Item2.GetCountItems() == 1)
				//{
				//	return (el.Item1, new CustomList(el.Item2));
				//}

				var remain = el.Item1 - widthSaw * el.Item2.GetCountItems();

				return remain >= -widthSaw ? 
				(remain > 0 ? remain : 0, new CustomList(el.Item2)): 
				(remain, new CustomList(el.Item2));

			}).Where(el => el.Item1 >= 0).ToList();
		}

		/// <summary>
		/// Костыль для показательного(тестового распила)
		/// </summary>
		/// <param name="combs"></param>
		/// <param name="els"></param>
		/// <returns></returns>
		private List<(int, CustomList)> SingleSelect(List<(int, CustomList)> combs, int[] els) {
			var res = new List<(int, CustomList)>();
			int baseForMin;
			foreach (var el in combs)
			{
				// если палка не длиномер то длину задать искуственно
				baseForMin = els[1] < 5000 ? 5000 : els[1];
				// 3 - остаток в мм , 4 - процент
				if (el.Item1 <= baseForMin / 100 * els[4] &&
					el.Item2.allSingles())
				{
					el.Item2.singleFlag = true;
					res.Add(el);
				}
				
			}
			return res;
		}

		/// <summary>
		/// Проверка на условия ликвидности, по 3 и 4 параметра доски скалда
		/// </summary>
		/// <param name="combs"></param>
		/// <param name="liq"></param>
		/// <param name="obr"></param>
		/// <returns></returns>
		private List<(int, CustomList)> LiqSelect(List<(int, CustomList)> combs, int[] els, int longMeaser)
        {
            var res = new List<(int, CustomList)>();
			int baseForMin;
            foreach (var el in combs)
            {
				// если палка не длиномер то длину задать искуственно
				baseForMin = els[1] > 5000 ? els[1]: longMeaser;
				// 3 - остаток в мм , 4 - процент
				if (el.Item1 >= els[3] || el.Item1 <= baseForMin / 100 * els[4])
                    res.Add(el);
                else 
				if (el.Item2.GetCountItems() == 1 && 
					els[5] > 2 &&
					el.Item2.Summlen()*2 > els[1])
                {
                    res.Add(el);
                }
            }
            return res;

        }
		
		/// <summary>
		/// Подсчет для конкретной длины доски
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="total"></param>
		/// <param name="widthWa"></param>
		/// <returns>[(остаток, лист комбинаций)]</returns>
		private List<(int, CustomList)> Calc(int[][] orders, int total, int widthWa = 4)
        {
			//this.orders = orders.Select(el => new int[] { el[0], el[1] + widthWa, el[2], el[3] }).ToArray();
			this.orders = orders;
			this.total = total;
            // [строка сложенных элементов, остаток от доски]
            conds = new List<(int, CustomList)>();
            
            Deeper(0, conds, new CustomList());

			return conds;
        }

		/// <summary>
		/// Главная рекурсивая функция
		/// </summary>
		/// <param name="depth"> Кол-во заказов</param>
		/// <param name="currSum"> Текущая сумма длин заказов</param>
		/// <param name="conds"> Главный массив комбинаций  </param>
		/// <param name="cond"> Карта завазов что в данный момент дают сумму</param>
		/// <param name="counter"></param>
		private void Deeper(int depth, List<(int, CustomList)> conds, CustomList cond)
        {
            if (depth >= orders.Length)
            {
                return;
            }

            for (var i = 0; i <= orders[depth][0]; i++)
            {
				var cond2 = new CustomList(cond);

				//var s = orders[depth][1] * i + currSum;
				var s = orders[depth][1] * i + cond.Summlen();
                if (s > this.total) return;
                
                if (i > 0)
                {
						// номер строки , кол-во, длина заказа
						cond2.Add( (orders[depth][3], i, orders[depth][1]) );
						// остаток длины , лист распилов
                        conds.Add((this.total - s, cond2));
                }
                Deeper(depth + 1, conds, cond2);
            }

        }

        private bool KeyExist(List<(int, CustomList)> conds, int ostk)
        {
            foreach (var cn in conds)
            {
                if (cn.Item1 == ostk)
                    return true;
            }
            return false;
        }

    }
}

