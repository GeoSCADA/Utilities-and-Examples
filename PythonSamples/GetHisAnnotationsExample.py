# Import .Net runtime support - needs "pip install pythonnet" (Use Python 3.8)
import clr
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 
import time
from System import DateTime # Support .Net date/time

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
connection.LogOn( "", "" ) # ENTER YOUR USERNAME AND PASSWORD HERE

# Find a historic point      
pointObject2 = connection.GetObject("Test.New Analog Point" )

hisStart = DateTime( 2021, 1, 1,0,0,0 ) # 2 years
hisEnd =   DateTime( 2022,12,31,0,0,0 )

hisArgs = [ hisStart, hisEnd ]
annCount = pointObject2.InvokeMethod("Historic.AnnCount", hisArgs )
print("Annotations: ", annCount)

if annCount > 0:
    print( "Data time", "User", "Comment Time", "Comment")
    for i in range(0, annCount):
        hisArgs = [ hisStart, hisEnd, i, True ]
        annComment = pointObject2.InvokeMethod("Historic.AnnComment", hisArgs )
        annModifyTime = pointObject2.InvokeMethod("Historic.AnnModifyTime", hisArgs )
        annTimestamp = pointObject2.InvokeMethod("Historic.AnnTimestamp", hisArgs )
        annUser = pointObject2.InvokeMethod("Historic.AnnUser", hisArgs )
        print( annTimestamp, annUser, annModifyTime, annComment )
    


