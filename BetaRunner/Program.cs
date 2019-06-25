﻿using BagTasker;
using DataMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetaRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			var (orders, store) = Data.autoGeneratedData();

			var mapRaspil = new List<List<(int, (string, int, int))>>();

			var bt = new BagTask.BagTasker();

			var unicalID = orders.Select(row => row[2]).Distinct().ToList();
			var res = new List<Dictionary<(int, int), List<(int, CustomList)>>>();

			unicalID.ForEach(id =>
			{
				res.Add(bt.calculate(
				orders.Where(el => el[2] == id).ToArray(),
				store.Where(el => el[0] == id).ToArray(),
				false));
			});
			


		}
	}
}
