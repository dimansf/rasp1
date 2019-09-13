
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
        int total;

        /// <summary>
        /// 
        /// </summary>
        List<(int, CustomList)> conds;
		/// <summary>
		/// На этом уровне нет id досок, просто заказы и массив складов
		/// 2 уровень
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="store"></param>
		/// <param name="liqCondition"></param>
		/// <returns> (длина доски , ИД склада ), [структура с картой подбора всех заказов]</returns>
		public List<((int, int), List<(int, CustomList)>)> Calculate(int[][] orders, int[][] store, bool liqCondition = true, int widthSaw = 4)
        {
            var dat = new List<((int, int), List<(int, CustomList)>)>();
			// для каждой  палки на складе в кол-ве  > 0
			store.Select(el =>
            {
                //[ид, длина, кол - во, ликвид, макс.обр, номер склада]
                var temp = Calc(orders, el[1], widthSaw);
                if (liqCondition)
                {
                    temp = LiqSelect(temp, el);
                }
				// карта комбинаций для конкретной доски 
				// длина доски , ИД склада , структура с картой подбора всех заказов
                dat.Add(((el[1], el[5]), temp));

                return 0;
            }).ToList();

            return dat;
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
			var baseForMin = 0;
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
		private void CheckValidConds( (int, CustomList) list, int len) {

			if (len - list.Item1 != list.Item2.Summlen())
				throw new Exception("SummLen not equal CurrSumm");
			
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
           
            Deeper(0, conds, new CustomList(), 0);

			conds = conds.Select(el =>{
				//28.07.2019 убрал добавочную ширину для доски
				CheckValidConds(el, total);

				return (this.total - el.Item2.Summlen() - widthWa * el.Item2.GetCountItems(), el.Item2);
				//return (el.Item1 - widthWa * el.Item2.GetCountItems(), new CustomList(el.Item2));
				
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
		private void Deeper(int depth, List<(int, CustomList)> conds, CustomList cond, int currSumm)
        {
            if (depth >= orders.Length)
            {
                return;
            }

            for (var i = 0; i <= orders[depth][0]; i++)
            {
				var cond2 = new CustomList(cond);

				//var summ = orders[depth][1] * i + currSumm;
				var summ = orders[depth][1] * i + cond.Summlen();
				if (summ > this.total) return;
                
                if (i > 0)
                {
					//if (!KeyExist(conds, this.total - summ))
					if (true)
					{
						// номер строки , кол-во, длина заказа
						cond2.Add((orders[depth][3], i, orders[depth][1]));
						// остаток длины , лист распилов
                        conds.Add((this.total - summ, cond2));
                    }

                }
                Deeper(depth + 1, conds, cond2, summ);
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

