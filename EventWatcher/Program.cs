using System;
using System.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearScada.Client; // Find ClearSCADA.Client.dll in the Program Files\Schneider Electric\ClearSCADA folder
using ClearScada.Client.Advanced;

namespace EventWatcher
{
	class Program
	{
		static bool WaitUntilStopped = true;
		async static Task Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Usage: EventWatcher \"username\" \"password\" ");
				return;
			}
			string user = args[0];
			string pass = args[1];

			ClearScada.Client.Simple.Connection connection;
			// Older Geo SCADA uses param: ClearScada.Client.ConnectionType.Standard
			var node = new ClearScada.Client.ServerNode("127.0.0.1", 5481);
			connection = new ClearScada.Client.Simple.Connection("Utility");
			IServer AdvConnection;
			try
			{
				connection.Connect(node);
				//AdvConnection = node.Connect("SeverityCleaner");									// Up to v80
				//AdvConnection = node.Connect("SeverityCleaner", false);							// From v81 to v84
				var conSettings = new ClientConnectionSettings();                                   // From v85 onwards
				conSettings.IsLimited = false;                                                      // From v85 onwards
				conSettings.IsVirtualized = false;                                                  // From v85 onwards
				AdvConnection = node.Connect("SeverityCleaner", conSettings);                       // From v85 onwards
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
			// subscriptionId is just a unique Id, we just use  a Guid (you can make many subscriptions if you want)
			var subscriptionId = Guid.NewGuid().ToString();

			// Filter string is the Server side Alarm Filter String that you can configure in ViewX
			var filter="Categories=\"PointState;Security;Action\"";

			// OPCEventCategory allows you to request properties are sent along with events.
			// The CategoryId 0x208 identifies the EventCategory and comes from the Database.
			// The opcvalue is a an OPC property number from the schema
			///OPCEventCategory category1 = new OPCEventCategory(0x208); // Analogue alarm
			///category1.OPCAttributes.Add(OPCAttr1);
			///category1.OPCAttributes.Add(OPCAttr2);
			///List<OPCEventCategory> OPCCategories = new List<OPCEventCategory>();
			///OPCCategories.Add(category1);
			// And add OPCCategories to the AddEventSubscription call below

			// Register the Event Handler 
			AdvConnection.EventsUpdated += OnEventSubscriptionEvents;

			// Disconnect Callback
			AdvConnection.StateChanged += DBStateChangeEvent;
			AdvConnection.AdviseStateChange();


			// Subscribe to Events
			AdvConnection.AddEventSubscription(subscriptionId, filter); ///, OPCCategories);

			// Wait and watch for changes
			Console.WriteLine("Waiting for changes");
			while (WaitUntilStopped)
			{
				await Task.Delay(1000);
			}

			connection.Disconnect();
			return;
		}


		static void OnEventSubscriptionEvents(object sender, EventsUpdatedEventArgs arg)
		{
			// If there are two different sub's open at the same time, they will both be seeing each others events.
			//	if (string.CompareOrdinal(arg.ClientId, eventSubscriptionId) != 0)
			//	return; 

			foreach (EventData f in arg.Updates)
			{
				// Do stuff with the events
				string AttrList = "";
				foreach( var AttrItem in f.EventAttributes)
				{ 
					AttrList += ", " + (AttrItem ?? "-").ToString();
				}
				Console.WriteLine($"{f.Time} {f.Message}, {f.EventCategory}, {f.Severity}, {f.Source}, {f.EventCategory}  {AttrList}");
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
