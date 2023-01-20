using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearScada;

namespace SetStaticLocation
{
	class Program
	{
		static void Main(string[] args)
		{
			if ( args.Length != 7)
			{
				Console.WriteLine("Sets the static location of an object\n" +
									"Arguments:\n" +
									"0 - Server IP/host\n" +
									"1 - Port\n" +
									"2 - User\n" +
									"3 - Password\n" +
									"4 - Object Full Name\n" +
									"5 - Latitude\n" +
									"6 - Longitude\n" +
									" Example: SetStaticLocation localhost 5481 freddy password \"group1.group2.objectname\" 1.234 5.678");
				return;
			}
			var node = new ClearScada.Client.ServerNode(ClearScada.Client.ConnectionType.Standard, args[0], int.Parse(args[1]));
			var connection = new ClearScada.Client.Simple.Connection("UtilityB1");
			connection.Connect(node);
			var AdvConnection = node.Connect("UtilityB2", false);
			using (var spassword = new System.Security.SecureString())
			{
				foreach (var c in args[3])
				{
					spassword.AppendChar(c);
				}
				connection.LogOn(args[2], spassword);
				AdvConnection.LogOn(args[2], spassword);
			}
			string fullname = args[4];
			var dbobject = connection.GetObject(fullname);
			ClearScada.Client.Simple.Aggregate agg = dbobject.Aggregates["GISLocationSource"];
			agg.ClassName = "CGISLocationSrcStatic";
			//Console.WriteLine(fullname + "," + dbobject.ClassDefinition.Name + "," );
			double lat;
			double.TryParse(args[5], out lat);
			double lon;
			double.TryParse(args[6], out lon);
			agg.SetProperty("Latitude",  lat);
			agg.SetProperty("Longitude",  lon);
		}
	}
}
