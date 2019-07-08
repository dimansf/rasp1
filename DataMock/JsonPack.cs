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
        public static string packGeneratedData(int[][] order, int[][] store)
        {
            var jw = new JsonWrapper
            {
                order = order,
                store = store
            };

            string ord = JsonConvert.SerializeObject(jw);
           

            var x = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
            File.WriteAllText(x, ord);
            return x;
            
        }
    }

    public class JsonWrapper
    {
        public int[][] order;
        public int[][] store;
    }

}
