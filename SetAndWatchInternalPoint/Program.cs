using System;
using System.Security;
using System.Threading.Tasks;
using ClearScada.Client; // Find ClearSCADA.Client.dll in the Program Files\Schneider Electric\ClearSCADA folder
using ClearScada.Client.Advanced;

namespace SetInternalPoint
{
	class Program
	{
		static bool WaitUntilStopped = true;
		async static Task Main(string[] args)
		{
			if (args.Length != 5)
			{
				Console.WriteLine("Usage: SetAndWatchInternalPoint \"username\" \"password\" \"Point-Name\" \"Value\" \"Watch-Point-Name\" ");
				return;
			}
			string user = args[0];
			string pass = args[1];
			string pointname = args[2];
			string valuetext = args[3];
			string watchpointname = args[4];
			double valuedouble;
			if (!double.TryParse(valuetext, out valuedouble))
			{
				Console.WriteLine("Value is not numeric");
				return;
			}

			ClearScada.Client.Simple.Connection connection;
			var node = new ClearScada.Client.ServerNode("127.0.0.1", 5481);
			connection = new ClearScada.Client.Simple.Connection("Utility");
			IServer AdvConnection;
			try
			{
				connection.Connect(node);
				var ConSet = new ClientConnectionSettings();
				AdvConnection = node.Connect("Utility", ConSet);
			}
			catch (CommunicationsException)
			{
				Console.WriteLine("Unable to communicate with Geo SCADA server.");
				return;
			}
			if (!connection.IsConnected)
			{
				Console.WriteLine("Not connected to Geo SCADA server.");
				return;
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
					AdvConnection.LogOn(user, spassword);
				}
				catch (AccessDeniedException)
				{
					Console.WriteLine("Access denied, incorrect user Id or password");
					return;
				}
				catch (PasswordExpiredException)
				{
					Console.WriteLine("Credentials expired.");
					return;
				}
			}

			// Set event callback
			AdvConnection.TagsUpdated += TagUpdateEvent;

			// Disconnect Callback
			AdvConnection.StateChanged += DBStateChangeEvent;
			AdvConnection.AdviseStateChange();
			
			// Register for change for this point
			// ReadSeconds is the interval for the Server to watch for change. Be careful not to set this small
			int ReadSeconds = 10;
			try
			{
				int RegisterId = 1; // Use a unique number for each point waited for (not needed to be same as Object Id)
				// Registration is by point name, specify the field which has to change (could use CurrentTime to catch every change).
				AdvConnection.AddTags(new TagDetails(RegisterId, watchpointname + ".CurrentValue", new TimeSpan(0, 0, ReadSeconds)));
			}
			catch (ClearScada.Client.CommunicationsException)
			{
				Console.WriteLine("*** Comms exception (AddTags)");
				return;
			}

			// Get point object to be modified
			ClearScada.Client.Simple.DBObject pointobject = null;
			try
			{
				pointobject = connection.GetObject(pointname);
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot get point object. " + e.Message);
				return;
			}
			// Set value
			try
			{
				pointobject.InvokeMethod("CurrentValue", valuedouble);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error setting value. " + e.Message);
				return;
			}

			// Wait and watch for changes
			Console.WriteLine("Waiting for changes");
			while (WaitUntilStopped)
			{
				await Task.Delay(1000);
			}



			// Demo to read historic data
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
			return;
		}


		// Callback - Tag Update
		static void TagUpdateEvent(object sender, TagsUpdatedEventArgs EventArg)
		{
			Console.WriteLine($"Received {EventArg.Updates.Count} update(s).");
			foreach (var Update in EventArg.Updates)
			{
				// You cannot call any Geo SCADA Client interface methods here
				// So if you want to read more data as a result of this change,
				// Please use a queue and process data later.
				Console.WriteLine($"Update: Id {Update.Id}, Status {Update.Status}, Value {Update.Value}, Quality {(long)Update.Quality}, Time {Update.Timestamp}");
			}
		}


		// Called when server state changes, stop the wait
		static void DBStateChangeEvent(object sender, StateChangeEventArgs stateChange)
		{
			if (stateChange.StateDetails.State != ServerState.Main)
			{
				WaitUntilStopped = false;
			}
		}
	}
}
