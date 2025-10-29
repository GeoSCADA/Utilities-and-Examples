# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8, 3.12
import clr
# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
#You could prompt user for this
connection.LogOn( "", "" ) # ENTER YOUR USERNAME AND PASSWORD HERE

#Type the group to be reference-checked here
#You could prompt user for this
GroupName = "$Root"
TypeName = "CMimic"
DeleteIt = False

GroupToRef = connection.GetObject(GroupName)

def GetRefs( MyObject, DeleteIt):
    c = 0
    # List references here
    ReferencedObjects = MyObject.GetReferencesTo()
    if (len(ReferencedObjects) == 0):
        print( "Object: ", MyObject.FullName, "(" + MyObject.ClassDefinition.Name +")")
        if (DeleteIt):
            # only delete if the text "Template" is not present in the name
            if ("template" not in MyObject.FullName.lower()):
                MyObject.Delete( False)
                print("Deleted")
            else:
                print("Did not delete")
        c += 1
    return c

def ListReferences( GroupToRef, TypeName, DeleteIt):
    c = 0
    # Get all child members of the root
    Children = GroupToRef.GetChildren( "","")

    # get all children
    for ChildObj in Children:
        #print (ChildObj.ClassDefinition.Name, TypeName)
        if (ChildObj.ClassDefinition.Name == TypeName):
            c += GetRefs( ChildObj, DeleteIt)
        # recurse down groups
        if (ChildObj.IsGroup):
            c += ListReferences(ChildObj, TypeName, DeleteIt)
    return c

c = ListReferences( GroupToRef, TypeName, DeleteIt)
print( "Found: " + str(c) + " with no references.")

