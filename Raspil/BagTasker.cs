
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Raspil
{
    public class BagTasker
    {
		private readonly int[] dlinnomerScladId = new int[] { 3, 4, 5 };
        /// <summary>
        /// 
        /// </summary>
        int[][] orders;
        /// <summary>
        /// 
        /// </summary>
        int total = 0;

		bool singleFlag = false;
        /// <summary>
        /// 
        /// </summary>
        List<(int, CustomList)> conds;

		public BagTasker(bool sf = false) {
			singleFlag = sf;
		}
		/// <summary>
		/// На этом уровне нет id досок, просто заказы и массив складов
		/// 2 уровень
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="store"></param>
		/// <param name="liqCondition"></param>
		/// <returns> (длина доски , ИД склада ), [структура с картой подбора всех заказов]</returns>
		public List<OneStoreCombinations> Calculate(int[][] orders, int[][] store, bool liqCondition = true, int widthSaw = 4)
        {
						//(длина доски, ИД склада),[структура с картой подбора всех заказов]
			//var dat = new List<((int, int), List<(int, CustomList)>)>();
			var dat = new List<OneStoreCombinations>();
			// для каждой  палки на складе в кол-ве  > 0
			store.Select(storeStick =>
            {
                //[ид, длина, кол - во, ликвид, макс.обр, номер склада]
                var temp = Calc(orders, storeStick[1], widthSaw);
				List<(int, CustomList)> temp1 = null, temp2 = null;

				if (liqCondition)
                {
                    temp1 = LiqSelect(temp, storeStick);
					/* как частный случай ликвидного остатка, иначе нет смысла 
					 была введена для очень специфичного распила, из Generators
					 оставил пометку
					*/
					if (singleFlag)
					{
						temp2 = SingleSelect(temp, storeStick);
					}

					temp = temp2 != null && temp2.Count != 0 ? temp2 : temp1;
					
				}
				
				
				// карта комбинаций для конкретной доски 
				// длина доски , ИД склада , структура с картой подбора всех заказов
				dat.Add(new OneStoreCombinations(storeStick[1], storeStick[5], temp));
				

                return 0;
            }).ToList();

            return dat;
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
		private List<(int, CustomList)> LiqSelect(List<(int, CustomList)> combs, int[] els)
        {
            var res = new List<(int, CustomList)>();
			int baseForMin;
            foreach (var el in combs)
            {
				// если палка не длиномер то длину задать искуственно
				baseForMin = els[1] < 5000 ? 5000 : els[1];
				// 3 - остаток в мм , 4 - процент
				if (el.Item1 >= els[3] || el.Item1 <= baseForMin / 100 * els[4])
                    res.Add(el);
                else 
				if (el.Item2.lis.Count() == 1 && 
					el.Item2.lis[0].Item2 == 1 && 
					(els[5] == dlinnomerScladId[0] || 
					els[5] == dlinnomerScladId[1] ||
					els[5] == dlinnomerScladId[2]) && 
					el.Item2.lis[0].Item3 + els[3] > els[1])
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

			// вычитание толщины пила
			conds = conds.Select(el =>{
				// частный случай когда доска в точности совпадает с длиной распила и ничего не нужно резать
				// 2376 - 2376 = 0
				if (el.Item1 == 0 && el.Item2.GetCountItems() == 1) {
					return (el.Item1, new CustomList(el.Item2));
				}

				var remain = el.Item1 - widthWa * el.Item2.GetCountItems();

				return (remain , new CustomList(el.Item2));
				
			}).Where(el => el.Item1 >= 0).ToList();

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
                    //if (!KeyExist(conds, this.total - s))
					
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

