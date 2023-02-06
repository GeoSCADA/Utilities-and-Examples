# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8...
import clr
# Import ODBC support - needs "pip install pyodbc"
import pyodbc

# Server Info and User creds - please edit these as needed
serverIP = "127.0.0.1"
serverPort = 5481
systemName = "Local"

# Access details for your system - IMPORTANT - Set the user object's Severity to None,
# or your system will be swamped by event logging!

# ENTER CREDENTIALS HERE FOR A SIMULATOR USER - YOU ARE RESPONSIBLE FOR ALTERING THIS
# AND KEEPING CREDENTIALS SAFE.
username = ""
password = ""

# Simulation load
updatesPerBatch = 250 # Edit to change how many points change in a batch
intervalBetwenBatches = 2 # Seconds delay per batches
countOfBatches = 20 # Number of batches to run, so this limits length of simulation
# i.e. 1000,5,20 will simulate 20,000 changes over 100 seconds, plus the release time.

# NOTE - THIS PROGRAM WILL CHANGE YOUR DATABASE - DO NOT RUN ON A LIVE SYSTEM

# When run the first time, points will be reconfigured and all overridable points
# will be modified, it then does a trial override of ALL points to check all that
# can be, then it writes that list to two json files in the local path so that
# the next run of the program is quicker. If you load/reload a database then
# delete the two json files and restart.
# So: the first run could take a very long time on a large database, but it will
# still try to change data at the rate requested.

# Point Filter - restrict reconfig and override of points to those matching this SQL
pointFilter = "FullName LIKE '%' "

# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Other Libs
import random
import json
import os.path
import datetime
import time

# Our class for the simulation by override load test
class OverrideSim:
    def __init__(self, pointFilter, updatesPerBatch, intervalBetwenBatches, countOfBatches):
        self.pointFilter = pointFilter
        self.updatesPerBatch = updatesPerBatch
        self.intervalBetwenBatches = intervalBetwenBatches
        self.countOfBatches = countOfBatches
        
        print(datetime.datetime.now(), "Connecting")
        # Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
        node = CSClient.ServerNode( CSClient.ConnectionType.Standard, serverIP, serverPort )
        self.connection = CSClient.Simple.Connection( "OverrideSim" )
        self.connection.Connect( node )
        self.Advconnection = self.connection.Server
        self.connection.LogOn( username, password )

        # Log into ODBC for queries
        self.conn = pyodbc.connect(r'Driver={ClearSCADA Driver};LOCATION=' + systemName +
                              r';UID=' + username +
                              r';PWD=' + password)

    def ReadTableDef(self):
        cursor = self.conn.cursor()
        print("Read Table definitions")
        # List point types in the schema with analog minimum/maximum
        cursor.execute("select Table from dbfielddef where Name = 'OverrideMinimum' and table not in ('CPointAlgManual','CPointAlgCalc')")
        self.analogoverridetables = cursor.fetchall()
        # List point types in the schema with digital state override
        cursor.execute("select Table from dbfielddef where Name = 'State1Override' and table not in ('CPointDigitalManual','CPointDigitalCalc')")
        self.digitaloverridetables = cursor.fetchall()


    def EnableAnalogOverride(self, Id, fs, zs):
        try:
            #print("CHANGE Ag:", Id)
            OId = CSClient.ObjectId( Id)
            self.Advconnection.SetProperties (OId, ["OverrideAllowed"],[True])
            self.Advconnection.SetProperties (OId, ["OverrideMaximum","OverrideMinimum"],[fs,zs])
            return 1
        except CSClient.AccessDeniedException:
            #print("Cannot set properties:", Id)
            return 0

    def EnableDigitalOverride(self, Id):
        try:
            #print("CHANGE Dg:", Id)
            OId = CSClient.ObjectId( Id)
            self.Advconnection.SetProperties (OId, ["State0Override","State1Override"],[True,True])
            return 1
        except CSClient.AccessDeniedException:
            #print("Cannot set properties:", Id)
            return 0

    def FindSetOverrideConfig(self, TemplatesOnly):
        errorcount=0
        successcount=0
        cursor = self.conn.cursor()
        print("Find and enable analog overrides")
        # Read points which can be NOT overridden - InService but not allowed
        CountAnalogs = 0
        for trow in self.analogoverridetables:
            # List points in each table
            #print("Table",trow[0] )
            try:
                q = "select with templates Id,FullName,OverrideMinimum,OverrideMaximum,FullScale,ZeroScale from " + trow[0] + \
                               " where InService = True And ConfigValid = True and " + \
                               "(OverrideAllowed = False or OverrideMinimum <> ZeroScale or OverrideMaximum <> FullScale)" + \
                               " AND " + self.pointFilter
                if (TemplatesOnly):
                    q += " AND ParentTemplateId > 0"
                cursor.execute(q )
                for prow in cursor.fetchall():
                    #print( 'Modify', prow[1] )
                    s = self.EnableAnalogOverride( prow[0], prow[4], prow[5])
                    CountAnalogs += s
                    if (s == 0):
                        errorcount+=1
                    else:
                        successcount+=1
                    if ((errorcount + successcount) % self.updatesPerBatch == 0):
                        print("datetime.datetime.now(), Configured:", successcount, "Unsuccessful:", errorcount)
                        # Wait
                        time.sleep( self.intervalBetwenBatches )
            except pyodbc.Error:
                print("No suitable columns in table?",trow[0])
        print(datetime.datetime.now(), "Analogs Changed", CountAnalogs)

        print("Find and enable digital overrides")
        # Read points which can NOT be overridden - InService but not allowed
        CountDigitals = 0
        for trow in self.digitaloverridetables:
            # List points in each table
            #print("Table",trow[0] )
            try:
                q = "select with templates Id,FullName from " + trow[0] + \
                               " where InService = True And ConfigValid = True and " + \
                               " (State0Override = False or State1Override = False)" + \
                               " AND " + self.pointFilter

                if (TemplatesOnly):
                    q += " AND ParentTemplateId > 0"
                cursor.execute(q )
                for prow in cursor.fetchall():
                    #print( 'Modify', prow[1] )
                    s = self.EnableDigitalOverride( prow[0])
                    CountDigitals += s
                    if (s == 0):
                        errorcount+=1
                    else:
                        successcount+=1
                    if ((errorcount + successcount) % self.updatesPerBatch == 0):
                        print(datetime.datetime.now(), "Configured:", successcount, "Unsuccessful:", errorcount)
                        # Wait
                        time.sleep( self.intervalBetwenBatches )
            except pyodbc.Error:
                print("No suitable columns in table?",trow[0])
        print(datetime.datetime.now(), "Digitals Changed", CountDigitals)

        #Return # of changes
        return (CountAnalogs + CountDigitals)
    

    def ModifyAllOverrideConfig(self):
        # Will change the database, first do templates
        while ( OS.FindSetOverrideConfig( TemplatesOnly = True) > 0):
            print( "Reconfiguring templates")
        # Now non-templated
        while (OS.FindSetOverrideConfig( TemplatesOnly = False) > 0):
            print( "Reconfiguring points")

    # Find Points which have override facility
    def FindOverridablePoints(self):
        cursor = self.conn.cursor()

        # Point Names, Range, Details
        self.analogpointrefs = []
        self.digitalpointrefs = []

        # Read points which can be overridden - InService
        for trow in self.analogoverridetables:
            # List points in each table
            #print("Table",trow[0] )
            try:
                cursor.execute("select Id,FullName,OverrideMinimum,OverrideMaximum from " + trow[0] + \
                               " where InService = True And ConfigValid = True and OverrideAllowed = True" + \
                               " AND " + self.pointFilter )
                for prow in cursor.fetchall():
                    #print( prow[0] )
                    algdata = {"id":prow[0],"FullName":prow[1],"OverrideMinimum":prow[2],"OverrideMaximum":prow[3]}
                    self.analogpointrefs.append( algdata )
            except pyodbc.Error:
                print("No suitable columns in table",trow[0])

        # Read points which can be overridden - InService
        for trow in self.digitaloverridetables:
            # List points in each table
            #print("Table",trow[0] )
            try:
                # Assume only 1 bit
                cursor.execute("select Id,FullName from " + trow[0] + \
                               " where InService = True And ConfigValid = True " + \
                               " and State0Override = True and State1Override = True" + \
                               " AND " + self.pointFilter )
                for prow in cursor.fetchall():
                    #print( prow[0] )
                    digdata = {"id":prow[0],"FullName":prow[1] }
                    self.digitalpointrefs.append( digdata )
            except pyodbc.Error:
                print("No suitable columns in table",trow[0])

        print("Analogs", len(self.analogpointrefs))
        print("Digitals", len(self.digitalpointrefs))

    # Override everything - remove from our list the things we can't override
    def OverrideAllPoints(self):
        errorcount = 0
        overridecount = 0
        # analog
        analogpointrefsvalid = []
        for alg in self.analogpointrefs:
            val = random.triangular(alg["OverrideMinimum"],alg["OverrideMaximum"] )
            #print(alg["id"], alg["FullName"], val )
            pointObject = self.connection.GetObject( alg["FullName"])
            if pointObject["InService"]:
                try:
                    pointObject.InvokeMethod("Override", val)
                    analogpointrefsvalid.append( alg)
                    overridecount+=1
                except CSClient.MethodException:
                    errorcount+=1
                    #print("Cannot override", alg["id"])
            if ((errorcount + overridecount) % self.updatesPerBatch == 0):
                print(datetime.datetime.now(), "Overridden:", overridecount, "Unsuccessful:", errorcount)
                # Wait
                time.sleep( self.intervalBetwenBatches )
        # Replace list
        self.analogpointrefs = analogpointrefsvalid
        # digital
        digitalpointrefsvalid = []
        for dig in self.digitalpointrefs:
            val = random.randint(0, 1)
            #print(dig["id"], dig["FullName"], val )
            pointObject = self.connection.GetObject( dig["FullName"])
            if pointObject["InService"]:
                try:
                    pointObject.InvokeMethod("Override", val)
                    digitalpointrefsvalid.append( dig)
                    overridecount+=1
                except CSClient.MethodException:
                    errorcount+=1
                    #print("Cannot override", alg["id"])
            if ((errorcount + overridecount) % self.updatesPerBatch == 0):
                print(datetime.datetime.now(), "Overridden:", overridecount, "Unsuccessful:", errorcount)
                # Wait
                time.sleep( self.intervalBetwenBatches )
        # Replace list
        self.digitalpointrefs = digitalpointrefsvalid
        print("Total Analogs", len(self.analogpointrefs))
        print("Total Digitals", len(self.digitalpointrefs))

    # Override a random point
    def OverrideRandomPoint(self):
        # Digital or analog - random as a proportion of each?
        a = len(self.analogpointrefs)
        t = a + len(self.digitalpointrefs)
        isDigital = random.random() > a/t
        val = 0
        pointObject = 0
        if (isDigital):
            i = random.randrange(0, len(self.digitalpointrefs)-1)
            dig = self.digitalpointrefs[i]
            pointObject = self.connection.GetObject( dig["FullName"])
            val = random.randint(0, 1)
        else:
            i = random.randrange(0, len(self.analogpointrefs)-1)
            alg = self.analogpointrefs[i]
            pointObject = self.connection.GetObject( alg["FullName"])
            val = random.triangular(alg["OverrideMinimum"],alg["OverrideMaximum"] )
        #print(pointObject.FullName, " to ", val)
        try:
            pointObject.InvokeMethod("Override", val)
            #print( "Override OK")
            return True
        except CSClient.MethodException:
            #print( "Override Fail")
            return False

    # Write overridable points list to file
    def WritePointRefsToFile(self):
        with open('analogpointrefs.json', 'w', encoding='utf-8') as f:
            json.dump(self.analogpointrefs, f, ensure_ascii=False, indent=4)
        with open('digitalpointrefs.json', 'w', encoding='utf-8') as f:
            json.dump(self.digitalpointrefs, f, ensure_ascii=False, indent=4)

    # Read points list
    def ReadPointRefsFromFile(self):
        with open('analogpointrefs.json', 'r', encoding='utf-8') as f:
            self.analogpointrefs = json.load(f )
        with open('digitalpointrefs.json', 'r', encoding='utf-8') as f:
            self.digitalpointrefs = json.load(f )
        print("Total Analogs", len(self.analogpointrefs))
        print("Total Digitals", len(self.digitalpointrefs))

    # Check json files exist
    def CheckPointsFiles(self):
        if (os.path.exists( 'analogpointrefs.json') and os.path.exists( 'digitalpointrefs.json') ):
            return True
        return False
    
    # Release all overrides
    def ReleaseAllOverrides(self):
        releasecount = 0
        cursor = self.conn.cursor()
        cursor.execute("select id, FullName FROM CDBPoint " \
                       "WHERE CurrentOverridden = True " \
                       "AND TypeName not in ('CPointDigitalManual','CPointDigitalCalc','CPointAlgManual','CPointAlgCalc') " + \
                       "AND " + self.pointFilter )
        for row in cursor.fetchall():
            #print (row[0], row[1])
            pointObject = self.connection.GetObject( row[1])
            if pointObject["InService"]:
                try:
                    #print("Releasing")
                    pointObject.InvokeMethod("Release" )
                    releasecount+=1
                except CSClient.MethodException:
                    print("Cannot release override", row[1])
            if ((releasecount) % self.updatesPerBatch == 0):
                print(datetime.datetime.now(), "Released:", releasecount)
                # Wait
                time.sleep( self.intervalBetwenBatches )
        print(datetime.datetime.now(), "Released:", releasecount)

    def PrepareSim(self):
        # Do files exist?
        if not self.CheckPointsFiles():
            # Must start by reading table definitions
            self.ReadTableDef()
            # Reconfig
            self.ModifyAllOverrideConfig()
            # Find
            self.FindOverridablePoints()
            # Override
            self.OverrideAllPoints()
            # Write overridable points list to file
            self.WritePointRefsToFile()
        else:
            # Read points list
            self.ReadPointRefsFromFile()

    def ExecuteSim(self):
        for i in range(0, self.countOfBatches):
            print(datetime.datetime.now(), "Batch Start"  )
            c = 0
            for j in range(0, self.updatesPerBatch):
                if ( self.OverrideRandomPoint() ):
                    c+=1
            print(datetime.datetime.now(), "Batch End", c )
            # Wait
            time.sleep( self.intervalBetwenBatches )

if __name__ == "__main__":
    # New sim object
    OS = OverrideSim(pointFilter, updatesPerBatch, intervalBetwenBatches, countOfBatches)

    print(datetime.datetime.now(), "Simulation Prepare Start" )

    # Modify database (first time) and try ALL overrides, or load the json if done already
    OS.PrepareSim()
    # Run the requested loops
    OS.ExecuteSim()
    # Release - optional step
    print(datetime.datetime.now(), "Release all overrides" )
    OS.ReleaseAllOverrides()

    print(datetime.datetime.now(), "Finished" )

