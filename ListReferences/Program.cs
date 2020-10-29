using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearScada;

namespace ListReferences
{
	class Program
	{
		static void Main(string[] args)
		{
			if ( args.Length != 5)
			{
				Console.WriteLine("List the referenced items and types of all objects matching an SQL query\n" +
									"Arguments:\n" +
									"1 - Server IP/host\n" +
									"2 - Port\n" +
									"3 - User\n" +
									"4 - Password\n" +
									"5 - SQL query returning a list of FullNames\n" +
									" Example: ListReferences localhost 5481 freddy password \"select fullname from clogiccore\" \n" +
									" Output: object name, type, referenced object name, type ");
				return;
			}
			var node = new ClearScada.Client.ServerNode(ClearScada.Client.ConnectionType.Standard, args[0], int.Parse(args[1]));
			var connection = new ClearScada.Client.Simple.Connection("Utility");
			connection.Connect(node);
			var AdvConnection = node.Connect("UtilityA", false);
			using (var spassword = new System.Security.SecureString())
			{
				foreach (var c in args[3])
				{
					spassword.AppendChar(c);
				}
				connection.LogOn(args[2], spassword);
				AdvConnection.LogOn(args[2], spassword);
			}
			string sql = args[4];
			ClearScada.Client.Advanced.IQuery serverQuery = AdvConnection.PrepareQuery(sql, new ClearScada.Client.Advanced.QueryParseParameters());
			ClearScada.Client.Advanced.QueryResult queryResult = serverQuery.ExecuteSync(new ClearScada.Client.Advanced.QueryExecuteParameters());
			if (queryResult.Status == ClearScada.Client.Advanced.QueryStatus.Succeeded || queryResult.Status == ClearScada.Client.Advanced.QueryStatus.NoDataFound)
			{
				if (queryResult.Rows.Count > 0)
				{
					IEnumerator<ClearScada.Client.Advanced.QueryRow> e = queryResult.Rows.GetEnumerator();
					while (e.MoveNext())
					{
						string fullname = (string)e.Current.Data[0];
						var dbobject = connection.GetObject(fullname);
						var reflist = dbobject.GetReferencesFrom();
						if (reflist.Count == 0)
						{
							Console.WriteLine(fullname + "," + dbobject.ClassDefinition.Name + "," );
						}
						else
						{
							foreach ( var refobject in reflist)
							{
								Console.Write(fullname + "," + dbobject.ClassDefinition.Name + "," );
								Console.WriteLine(refobject.FullName + ", " + refobject.ClassDefinition.Name);
							}
						}
					}
				}
			}
			serverQuery.Dispose();
		}
	}
}
