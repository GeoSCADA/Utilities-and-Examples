# Import .Net runtime support - needs "pip install pythonnet", supported by Python 3.8, 3.12
import clr
# Get Geo SCADA Library
CS = clr.AddReference( "c:\Program Files\Schneider Electric\ClearSCADA\ClearSCADA.Client.dll" )
import ClearScada.Client as CSClient 

# Create node and connect, then log in. (Could read net parameters from SYSTEMS.XML)
node = CSClient.ServerNode( CSClient.ConnectionType.Standard, "127.0.0.1", 5481 )
connection = CSClient.Simple.Connection( "Utility" )
connection.Connect( node )
# Add your credentials to Geo SCADA here:
connection.LogOn( "", "" ) # ENTER YOUR USERNAME AND PASSWORD HERE

# Web request library - needs "pip install requests"
import requests

# Get the key by registering at openweathermap.org
API_key = "enter your openweathermap key here"
base_url = "http://api.openweathermap.org/data/2.5/weather?"

# Here we are using Lat and Long to get the data, you can use city/town names
# (You could read a list of Geo SCADA points with their locations and use those too)
# This is the location of SE offices in Bogota, Colombia
latitude = 4.70960
longitude = -74.06192

# Make the web request
Final_url = base_url + "appid=" + API_key + "&lat=" + str(latitude) + "&lon=" + str(longitude)
weather_data = requests.get(Final_url).json()

# Print out
print(weather_data)

# Get Wind speed (default in meters/sec) and Direction (degrees)
wind_speed = weather_data["wind"]["speed"]
wind_dir = weather_data["wind"]["deg"]

# Find an internal named according to the signal - create this Internal point first with ViewX
WindSpeedObj = connection.GetObject("Wind Speed in Bogota" )
WindDirObj = connection.GetObject("Wind Dir in Bogota" )

WindSpeedObj.InvokeMethod("CurrentValue", wind_speed )
WindDirObj.InvokeMethod("CurrentValue", wind_dir )

# Echo value back as a test
print( "Speed set to: " + str(WindSpeedObj.GetProperty("CurrentValue" ) ) )
print( "Direc set to: " + str(WindDirObj.GetProperty("CurrentValue" ) ) )

#Sample output from OpenWeatherMap API
#{
# "coord": {
#   "lon": -74.0619,
#   "lat": 4.7096
# },
# "weather": [
#   {
#     "id": 803,
#     "main": "Clouds",
#     "description": "broken clouds",
#     "icon": "04n"
#   }
# ],
# "base": "stations",
# "main": {
#   "temp": 283.15,
#   "feels_like": 282.19,
#   "temp_min": 283.15,
#   "temp_max": 283.15,
#   "pressure": 1025,
#   "humidity": 93
# },
# "visibility": 10000,
# "wind": {
#   "speed": 1.03,
#   "deg": 0
# },
# "clouds": {
#   "all": 75
# },
# "dt": 1615974896,
# "sys": {
#   "type": 1,
#   "id": 8582,
#   "country": "CO",
#   "sunrise": 1615978896,
#   "sunset": 1616022449
# },
# "timezone": -18000,
# "id": 7033249,
# "name": "Santa Barbara Central",
# "cod": 200
#}

