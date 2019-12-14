using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raspil;

namespace RaspilTests
{
	[TestClass]
	public class OneBoardCombinationsTest
	{


		int[][] listOrders = new int[][] {
			new int[] {  66, 2050, 3 },
			new int[] { 1965, 2055, 33 },
			new int[] { 8974, 2055, 165 }
		};

		int[][] listStores = new int[][] {
			new int[] {  66,3050, 3, 1000, 4, 1 },
			new int[] {  66,2050, 5, 1000, 4, 2 },
			new int[] {  66,6500, 20, 1000, 4, 5 },
			new int[] { 1965, 2200, 4, 1000, 5, 1 },
			new int[] { 8974, 2050, 2, 1000, 5, 2 },
			new int[] { 8974, 6500, 4, 1000, 5, 5 }

		};


		[TestMethod]
		public void Create()
		{

			var orders = new OrderList(listOrders);
			var stores = new StoreList(listStores);

			var onc = new OneBoardCombinations(stores[1]);

			onc.Add(orders);

		}

		[TestMethod]
		public void WidthSawSelect()
		{
			int[][] listOrders = new int[][] {
			new int[] {  66, 700, 3 }

		};

			int[][] listStores = new int[][] {
			new int[] {  66, 2106, 3, 1000, 4, 1 }

		};

			var orders = new OrderList(listOrders);
			var stores = new StoreList(listStores);

			var onc = new OneBoardCombinations(stores[0]);
			onc.Add(orders);

			Assert.AreEqual(0, onc.WidthSawSelect(3));
			Assert.AreEqual(1, onc.WidthSawSelect(4));

		}


		[TestMethod]
		public void GetBestOneBoardCombination()
		{
			int[][] listOrders = new int[][] {
			new int[] {  66, 700, 3 },


		};
			int[][] listOrders2 = new int[][] {

			new int[] {  66, 600, 3 }

		};

			int[][] listStores = new int[][] {
			new int[] {  66, 2106, 3, 1000, 4, 1 }

		};

			var orders = new OrderList(listOrders);
			var orders2 = new OrderList(listOrders2);
			var stores = new StoreList(listStores);

			var onc = new OneBoardCombinations(stores[0]);
			onc.Add(orders);
			onc.Add(orders2);


			var onc2 = new OneBoardCombinations(stores[0]);
			onc2.Add(orders);

			Assert.IsTrue(onc.GetBestOneBoardCombination().Equals(onc2));
			var onc3 = new OneBoardCombinations(stores[0]);
			onc3.Add(orders2);
			Assert.IsFalse(onc.GetBestOneBoardCombination().Equals(onc3));
		}


		[TestMethod]
		public void Duplicate()
		{
			int[][] listOrders = new int[][] {
			new int[] {  66, 700, 3 },
			new int[] {  66, 600, 3 }

		};
			int[][] listOrders2 = new int[][] {

			new int[] {  66, 600, 3 }

		};

			int[][] listStores = new int[][] {
			new int[] {  66, 2106, 3, 1000, 4, 1 }

		};

			var orders = new OrderList(listOrders);
			var stores = new StoreList(listStores);

			var onc = new OneBoardCombinations(stores[0]);
			onc.Add(orders);

			var onc2 = new OneBoardCombinations(stores[0]);
			onc2.Add(orders);

			Assert.IsTrue(onc.GetBestOneBoardCombination().Equals(onc2.GetBestOneBoardCombination()));

			var orders2 = new OrderList(listOrders2);
			var onc3 = new OneBoardCombinations(stores[0]);
			onc3.Add(orders2);
			Assert.IsFalse(onc.GetBestOneBoardCombination().Equals(onc3.GetBestOneBoardCombination()));

		}

		[TestMethod]
		public void LiquidSelect()
		{
			int[][] listOrders = new int[][] {
			new int[] {  66, 700, 2 },
			

		};
			int[][] listOrders2 = new int[][] {
			new int[] {  66, 500, 3 },


		};
			int[][] listOrders3 = new int[][] {
			new int[] {  66, 700, 3 },
		};
			
			int[][] listStores = new int[][] {
			new int[] {  66, 2106, 5, 1000, 4, 1 }

		};
			

			var onc = new OneBoardCombinations(new StoreList(listStores)[0]);
			
			onc.Add(new OrderList(listOrders));
			onc.Add(new OrderList(listOrders2));
			onc.Add(new OrderList(listOrders3));

			Assert.AreEqual(2, onc.LiquidSelect());
			
		}
		
		
	}
}
