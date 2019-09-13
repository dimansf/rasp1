using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{

	public class CustomList
	{
		static long callCounter = 0;
		private int lenCounter = 0;

		public List<(int, int, int)> lis = null;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x">номер строки в заказе</param>
		/// <param name="y">кол-во</param>
		/// <param name="z">длина</param>
		public CustomList(int x, int y, int z)
		{
			lis = new List<(int, int, int)>();
			this.Add((x, y, z));
		}
		public CustomList()
		{
			lis = new List<(int, int, int)>();

		}
		public CustomList(CustomList cl)
		{
			lis = cl.lis.Select(el => (el.Item1, el.Item2, el.Item3)).ToList();
			//lenCounter = cl.Summlen();
		}

		/// <summary>
		/// Считаем число досок в списке комбинаций к доске из склада
		/// </summary>
		/// <returns></returns>
		public int GetCountItems()
		{
			int x = 0;
			lis.ForEach(tup =>
			{
				x += tup.Item2;
			});
			return x;
		}
		public int Summlen() {
			callCounter++;
			int x = 0;
			lis.ForEach(tup =>
			{
				x += tup.Item3 * tup.Item2;
			});
			//return lenCounter;
			return x;
		}
		public void Add((int, int, int) tup)
        {               //(ид строки, кол-во,  длина)
            bool fl = true;
            lis = lis.Select(el => {
                if (el.Item1 == tup.Item1)
                {
                    fl = false;
					//lenCounter += tup.Item2 * tup.Item3 - el.Item2 * el.Item3;
					return (el.Item1, tup.Item2, el.Item3);
                }
                return el;
            }).ToList();

            if (fl)
            {
				//lenCounter += tup.Item3 * tup.Item2;
				lis.Add(tup);
            }
        }
	}
}
