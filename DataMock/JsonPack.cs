using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataMock
{
    public class JsonPack
    {
        public static (string,string) packGeneratedData(int[][] order, int[][] store)
        {
            string ord = JsonConvert.SerializeObject(order);
            string stro = JsonConvert.SerializeObject(store);

            var x = Path.Combine(Directory.GetCurrentDirectory(), "order.json");
            var y = Path.Combine(Directory.GetCurrentDirectory(), "store.json");
            File.WriteAllText(x, ord);
            File.WriteAllText(y, stro);
            return (x, y);
            
        }
    }
}
