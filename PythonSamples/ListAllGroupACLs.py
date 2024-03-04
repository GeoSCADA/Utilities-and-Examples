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

# Functions to iterate over all groups and return permissions applied
# Note that for a large database this can take a long time to run and
# could affect other operations.

PermissionNames = ["Browse", "Configure", "Security", "SystemAdmin", "Control", "AcceptAlarms", "DisableAlarms", "Override", "Notes", "RetrieveData", "DisableControl", "Promote", "SwitchLine", "Tune", "Diagnostics", "ModifyHistoric", "AnnotateHistoric", "ValidateHistoric", "ViewAlarms", "Read", "ExclusiveControl", "ManageExclusiveControl", "RemoveAlarms", "EnableDisable", "ManualRedirect", "UnacceptAlarms", "AssignAlarms", "OnOffScan", "CancelRequest"]
def PermissionMaskToName( mask):
    p = ""
    for b in range(0,32):
        if (mask & (1<<b)):
            p = p + PermissionNames[b] + ", "
    return p[:-2]

def ListACLs( DBObj):
    ACL = DBObj.GetSecurity()
    if (ACL.InheritedFromParent == False):
        print( "Object", DBObj.FullName)
        print( "    ACL", ACL.Count, "entries")
        for user in ACL.Keys:
            print( "        User:", user, "ACL:",
                   ACL[ user].Permissions.ToObjectPermissions(),
                   "[", PermissionMaskToName( ACL[ user].Permissions.ToObjectPermissions()), "]" )
        return ACL.Count
    else:
        return 0

def RecurseACLs( Group):
    c = 0
    # Get all child members of the root
    Children = Group.GetChildren( "","")

    # get all children
    for ChildObj in Children:
        c += ListACLs( ChildObj)
        # recurse down groups
        if (ChildObj.IsGroup):
            c += RecurseACLs(ChildObj)
    return c

R = connection.GetObject("$Root")
c = ListACLs( R)
c += RecurseACLs( R)
print( "Found: " + str(c) + " ACLs.")

