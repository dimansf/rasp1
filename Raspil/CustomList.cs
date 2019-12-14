using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspil
{


	public class RaspilMap : Dictionary<int, List<OneBoardCombinations>>
	{

		public RaspilMap() : base()
		{

		}

		public new void Add(int key, List<OneBoardCombinations> value)
		{

			var onc = value[0];
			List<OneBoardCombinations> val;
			this.TryGetValue(key, out val);

			if (val != null)
			{
				var index = val.IndexOf(onc);
				if (index == -1)
					val.Add(onc);
				else
					val[index].Duplicate();
			}
			else
			{
				base.Add(key, value);
			}




		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="or"></param>
		/// <param name="st"></param>
		/// <returns>(ordersCounter, storesCounter)</returns>
		public (int, int) GetTotalBoardsInMap(int or, int st) {
			var orders = 0;
			var stores = 0;
			this.Select(kp => {
				kp.Value.Select(oneComb => {
					var (ors, s) = oneComb.GetTotal();
					orders += ors;
					stores += s;
					return 0;
				}).ToArray(); 
				return 0;
			}).ToArray();
			return (orders, stores);
		}

		public new string ToString()
		{

			return "";
		}


	}
	/// <summary>
	/// (остаток от доски, карта элементов)
	/// </summary>
	public class OneBoardCombinations : List<OrderList>, IEquatable<OneBoardCombinations>, IEnumerable<OrderList>
	{

		/// <summary>
		/// 
		/// </summary>
		public readonly StoreBoard board;


		public OneBoardCombinations(StoreBoard board, List<OrderList> list = null) : base()
		{
			this.board = (StoreBoard)board.Clone();
			this.board.count = 1;
			if (list != null)
				AddRange(list);
		}

		public int WidthSawSelect(int widthSaw)
		{
			int counter = this.Count;

			RemoveAll(orderList =>
			{
				var rem = board.len - orderList.TotalCount() * widthSaw - orderList.Summlen();
				return rem < -widthSaw;
			});
			return counter - this.Count;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns> (ordersSum, storeSumm)</returns>
		public (int, int) GetTotal() {
			var ordersSum = this.Aggregate(0, (sum, list) => list.TotalCount());
			var storeSumm = board.count;
			return (ordersSum * storeSumm, storeSumm);
		}
		public new void AddRange(IEnumerable<OrderList> items)
		{
			items.Select(el => { Add(el); return 0; }).ToArray();
		}

		public new void Add(OrderList item)
		{
			var flag = true;
			ForEach(list =>
			{
				if (list.FullCompare(item))
					flag = false;
			});
			if (flag)
				base.Add(item);
		}
		public OneBoardCombinations GetBestOneBoardCombination()
		{
			var bestList = new OrderList();

			ForEach(combination =>
			{
				var best = Math.Round(Convert.ToDouble((double)bestList.Summlen() / (double)board.len), 3);
				var current = Math.Round(Convert.ToDouble((double)combination.Summlen() / (double)board.len), 3);
				if (best < current)
					bestList = combination;

			});
			return new OneBoardCombinations(board.Clone() as StoreBoard, new List<OrderList>() { bestList.Clone() as OrderList });
		}
		public double GetBestPercantage(int index = 0)
		{
			return Math.Round(Convert.ToDouble((double)this[index].Summlen() / (double)board.len), 3);
		}

		public void Duplicate()
		{
			board.count++;
		}

		public int LiquidSelect(int longMeasure = 5500)
		{
			int counter = Count;
			// если палка не длиномер то длину задать искуственно
			var baseForMin = board.len >= 5000 ? board.len : longMeasure;

			RemoveAll(orderList =>
			{
				var flag = true;
				var remain = board.len - orderList.Summlen();

				if (remain >= board.remain || remain <= baseForMin / 100 * board.remainPercent)
				{
					flag = false;
				}

				if (orderList.TotalCount() == 1 && board.numRepos > 2 && orderList.Summlen() * 2 >= board.len)
				{

					flag = false;
				}

				return flag;
			});

			return counter - Count;
		}

		public bool Equals(OneBoardCombinations other)
		{
			var flag = board == other.board &&
				other.Count == Count && Count == 1;

			var oneList = this[0];
			if (!flag) return flag;

			return flag && oneList.FullCompare(other[0]);

		}
	}





	//[ид, длина, кол - во, ликвид, макс.обр, номер склада]
	public class StoreBoard : OrderBoard, IEquatable<StoreBoard>
	{


		public int remain = 0;
		//минимальный процент остатка от распила
		public int remainPercent = 0;
		public int numRepos = 0;
		public StoreBoard() : base() { }
		public StoreBoard(OrderBoard ob) : base(ob)
		{

		}
		public new object Clone()
		{
			StoreBoard clone = new StoreBoard((OrderBoard)base.Clone());

			clone.remain = remain;
			clone.remainPercent = remainPercent;
			clone.numRepos = numRepos;
			return clone;

		}
		public bool FullCompare(StoreBoard obj2)
		{
			if (id == obj2.id &&
				len == obj2.len &&
				remain == obj2.remain &&
				remainPercent == obj2.remainPercent &&
				numRepos == obj2.numRepos &&
				count == obj2.count
				) return true;
			return false;
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
				obj1.numRepos == obj2.numRepos /*&&
				obj1.count == obj2.count*/

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


		//public new string ToString() { 

		//}
		public OrderBoard() { }
		public OrderBoard(OrderBoard ob)
		{
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

			};

		}
		public bool FullCompare(OrderBoard obj2)
		{
			if (id == obj2.id &&
				len == obj2.len &&
				count == obj2.count
				) return true;
			return false;
		}
		public bool Equals(OrderBoard other)
		{
			return this == other;
		}

		public static bool operator ==(OrderBoard obj1, OrderBoard obj2)
		{
			if (obj1.id == obj2.id &&
				obj1.len == obj2.len /*&&
				obj1.count == obj2.count*/
				) return true;
			return false;
		}

		public static bool operator !=(OrderBoard obj1, OrderBoard obj2)
		{
			if (obj1 == obj2) return false;
			return true;
		}
	}

	public class StoreList : List<StoreBoard>, IEnumerable<StoreBoard>
	{
		public StoreList(IEnumerable<StoreBoard> list) : base()
		{
			list.Select(el => { Add(el); return true; }).ToArray();
		}
		public StoreList(IEnumerable<int[]> elems) : base()
		{

			AddRange(elems.Select(row =>
			{
				return new StoreBoard()
				{
					id = row[0],
					len = row[1],
					count = row[2],
					remain = row[3],
					remainPercent = row[4],
					numRepos = row[5]
				};
			}).ToArray());
		}

		public void Subtract(OneBoardCombinations board)
		{
			this[IndexOf(board.board)].count -= board.board.count;
			ClearEmptyBoards();
		}

		public bool FullCompare(StoreList obj2)
		{

			var flag = true;
			obj2.ForEach(listEl =>
			{
				var counter = this.Aggregate(-1, (index, el) =>
				{
					if (listEl.FullCompare(el))
						return this.IndexOf(el);
					return index;
				});
				if (counter == -1)
					flag = false;
			});
			return flag;
		}
		private int ClearEmptyBoards()
		{
			int c = Count;
			RemoveAll(el => el.count == 0);
			this.Where(el => el.count < 0).Select(el =>
			{
				throw new Exception("Class Raspil.StoreList in method ClearEmptyOrderBoards() " +
					"element has negative counter"); return 0;
			}).ToArray();

			return c - Count;
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
	public class OrderList : List<OrderBoard>, ICloneable, IEnumerable<OrderBoard>, IEquatable<OrderList>
	{
		public OrderList() : base() { }

		public OrderList(IEnumerable<OrderBoard> list) : base()
		{
			list.Select(el => { Add(el); return true; }).ToArray();

		}

		public OrderList(IEnumerable<int[]> elems) : base()
		{
			int i = 0;
			AddRange(elems.Select(row =>
				new OrderBoard()
				{
					id = row[0],
					len = row[1],
					count = row[2],
				}
			).ToArray());
		}
		//public void Substitute(OrderBoard board)
		//{

		//	var idx = IndexOf(board);
		//	if (idx != -1)
		//	{

		//		this[idx].count = board.count;
		//	}
		//	else
		//	{
		//		Add(board);
		//	}
		//}
		public new void Add(OrderBoard board)
		{
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

		//public void Subtract(OneBoardCombinations board)
		//{

		//	this[IndexOf(board.board)].count -= board.board.count;
		//	ClearEmptyBoards();
		//}
		public void Subtract(IEnumerable<OrderList> rows)
		{
			rows.Select(collection =>
				collection.Select(el =>
			{
				this[IndexOf(el)].count -= el.count;
				return 0;
			}).ToArray()).ToArray();


			ClearEmptyBoards();

		}

		private int ClearEmptyBoards()
		{
			int c = 0;
			RemoveAll(el => { c++; return el.count == 0; });
			if (this.Where(el => el.count < 0).ToArray().Length > 0)
				throw new Exception("Class Raspil.OrderList in method ClearEmptyBoards() element has negative counter");
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
		public bool FullCompare(OrderList obj2)
		{

			var flag = Count == obj2.Count;
			if (!flag) return flag;
			obj2.ForEach(listEl =>
			{
				var counter = this.Aggregate(-1, (index, el) =>
				{
					if (listEl.FullCompare(el))
						return this.IndexOf(el);
					return index;
				});
				if (counter == -1)
					flag = false;
			});
			return flag;
		}

		public bool Equals(OrderList other)
		{
			return this == other;
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
