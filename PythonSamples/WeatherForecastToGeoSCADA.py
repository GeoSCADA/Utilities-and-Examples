# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8
import clr
# Get Geo SCADA Library 
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
# Add your credentials to Geo SCADA here:
connection.LogOn( "", "" ) # ENTER YOUR USERNAME AND PASSWORD HERE

# Web request library - needs "pip install requests"
import requests
# Support .Net DateTime
from System import DateTime
from System import DateTimeKind

# Get the key by registering at openweathermap.org, these are Steve Beadle's keys
API_key = "your openweathermap key"
base_url = "http://api.openweathermap.org/data/2.5/onecall?"

# Here we are using Lat and Long to get the data, you can use city/town names
# (You could read a list of Geo SCADA points with their locations and use those too)
# This is the location of SE offices in Bogota, Colombia
latitude = 4.70960
longitude = -74.06192

# Make the web request
Final_url = base_url + "appid=" + API_key + "&lat=" + str(latitude) + "&lon=" + str(longitude)
weather_data = requests.get(Final_url).json()

# Print out
#print(weather_data)

wind_speed = weather_data["current"]["wind_speed"]
wind_dir = weather_data["current"]["wind_deg"]
temperature = weather_data["current"]["temp"]

# First set a point with current weather data

# Find an internal point named according to the signal - create this Internal point first with ViewX
WindSpeedObj = connection.GetObject("Meteo.Bogota Wind Speed" )
WindDirObj = connection.GetObject("Meteo.Bogota Wind Direction" )
TemperatureObj = connection.GetObject("Meteo.Bogota Temperature" )

WindSpeedObj.InvokeMethod("CurrentValue", wind_speed )
WindDirObj.InvokeMethod("CurrentValue", wind_dir )
TemperatureObj.InvokeMethod("CurrentValue", temperature - 273)

# Second get a Forecast object reference and add wind speed forecasts

# Get Forecast Object
ForecastObj = connection.GetObject("Meteo.Bogota Wind Speed Forecast Hourly")

# Hourly data
hourlydata = weather_data["hourly"]
fcfirsttime = hourlydata[0]["dt"]
fclasttime = fcfirsttime
print( fcfirsttime )
fcvalues = []
fctimes = []
fcqualities = []
for i in hourlydata:
    print ("Hourly: ", i["dt"], i["wind_speed"]  )
    fclasttime = i["dt"]
    fcvalues.append( i["wind_speed"] )
    fctimes.append( DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(fclasttime) )
    fcqualities.append( 192)

## Daily data
dailydata = weather_data["daily"]
for i in dailydata:
    if ( i["dt"] > fclasttime):
        print ("Daily: ", i["dt"], i["wind_speed"]  )
        fclasttime = i["dt"]
        fcvalues.append( i["wind_speed"] )
        fctimes.append( DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(fclasttime) )
        fcqualities.append( 192)

#Set values
fctime =  DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(fcfirsttime) # time of first sample
fcname = "Forecasted " + fctime.ToString("s").replace("T"," ") + " UTC" # Forecast time name is in UTC
# name, times, values, qualities
ForecastObj.InvokeMethod("SetForecastEx", fcname, fctimes, fcvalues, fcqualities)
print ("Forecast Set")


