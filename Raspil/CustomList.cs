using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{

	public class CustomList 
	{

		public List<(int lineNumber, int amount , int lenght)> lis = null;

		public bool singleFlag  = false;

		
		

		public CustomList()
		{
			lis = new List<(int, int, int)>();

		}
		public CustomList(CustomList cl)
		{
			lis = cl.lis.Select(el => (el.Item1, el.Item2, el.Item3)).ToList();
			singleFlag = cl.singleFlag;
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
				x += tup.amount;
			});
			return x;
		}
		/// <summary>
		/// Общая длина палок
		/// </summary>
		/// <returns></returns>
		public int Summlen() {
			int x = 0;
			lis.ForEach(tup =>
			{
				x += tup.lenght * tup.amount;
			});
			return x;
		}

		public int SummlenSingle() {
			int x = 0;
			lis.ForEach(tup =>
			{
				x += tup.lenght;
			});
			return x;
		}
		/// <summary>
		/// Все ли палки в одном экземпляре
		/// </summary>
		/// <returns></returns>
		public bool allSingles() {
			bool fl = true;
			lis.ForEach(el => {
				if (el.amount != 1)
					fl = false;
			});
			return fl;
		}
		public void Add((int lineNumber, int amount, int lenght) tup)
        {               //(ид строки, кол-во)
            bool fl = true;
            lis = lis.Select(el => {
                if (el.lineNumber == tup.lineNumber)
                {
                    fl = false;
                    return (el.lineNumber, tup.amount, el.lenght);
                }
                return el;
            }).ToList();
            if (fl)
            {
                lis.Add(tup);
            }
        }

		
	}

	public class OneStoreCombinations
	{
		/// <summary>
		/// (остаток от доски, карта элементов)
		/// </summary>
		public List<(int remain, CustomList list)>  combinations;
		public int lenght;
		public int scladId;

		public OneStoreCombinations(int lenght, int scladId, List<(int, CustomList)> combinations)
		{
			this.lenght = lenght;
			this.scladId = scladId;
			this.combinations = combinations;
		}
	}
}
