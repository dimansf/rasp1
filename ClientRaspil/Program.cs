
using DataGenerator;
using Net;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ClientRaspil
{
	class Program
    {
       
        static void Main(string[] args)
        {
			switch (args[0])
			{
				case "1": prog1(args);
					break;
				case "2": prog2(args);
					break;
				case "3":
					prog3(args);
					break;
			}
           
        }

		private static void prog1(string[] args)
		{


			var (orders, store) = Generator.GenerateJsonOrderStore(3);

			var xx = new ClientData
			{
				orders = orders,
				store = store,
				widthSaw = 3,

				scladMax = true,
				optimize = false
			};

			var newData = JsonConvert.SerializeObject(xx);

			MainAction(args, newData);


		}
		private static void prog2(string[] args)
		{
			MainAction(args, Generator.case2);

		}
		private static void prog3(string[] args)
		{
			var path = Directory.GetCurrentDirectory();
			MainAction(args, File.ReadAllText(Path.Combine(path, "../../resources/json1.txt")));

		}


		private static void MainAction(string[] args, string newData) {
			AppExchangeClient clinet;


			if (args.Length > 1)
				clinet = new AppExchangeClient("127.0.0.1", args[1]);
			else
				clinet = new AppExchangeClient("127.0.0.1", "49770");


			clinet.Send(newData);


			var d = clinet.Receive();

			//File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "out.txt"), d + "\n");
			d = clinet.Receive();
			//File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "out.txt"), d + "\n");
			Console.WriteLine(d);
			Console.ReadLine();
		}
		


    }

    
}
