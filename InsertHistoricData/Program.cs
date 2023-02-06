using System;
using ClearScada.Client;
using System.Security;

namespace InsertHistoricData
{
	class Program
	{
		static void Main()
		{
			// EDIT YOUR CREDENTIALS, OR PASS IN AS ARGUMENTS - PLEASE CHOOSE YOUR OWN METHOD
			string user = "";
			string pass = "";

			ClearScada.Client.Simple.Connection connection;
			var node = new ClearScada.Client.ServerNode("127.0.0.1", 5481);
			connection = new ClearScada.Client.Simple.Connection("Utility");
			connection.Connect(node);
			var spassword = new System.Security.SecureString();
			foreach (var c in pass)  spassword.AppendChar(c); 
			connection.LogOn(user, spassword);

			// Insert point name here
			ClearScada.Client.Simple.DBObject PointObj = connection.GetObject("New Analog Point");
			DateTime now = DateTime.UtcNow;

			// Load a year's values
			DateTimeOffset Time = new DateTimeOffset(2022, 1, 1, 0, 30, 0, TimeSpan.Zero); // Start of last year + 30 Min
			double Value = 0;
			do
			{
				Object[] p1 = new Object[4];
				p1[0] = 1;
				p1[1] = 192;
				p1[2] = Time;
				p1[3] = Value;
				PointObj.Aggregates["Historic"].InvokeMethod("LoadDataValue", p1);

				Value += 0.01;
				if ((Value - Math.Floor(Value)) > 0.235) Value = Math.Floor(Value + 1);
				Time += new TimeSpan(1, 0, 0); // 1H
			} while (Time < DateTime.UtcNow);

#if false
			// Various calls to read values back
			Object[] p2 = new Object[5];
			p2[0] = now.AddSeconds(-1);
			p2[1] = now.AddSeconds(1);
			p2[2] = 0;
			p2[3] = true;
			p2[4] = "All";
			object r = PointObj.Aggregates["Historic"].InvokeMethod("RawValue", p2);
			Console.WriteLine(r);

			Object[] p3 = new Object[6];
			p3[0] = now.AddSeconds(-1);
			p3[1] = now.AddSeconds(1);
			p3[2] = 0;
			p3[3] = 1;
			p3[4] = true;
			p3[5] = "All";
			object [] k = (object [])PointObj.Aggregates["Historic"].InvokeMethod("RawValues", p3);
			Console.WriteLine(k[0]);

			Object[] p4 = new Object[6];
			p4[0] = now.AddSeconds(-1);
			p4[1] = "2S";
			p4[2] = 0;
			p4[3] = 1;
			p4[4] = true;
			p4[5] = "All";
			object[] q = (object[])PointObj.Aggregates["Historic"].InvokeMethod("RawValuesRange", p4);
			Console.WriteLine(q[0]);
#endif
			Console.ReadKey();
		}
	}
}
