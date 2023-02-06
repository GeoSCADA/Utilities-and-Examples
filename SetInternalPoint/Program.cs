using System;
using System.Security;
using ClearScada.Client;

namespace SetInternalPoint
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length != 4)
			{
				Console.WriteLine("Usage: SetInternalPoint \"username\" \"password\" \"Point-Name\" \"Value\" ");
				return 1;
			}
			string user = args[0];
			string pass = args[1];
			string pointname = args[2];
			string valuetext = args[3];
			double valuedouble;
			if (!double.TryParse(valuetext, out valuedouble))
			{
				Console.WriteLine("Value is not numeric");
				return 1;
			}

			ClearScada.Client.Simple.Connection connection;
			var node = new ClearScada.Client.ServerNode("127.0.0.1", 5481);
			connection = new ClearScada.Client.Simple.Connection("Utility");
			try
			{
				connection.Connect(node);
			}
			catch (CommunicationsException)
			{
				Console.WriteLine("Unable to communicate with Geo SCADA server.");
				return 1;
			}
			if (!connection.IsConnected)
			{
				Console.WriteLine("Not connected to Geo SCADA server.");
				return 1;
			}
			using (var spassword = new System.Security.SecureString())
			{
				foreach (var c in pass)
				{
					spassword.AppendChar(c);
				}
				try
				{
					connection.LogOn(user, spassword);
				}
				catch (AccessDeniedException)
				{
					Console.WriteLine("Access denied, incorrect user Id or password");
					return 1;
				}
				catch (PasswordExpiredException)
				{
					Console.WriteLine("Credentials expired.");
					return 1;
				}
			}
			// Get point object
			ClearScada.Client.Simple.DBObject pointobject = null;
			try
			{
				pointobject = connection.GetObject(pointname);
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot get point object. " + e.Message);
				return 1;
			}
			// Set value
			try
			{
				object [] callparam = new object [1];
				callparam[0] = valuedouble;
				pointobject.InvokeMethod("CurrentValue", valuedouble);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error setting value. " + e.Message);
				return 1;
			}
			// Demo to read historic
			/*
			var pointObject2 = connection.GetObject("Example Projects.Oil and Gas.Transportation.Inflow Computer.GasFlow");
			DateTime hisStart = new DateTime(2021, 1, 19, 0, 0, 0);
			DateTime hisEnd = new DateTime(2021, 1, 20, 0, 0, 0);
			object[] hisArgs = { hisStart, hisEnd, 0, 1000, true, "All" };
			var hisResult = pointObject2.InvokeMethod("Historic.RawValues", hisArgs);
			Console.WriteLine(hisResult);
			Console.ReadKey();
			*/
			connection.Disconnect();
			return 0;
		}
	}
}
