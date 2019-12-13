using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{

	
	public class RaspilMap
	{
		Dictionary<int, List<OneBoardCombinations>> map;
		public RaspilMap() 
		{
			map = new Dictionary<int, List<OneBoardCombinations>>();
		}

		public void Add(int key, OneBoardCombinations combList)
		{
			var index = map[key].IndexOf(combList);
			if (index == -1)
				map[key].Add(combList);
			else
				map[key][index].Duplicate();


		}


	}
	/// <summary>
	/// (остаток от доски, карта элементов)
	/// </summary>
	public class OneBoardCombinations : List<OrderList>, IEquatable<OneBoardCombinations>
	{

		/// <summary>
		/// 
		/// </summary>
		public readonly StoreBoard board;
		public int boardCounter = 1;

		public OneBoardCombinations(StoreBoard board = null, List<OrderList> list = null) : base()
		{
			if (board != null)
				this.board = board;
			if (list != null)
				AddRange(list);
		}

		public int WidthSawSelect(int widthSaw)
		{
			int counter = 0;
			ForEach(orderList =>
			{
				if (board.len - orderList.TotalCount() * widthSaw + orderList.Summlen() >= -widthSaw)
				{
					Remove(orderList);
					counter++;
				}
			});
			return counter;

		}
		public OneBoardCombinations getBestOneBoardCombination()
		{
			var bestList = new OrderList();

			ForEach(combination =>
			{
				var best = Math.Round(Convert.ToDouble((double)bestList.Summlen() / (double)board.len), 3);
				var current = Math.Round(Convert.ToDouble((double)combination.Summlen() / (double)board.len), 3);
				if (best < current)
					bestList = combination;

			});
			return new OneBoardCombinations((StoreBoard)board.Clone(), new List<OrderList>() { (OrderList)bestList.Clone() });
		}
		public double getBestPercantage(int index = 0)
		{
			return Math.Round(Convert.ToDouble((double)this[index].Summlen() / (double)board.len), 3);
		}

		public void Duplicate()
		{
			boardCounter++;
		}

		public int LiquidSelect(int longMeasure = 5500)
		{
			int counter = 0;
			// если палка не длиномер то длину задать искуственно
			var baseForMin = board.len >= 5000 ? board.len : longMeasure;
			ForEach(orderList =>
			{
				var remain = board.len - orderList.Summlen();



				if (!(remain >= board.remain || remain <= baseForMin / 100 * board.remainPercent))
				{
					counter++;
					Remove(orderList);
				}
				else
				if (!(orderList.TotalCount() == 1 && board.numRepos > 2 && orderList.Summlen() * 2 > board.len))
				{
					counter++;
					Remove(orderList);
				}
			});
			return counter;
		}

		public bool Equals(OneBoardCombinations other)
		{
			var flag = this.board == other.board;
			if (!flag) return flag;
			ForEach(el =>
			{
				if (other.IndexOf(el) == -1)
					flag = false;
			});
			return flag;

		}
	}
	//[ид, длина, кол - во, ликвид, макс.обр, номер склада]
	public class StoreBoard : OrderBoard, IEquatable<StoreBoard>
	{


		public int remain = 0;
		//минимальный процент остатка от распила
		public int remainPercent = 0;
		public int numRepos = 0;
		public StoreBoard():base() { }
		public StoreBoard(OrderBoard ob): base(ob) {
			
		}
		public new object Clone()
		{
			StoreBoard clone = new StoreBoard((OrderBoard)base.Clone());
			
			clone.remain = remain;
			clone.remainPercent = remainPercent;
			clone.numRepos = numRepos;
			return clone;

		}
		public bool Equals(StoreBoard other)
		{
			return this == other;
		}

		public static bool operator ==(StoreBoard obj1, StoreBoard obj2)
		{
			if (obj1.id == obj2.id &&
				obj1.len == obj2.len &&
				obj1.remain == obj2.remain &&
				obj1.remainPercent == obj2.remainPercent &&
				obj1.numRepos == obj2.numRepos 
				//obj1.count == obj2.count
				) return true;
			return false;
		}
		public static bool operator !=(StoreBoard obj1, StoreBoard obj2)
		{
			if (obj1 == obj2) return false;
			return true;
		}
	}
	//[ид, длина, кол - во, номер строки]
	public class OrderBoard : ICloneable, IEquatable<OrderBoard>
	{

		public int id = 0;
		public int len = 0;
		public int count = 0;
		//public int lineNumber = -1;

		//public new string ToString() { 

		//}
		public OrderBoard() { }
		public OrderBoard(OrderBoard ob) {
			id = ob.id;
			len = ob.len;
			count = ob.count;
		}

		public object Clone()
		{

			return new OrderBoard()
			{
				id = this.id,
				len = this.len,
				count = this.count
				//lineNumber = this.lineNumber
			};
		}

		public bool Equals(OrderBoard other)
		{
			return this == other;
		}

		public static bool operator ==(OrderBoard obj1, OrderBoard obj2)
		{
			if (obj1.id == obj2.id &&
				obj1.len == obj2.len 
				//obj1.count == obj2.count 
				//obj1.lineNumber == obj2.lineNumber
				) return true;
			return false;
		}

		public static bool operator !=(OrderBoard obj1, OrderBoard obj2)
		{
			if (obj1 == obj2) return false;
			return true;
		}
	}

	public class StoreList : List<StoreBoard>
	{
		public StoreList(IEnumerable<StoreBoard> list) : base()
		{
			list.Select(el => { Add(el); return true; });
		}
		public StoreList(int[][] elems) : base()
		{
			elems.Select(row =>
			{
				Add(new StoreBoard()
				{
					id = row[0],
					len = row[1],
					count = row[2],
					remain = row[3],
					remainPercent = row[4],
					numRepos = row[5]
				});
				return 0;
			}).ToArray();
		}
		public void Subtract(OneBoardCombinations board)
		{
			this[IndexOf(board.board)].count -= board.board.count;
			ClearEmptyOrderBoards();

		}

		private int ClearEmptyOrderBoards()
		{
			int c = 0;
			ForEach(el =>
			{
				if (el.count == 0)
				{
					this.Remove(el);
					c++;
				}

			});

			return c;
		}

		public int FindLongMeasure(int id)
		{
			int max = 0;
			ForEach(board =>
			{
				if (board.id == id && max < board.len)
					max = board.len;
			});
			return max;
		}

		
	}
	public class OrderList : List<OrderBoard>, ICloneable, IEnumerable<OrderBoard>
	{
		public OrderList() : base() { }

		public OrderList(IEnumerable<OrderBoard> list) : base()
		{
			list.Select(el => { Add(el); return true; });
			
		}

		public OrderList(IEnumerable<int[]> elems)
		{
			int i = 0;
			elems.Select(row =>
			{
				Add(new OrderBoard()
				{
					id = row[0],
					len = row[1],
					count = row[2],
					//lineNumber = i++

				});
				return 0;
			}).ToArray();
		}
		public void Substitute(OrderBoard board)
		{
			
			var idx = IndexOf(board);
			if (idx != -1)
			{
				
				this[idx].count = board.count;
			}
			else
			{
				Add(board);
			}
		}
		public new void Add(OrderBoard board) {
			var idx = IndexOf(board);
			if (idx != -1)
			{
				this[idx].count += board.count;
			}
			else
			{
				base.Add(board); 
			}
		}
		public new void AddRange(IEnumerable<OrderBoard> collection)
		{
			collection.Select(el => { Add(el); return 1; }).ToArray();
		}
		public void Subtract(OneBoardCombinations board) {
			board[0].ForEach(el =>
			{
				this[IndexOf(el)].count -= el.count;
			});

			ClearEmptyOrderBoards();

		}

		private int ClearEmptyOrderBoards() {
			int c = 0;
			ForEach(el =>
			{
				if (el.count == 0) {
					this.Remove(el);
					c++;
				}
					
			});

			return c;
		}
		public object Clone()
		{
			var orl = new OrderList();
			ForEach(el => orl.Add(el.Clone() as OrderBoard));
			return orl;
		}

		/// <summary>
		/// Считаем число досок в списке комбинаций к доске из склада
		/// </summary>
		/// <returns></returns>
		public int TotalCount()
		{
			int x = 0;
			ForEach(elems =>
			{
				x += elems.count;
			});
			return x;
		}
		/// <summary>
		/// Общая длина палок
		/// </summary>
		/// <returns></returns>
		public int Summlen()
		{
			int x = 0;
			ForEach(tup =>
			{
				x += tup.len * tup.count;
			});
			return x;
		}

		public static bool operator ==(OrderList obj1, OrderList obj2)
		{
			var flag = true;
			obj1.ForEach(el =>
			{
				if (obj2.IndexOf(el) == -1)
					flag = false;
			});
			return flag;
		}

		public static bool operator !=(OrderList obj1, OrderList obj2)
		{
			if (obj1 == obj2) return false;
			return true;
		}

	}

}
