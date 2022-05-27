# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8 
import clr
# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 
import time
from System import DateTime # Support .Net date/time

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
connection.LogOn( "AdminExample", "enter your password here" )

# Open file

# Read every line

# set analog point value
P = connection.GetObject("Data Exchange.ExtDb.Logger 23 Flow Rate")
P["PresetQuality"] = 192 # Good quality value
P["PresetTimestamp"] = DateTime( 2022,4,22,12,30,0 ) # Set a time before now
P.InvokeMethod("CurrentValue", 3.14 ) # Set value using above time and quality

