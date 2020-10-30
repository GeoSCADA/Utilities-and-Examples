using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScxV6DbClient;

namespace InsertHistoryCOMAPI
{
	class Program
	{
		static void Main(string[] args)
		{
			ScxV6Server objServer = new ScxV6Server();
			objServer.Connect("Local", "s", "s");
			ScxV6Object obj = objServer.FindObject("New Analog Point");
			ScxV6Aggregate His = obj.Aggregate["Historic"];
			His.Interface.LoadDataValue(1, 192, DateTime.UtcNow, 13);
			obj = null;
			objServer.Disconnect();
		}
	}
}
