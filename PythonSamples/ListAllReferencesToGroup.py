# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8, 3.12
import clr
# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
# From 2021:
node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
# Older versions:
#node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
#You could prompt user for this
connection.LogOn( "", "" ) # ENTER YOUR USERNAME AND PASSWORD HERE

#Type the group to be reference-checked here
#You could prompt user for this
GroupName = "Example Projects.Oil and Gas"

GroupToRef = connection.GetObject(GroupName)

def GetRefs( MyObject, GroupName):
    c = 0
    # List references here
    ReferencedObjects = MyObject.GetReferencesTo()
    # for all referenced
    for RefObj in ReferencedObjects:
        # if the path lies outside our group
        if (not RefObj.FullName.startswith( GroupName + ".")):
            print( "Object: ", MyObject.FullName, "(" + MyObject.ClassDefinition.Name +")")
            print( "  Reference: ", RefObj.FullName, "("+RefObj.ClassDefinition.Name +")")
            c += 1
    return c

def ListReferences( GroupToRef, GroupName):
    c = 0
    # Get all child members of the root
    Children = GroupToRef.GetChildren( "","")

    # get all children
    for ChildObj in Children:
        c += GetRefs( ChildObj, GroupName)
        # recurse down groups
        if (ChildObj.IsGroup):
            c += ListReferences(ChildObj, GroupName)
    return c

c = ListReferences( GroupToRef, GroupName)
print( "Found: " + str(c) + " references.")

