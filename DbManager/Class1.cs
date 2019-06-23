using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManager
{
    public class PgProvider
    {
		string connStr = "Host={myserver};Username={mylogin};Password={mypass};Database={mydatabase}";


		NpgsqlConnection tie;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		/// <param name="dbName"></param>
		public PgProvider(string host, string user, string pass, string dbName) 
		{
			tie = new NpgsqlConnection(String.Format(connStr, host, user, pass, dbName));
			tie.Open();
		}

		public void perfQuery(string sqlQuery)
		{

			var cmd = new NpgsqlCommand("SELECT * FROM data", tie);
			var reader = cmd.ExecuteReader();
			while (reader.Read())
				Console.WriteLine(reader.GetString(0));
		}


    }
}
