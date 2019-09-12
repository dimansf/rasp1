
using Raspil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;


namespace ServerRaspil
{

	class Program
	{
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;

		/// <summary>
		///  Точка запуска сервера 
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			Runner(args);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private static void Runner2()
		{

			var server = new RaspilServer("49770");

			server.RunServer();
		}

		private static void Runner(string[] args)
		{
			RaspilServer server;
			try
			{
				server = new RaspilServer(args[0]);

			}
			catch
			{
				server = new RaspilServer("49770");
			}
			try
			{
				if (args[1] == "hide")
				{
					var win = GetConsoleWindow();
					ShowWindow(win, SW_HIDE);
				}

			}
			catch { }

			server.RunServer();
		}
		

		private static void prog2()
		{
			var handle = GetConsoleWindow();

			// Hide
			ShowWindow(handle, SW_HIDE);
			var rs = new RaspilServer(FreeTcpPort().ToString());
			rs.RunServer();
		}

		

		private static int FreeTcpPort()
		{
			TcpListener l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();
			int port = ((IPEndPoint)l.LocalEndpoint).Port;
			l.Stop();
			return port;
		}

	}
}
