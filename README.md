Teltonika IoT Academy .NET task

# Objective

Create console application which will print report based on input data.

## Functional requirements

1. Read GPS data from .json format file.
2. Read GPS data from .csv format file.
3. Draw histogram of sattelites data, e.g.:
 ![](RackMultipart20230421-1-6px63i_html_752da1ddac344b95.png)
4. Draw histogram of speed data, e.g.:

![](RackMultipart20230421-1-6px63i_html_ff8b07a9eae51d1c.png)

1. Find the road section, along all records loaded from all files, of at least 100 km long which was driven in the shortest time, e.g.:

![](RackMultipart20230421-1-6px63i_html_ec0d959f4e267e06.png)

**Bonus:**

1. Read and process GPS data form binary format file (.bin)

## Non-functional requirements

1. All stages of development must be reflected in a GIT repository.
2. Use .NET5 framework
3. Follow best practices of OOP programming and SOLID principles.
4. Upload solution to github.com and send a link, **do not upload GPS data** to github.com

##

## Data formats

### .JSON

| array | element |
| --- | --- |
| [{element1}, {element2}, … , {elementN}] | {"Latitude": 54.1234567,"Longitude": 25.1234567,"GpsTime": "2019-01-01 12:34:56.789","Speed": 76,"Angle": 125,"Altitude": 263,"Satellites": 15} |

Where:

- Latitude, Longitude – WGS84 coordinates, Decimal Degrees; position of a location where GPS record where taken.
- GpsTime – UTC timestamp of a GPS record.
- Speed – current moving speed.
- Angle – degrees (0-359°) moving direction.
- Altitude – altitude at current position.
- Satellites – count of visible GPS satellites at current position.

### .CSV

Values of elements described in JSON format separated by a comma, e.g.:

Latitude,Longitude,GpsTime,Speed,Angle,Altitude,Satellites

54.6681533000,25.1119433000,2019-07-07 16:11:05.000,19,295,92,17

54.6682050000,25.1118150000,2019-07-07 16:11:07.000,16,310,91,16

### .BIN

Values of elements described in JSON format stored in binary format, byte order – big endian. Binary representation of elements are stored in a continuous row, one by one.

#### Binary data example

0EE6B280202FBF000000016F5E9DD680000F008000FF0A

| Bytes | Size in bytes | Type | Element | Intermediate Value | Transformation | Final value |
| --- | --- | --- | --- | --- | --- | --- |
| 0EE6B280 | 4 | Int32 | Latitude | 250000000 | Divide by 10000000 | 54.0 |
| 202FBF00 | 4 | Int32 | Longitude | 540000000 | Divide by 10000000 | 25.0 |
| 0000016F5E9DD680 | 8 | Int64 | GpsTime | 1577840400000 | Add milliseconds to Unix Epoch | 2020-01-01 01:00:00 UTC |
| 000F | 2 | Int16 | Speed | → | → | 15 |
| 0080 | 2 | Int16 | Angle | → | → | 128 |
| 00FF | 2 | Int16 | Altitude | → | → | 255 |
| 0A | 1 | Byte | Satellites | → | → | 10 |

#### Date math code example:

var gpsTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(1577840400000);

## Hints

- To calculate distance between two points use [Geolocation.NetStandard](https://www.nuget.org/packages/Geolocation.NetStandard/) nuget package.
- Feel free to design your own histogram, do not use third party libraries to draw it.
