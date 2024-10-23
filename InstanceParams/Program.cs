using System;
using System.Collections.Generic;
using System.Security;
using ClearScada.Client;

namespace InstanceParams
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length != 5)
			{
				Console.WriteLine("Usage: InstanceParams \"username\" \"password\" \"Instance-Name\" \"Parameter-Name\" \"Value\" ");
				return 1;
			}
			string user = args[0];
			string pass = args[1];
			string instancename = args[2];
			string paramname = args[3];
			string valuetext = args[4];
			double valuedouble;
			if (!double.TryParse(valuetext, out valuedouble))
			{
				Console.WriteLine("Value is not numeric");
				//return 1;
			}

			ClearScada.Client.Simple.Connection connection;
#pragma warning disable 612, 618
			var node = new ClearScada.Client.ServerNode(ConnectionType.Standard, "127.0.0.1", 5481);
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
#pragma warning restore 612, 618
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
			// Get instance object
			ClearScada.Client.Simple.DBObject instanceobject = null;
			try
			{
				instanceobject = connection.GetObject(instancename);
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot get instance object. " + e.Message);
				return 1;
			}
			// Set find parameter
			InstanceVariablesCollection evars = null;
			try
			{
				evars = instanceobject.GetInstanceVariables();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error reading variables - is this an instance?" + e.Message);
				return 1;
			}
			foreach( var expVar  in evars)
			{
				Console.WriteLine("Parameter: " + expVar.Key + ", Current Value: " + expVar.Value.ToString() + ", Type: " + expVar.Value.VariableType);
				if (expVar.Key == paramname)
				{
					InstanceVariablesCollection newVarCol = new InstanceVariablesCollection();
					if (expVar.Value.VariableType == ExpressionVariableType.String)
					{
						InstanceVariable newVar = new InstanceVariable(valuetext, "", "", ExpressionVariableType.String);
						Console.WriteLine("Changing to: " + valuetext);
						newVarCol.Add(paramname, newVar);
						instanceobject.SetInstanceVariables(newVarCol);
					}
					if (expVar.Value.VariableType == ExpressionVariableType.Double)
					{
						InstanceVariable newVar = new InstanceVariable(valuedouble, "", "", ExpressionVariableType.Double);
						Console.WriteLine("Changing to: " + valuedouble);
						newVarCol.Add(paramname, newVar);
						instanceobject.SetInstanceVariables(newVarCol);
					}
				}
			}

			connection.Disconnect();
			return 0;
		}
	}
}
