using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Raspil;
using System.Collections.Generic;

namespace RaspilTests
{
	[TestClass]
	public class OrderListTest
	{

		int[][] listMOck = new int[][] {
			new int[] {  66, 2050, 3 },
			new int[] { 1965, 2055, 33 },
			new int[] { 8974, 2055, 165 }
		};

		[TestMethod]
		public void ListGenerate()
		{
			var res = new OrderList(listMOck);
			Assert.AreEqual(listMOck.Length, res.Count);
			//Assert.IsTrue(res[0].lineNumber == 1);
		}

		[TestMethod]
		public void Clone()
		{
			var orig = new OrderList(listMOck);
			var res = orig.Clone() as OrderList;
			Assert.AreNotSame(res, orig);
			Assert.IsTrue(res == orig);


		}
		[TestMethod]
		public void AdditionDuplicate()
		{
			var res = new OrderList(listMOck);
			var res2 = new OrderList(listMOck);
			res.AddRange(res2);

			Assert.AreEqual(res.Sum(el => el.count), res2.Sum(el => el.count) * 2);

		}

		//[TestMethod]
		public void Substitute()
		{
			var res = new OrderList(listMOck);
			var b = listMOck[0];
			var board = new OrderBoard() { id = b[0], len = b[1], count = 100 };
			//res.Substitute(board);

			Assert.AreEqual(1, res.Where(el => el.count == 100).Count());

		}
		

		[TestMethod]
		public void Subtract()
		{
			var res = new OrderList(listMOck);
			var sum = res.Sum(el => el.count);
			var b = listMOck[0];
			var board = new OrderBoard() { id = b[0], len = b[1], count = 1 };

			res.Subtract(new List<OrderList>() { new OrderList() { board } });
			Assert.AreEqual(sum - 1, res.Sum(el => el.count));

		}

		[TestMethod]
		public void ClearEmptyOrderBoards()
		{
			var res = new OrderList(listMOck);
			var sum = res.Count;
			var b = listMOck[0];
			var board = new OrderBoard() { id = b[0], len = b[1], count = 3 };
			res.Subtract(new List<OrderList>() { new OrderList() { board } });

			Assert.AreEqual(sum - 1, res.Count);
		}




	}

	[TestClass]
	public class StoreListTest
	{

		int[][] listMOck = new int[][] {
			new int[] {  66,3050, 3, 1000, 4, 1 },
			new int[] {  66,2050, 5, 1000, 4, 2 },
			new int[] {  66,6500, 20, 1000, 4, 5 },
			new int[] {  646,2200, 4, 1000, 5, 1 },
			new int[] {  646,2050, 2, 1000, 5, 2 },
			new int[] {  646,6500, 4, 1000, 5, 5 }

		};

		[TestMethod]
		public void ListGenerate()
		{
			var res = new StoreList(listMOck);
			Assert.AreEqual(listMOck.Length, res.Count);

		}
		
		[TestMethod]
		public void ListAddition()
		{
			var res = new StoreList(listMOck);
			var res2 = new StoreList(listMOck);
			res.AddRange(res2);

			Assert.AreEqual(res.Sum(el => el.count), res2.Sum(el => el.count) * 2);

		}
	}
}

