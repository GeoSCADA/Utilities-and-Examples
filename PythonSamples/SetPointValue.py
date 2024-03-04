# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8, 3.12
import clr
# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
connection.LogOn( "", "" ) # ENTER YOUR USERNAME AND PASSWORD HERE

# Find and set internal point values
pointObject = connection.GetObject("Example Projects.Oil and Gas.Transportation.Graphics.End Station.Valve 3.Position Control" )
pointObject.InvokeMethod("CurrentValue", 5.678 )
print( "Point set to: " + str(pointObject.GetProperty("CurrentValue" ) ) )

