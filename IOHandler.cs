using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace teltonika_internship_task
{
    class IOHandler
    {
        public List<GpsData> ReadGpsDataFromFile(string filePath)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    string fileExtension = Path.GetExtension(filePath);

                    if (fileExtension == ".json")
                    {
                        return ReadFromJson(streamReader);
                    }
                    else if (fileExtension == ".csv")
                    {
                        return ReadFromCSV(streamReader);

                    }
                    else if (fileExtension == ".bin")
                    {
                        return ReadFromBinary(streamReader);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid file extension. The file must be in JSON, CSV or Binary format.");
                    }
                }
            }
            catch (Exception ex) 
            {
                throw new Exception($"File '{filePath}' does not exist.");
            }
        }

        private List<GpsData> ReadFromJson(StreamReader streamReader)
        {
            string jsonData = streamReader.ReadToEnd();
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            return JsonSerializer.Deserialize<List<GpsData>>(jsonData, options);
        }

        private List<GpsData> ReadFromCSV(StreamReader streamReader)
        {
            var gpsDataList = new List<GpsData>();
            bool headerRow = true;
            while (streamReader.Peek() >= 0)
            {
                string line = streamReader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    string[] fields = line.Split(',');
                    if (headerRow)
                    {
                        headerRow = false;
                    }
                    else
                    {
                        GpsData gpsData = new GpsData();
                        try
                        {
                            gpsData.Latitude = double.Parse(fields[0], CultureInfo.InvariantCulture);
                            gpsData.Longitude = double.Parse(fields[1], CultureInfo.InvariantCulture);
                            gpsData.GpsTime = DateTime.ParseExact(fields[2], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            gpsData.Speed = double.Parse(fields[3], CultureInfo.InvariantCulture);
                            gpsData.Angle = double.Parse(fields[4], CultureInfo.InvariantCulture);
                            gpsData.Altitude = double.Parse(fields[5], CultureInfo.InvariantCulture);
                            gpsData.Satellites = int.Parse(fields[6], CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }
                        gpsDataList.Add(gpsData);
                    }
                }
            }
            return gpsDataList;
        }

        private List<GpsData> ReadFromBinary(StreamReader streamReader)
        {
            var gpsDataList = new List<GpsData>();
            using (BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream))
            {
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    GpsData gpsData = new GpsData();
                    try
                    {
                        gpsData.Latitude = binaryReader.ReadInt32();
                        gpsData.Longitude = binaryReader.ReadInt32();
                        long gpsTimeTicks = binaryReader.ReadInt64();
                        gpsData.GpsTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(1577840400000);
                        gpsData.Speed = binaryReader.ReadUInt16() / 100.0;
                        gpsData.Angle = binaryReader.ReadUInt16() / 100.0;
                        gpsData.Altitude = binaryReader.ReadUInt16();
                        gpsData.Satellites = binaryReader.ReadByte();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        break;
                    }
                    gpsDataList.Add(gpsData);
                }
            }
            return gpsDataList;
        }
    }
}