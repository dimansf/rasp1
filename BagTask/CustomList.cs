using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagTasker
{
	public class CustomList
	{
		
		public List<(int,int)> lis = null;
		public CustomList(int x, int y)
		{
			lis = new List<(int, int)>();
			this.Add((x, y));
		}
		public CustomList()
		{
			lis = new List<(int, int)>();
			
		}
		public void  Add((int,int) tup)
		{               //(ид строки, кол-во)
			bool fl = true;
			lis = lis.Select(el => {
				if (el.Item1 == tup.Item1) {
					fl = false;
					return (el.Item1, el.Item2 + tup.Item2);
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
