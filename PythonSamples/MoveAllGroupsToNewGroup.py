# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8 
import clr
# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
connection.LogOn( "AdminExample", "enter your password here" )

NewGroupName = "H"

Root = connection.GetObject("$Root")
# Create a new parent group
if (connection.GetObject(NewGroupName) == None):
    NewParent = connection.CreateObject( "CGroup", Root.Id, NewGroupName)
else:
    NewParent = connection.GetObject( NewGroupName)
    
# Get all child groups of the root
Children = Root.GetChildren( "","")

#Move all children, except ours
for ChildObj in Children:
    if (ChildObj != NewGroupName):
        ## connection.MoveObject( ChildObj.Id, NewParent.Id)
        print( "Moved: ", ChildObj.FullName, ChildObj.Id, NewParent.Id)

