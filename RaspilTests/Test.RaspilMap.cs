using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raspil;

namespace RaspilTests
{
	[TestClass]
	public class RaspilMapTest
	{

		int[][] listOrders = new int[][] {
			new int[] {  66, 700, 3 },
			new int[] {  66, 600, 3 }

		};
		int[][] listOrders3 = new int[][] {
			new int[] {  66, 600, 3 },
			new int[] {  66, 700, 3 }
			

		};
		int[][] listOrders2 = new int[][] {

			new int[] {  66, 600, 2 }

		};

		int[][] listStores = new int[][] {
			new int[] {  66, 2106, 3, 1000, 4, 1 }

		};

		[TestMethod]
		public void Add()
		{

			var rm =  new RaspilMap();

			var onc = new OneBoardCombinations(new StoreList(listStores)[0]);

			onc.Add(new OrderList(listOrders));
			

			var onc2 = new OneBoardCombinations(new StoreList(listStores)[0]);

			onc2.Add(new OrderList(listOrders3));
			

			var onc3 = new OneBoardCombinations(new StoreList(listStores)[0]);

			
			onc3.Add(new OrderList(listOrders2));

			rm.Add(1, new List< OneBoardCombinations >() { onc});
			rm.Add(1, new List<OneBoardCombinations>() { onc2 });
			rm.Add(1, new List<OneBoardCombinations>() { onc3 });

			Assert.AreEqual(2, rm[1].Count);


			Assert.AreEqual(2, rm[1][0].board.count);



		}
		[TestMethod]
		public void Add2()
		{

		}
	}
}
