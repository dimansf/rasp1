using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{

    public class CustomList
    {

        public List<(int, int, int)> lis = null;
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
		public void Add((int, int, int) tup)
        {               //(ид строки, кол-во)
            bool fl = true;
            lis = lis.Select(el => {
                if (el.Item1 == tup.Item1)
                {
                    fl = false;
                    return (el.Item1, tup.Item2, el.Item3);
                }
                return el;
            }).ToList();
            if (fl)
            {
                lis.Add(tup);
            }
        }
    }
}
