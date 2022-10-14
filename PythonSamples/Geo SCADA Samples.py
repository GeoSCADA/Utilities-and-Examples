# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8, 3.9?
import clr
# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
connection.LogOn( "AdminExample", "your password" )

# Read Security ACLs
R = connection.GetObject("$Root")
ACL = R.GetSecurity()
if (ACL.InheritedFromParent == False):
    print( "ACL ", ACL.Count, " entries")
    for user in ACL.Keys:
        print( "Key: ", user, " ACL: ", ACL[ user] )

# instance a template
P = connection.GetObject("Test") #parent
T = connection.GetObject("Test.Test Bulk Data.Ten Points") # A template
I = P.CreateInstance(T, "New Instance") # Create an instance

# set string point value
P = connection.GetObject("Test.New String Point") # A string point
P.InvokeMethod("CurrentValue", "fred" )

# get object children
FieldDevice = connection.GetObject("Test")
Children = FieldDevice.GetChildren("CDBPoint","")
for value in Children:
    print(value.FullName)

# set an object's Geo Location
FieldDevice = connection.GetObject("SELogger.2820015789 DLLTE4-SA.Logger" )
GeoAgg = FieldDevice.Aggregates["GISLocationSource"];
print( GeoAgg.Enabled);
GeoAgg.ClassName = "CGISLocationSrcDynamic"
print( GeoAgg.ClassName);
GeoAgg["Latitude"] = 3
GeoAgg["Longitude"] = 4

# Find and set internal point values
pointObject = connection.GetObject("Example Projects.Oil and Gas.Transportation.Graphics.End Station.Valve 3.Position Control" )
for i in range(1,100,30):
    pointObject.InvokeMethod("CurrentValue", i )
    print( "Point set to: " + str(pointObject.GetProperty("CurrentValue" ) ) )

# Find a historic point      
pointObject2 = connection.GetObject("Example Projects.Oil and Gas.Transportation.Inflow Computer.GasFlow" )
from System import DateTime # To support .Net date/time
# Historic arguments are start, end, index(=0), maxrecords, forwards=true, reason="All"
hisStart = DateTime( 2021,1,19,0,0,0 )
hisEnd =   DateTime( 2021,1,20,0,0,0 )
hisArgs = [ hisStart, hisEnd, 0, 100, True, "All" ]
# Call methods to get values and times. Could also read quality, or use .ProcessedValue to get fixed interval data
hisValues = pointObject2.InvokeMethod("Historic.RawValues", hisArgs )
hisQualities = pointObject2.InvokeMethod("Historic.RawQualities", hisArgs )
hisTimeStamps = pointObject2.InvokeMethod("Historic.RawTimestamps", hisArgs )
for i in range( hisTimeStamps.Length):
    print( hisTimeStamps[i], hisValues[i], hisQualities[i] )
    
