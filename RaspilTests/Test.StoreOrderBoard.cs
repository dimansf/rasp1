using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raspil;

namespace RaspilTests
{
	[TestClass]
	public class StoreBoardTest
	{

		StoreBoard board1 = new StoreBoard() { id = 1, count = 2, len = 1700,  numRepos = 5, remain = 300, remainPercent = 5 };
		StoreBoard board2 = new StoreBoard() { id = 1, count = 2, len = 1700,  numRepos = 5, remain = 300, remainPercent = 4 };
		StoreBoard board3 = new StoreBoard() { id = 1, count = 52, len = 1700,  numRepos = 5, remain = 300, remainPercent = 5 };
		StoreBoard board4 = new StoreBoard() { id = 1, count = 2, len = 1700,  numRepos = 5, remain = 250, remainPercent = 5 };

		[TestMethod]
		public void EqualTest()
		{
			Assert.IsTrue(board1 != board2);
			Assert.IsTrue(board1 == board3);
			Assert.IsTrue(board2 != board3);
			Assert.IsTrue(board1 != board4);
		}

		//[TestMethod]
		//public void NotEqualTest()
		//{
			

		//}

		[TestMethod]
		public void CloneTest()
		{
			var obj2 = (StoreBoard) board1.Clone();

			Assert.AreNotSame(obj2, board1);
			Assert.IsTrue(obj2 == board1);
		}
	}


	[TestClass]
	public class OrderBoardTest
	{

		OrderBoard board1 = new OrderBoard() { id = 1, count = 2, len = 15500 };
		OrderBoard board2 = new OrderBoard() { id = 1, count = 2, len = 1500};
		OrderBoard board3 = new OrderBoard() { id = 1, count = 3, len = 1500 };

		[TestMethod]
		public void EqualTest()
		{
			Assert.IsTrue(board1 != board2);
			Assert.IsTrue(board2 == board3);

		}

		//[TestMethod]
		//public void NotEqualTest()
		//{
			
		//}


		[TestMethod]
		public void CloneTest()
		{
			var obj2 = board1.Clone() as OrderBoard;

			Assert.AreNotSame(obj2, board1);
			Assert.IsTrue(obj2 == board1);

		}
	}
}
