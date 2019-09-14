
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
            prog1(args);
           
        }

        private static void prog1(string[] args)
        {
            AppExchangeClient clinet;
           

            var (orders, store) = Generator.GenerateJsonOrderStore(3);

            var xx = new ClientData
            {
                orders = orders,
                store = store,
                widthSaw = 3,
                algoritm = 2,
				singleFlag = false
			};

			var newData = JsonConvert.SerializeObject(xx);

			if (args.Length > 0)
				clinet = new AppExchangeClient("127.0.0.1", args[0]);
			else
				clinet = new AppExchangeClient("127.0.0.1", "49770");


            clinet.Send(newData);

            
            var d = clinet.Receive();

            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "out.txt"), d + "\n");
            d = clinet.Receive();
            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "out.txt"), d + "\n");
            Console.WriteLine(d);
            Console.ReadLine();


        }
    }

    
}
