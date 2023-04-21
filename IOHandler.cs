using System;
using System.Collections.Generic;
using System.IO;
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
            catch
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
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                string[] fields = line.Split(',');
                GpsData gpsData = new GpsData();
                try
                {
                    gpsData.Latitude = double.Parse(fields[0]);
                    gpsData.Longitude = double.Parse(fields[1]);
                    gpsData.GpsTime = DateTime.Parse(fields[2]);
                    gpsData.Speed = double.Parse(fields[3]);
                    gpsData.Angle = double.Parse(fields[4]);
                    gpsData.Altitude = double.Parse(fields[5]);
                    gpsData.Satellites = int.Parse(fields[6]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
                gpsDataList.Add(gpsData);
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
                        gpsData.Latitude = IntToCoords(BytesToInt(binaryReader.ReadBytes(4)));
                        gpsData.Longitude = IntToCoords(BytesToInt(binaryReader.ReadBytes(4)));
                        gpsData.GpsTime = LongToDate(BytesToLong(binaryReader.ReadBytes(8)));
                        gpsData.Speed = binaryReader.ReadUInt16() / 100.0;
                        gpsData.Angle = binaryReader.ReadUInt16() / 100.0;
                        gpsData.Altitude = binaryReader.ReadUInt16();
                        gpsData.Satellites = binaryReader.ReadByte();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    gpsDataList.Add(gpsData);
                }
            }
            return gpsDataList;
        }
        private DateTime LongToDate(long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateTime = epoch.AddMilliseconds(timestamp);
            return dateTime;
        }
        private double IntToCoords(int value)
        {
            return (double)value / 10000000.0;
        }
        public static int BytesToInt(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            int value = BitConverter.ToInt32(bytes, 0);
            return value;
        }
        public static long BytesToLong(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            long value = BitConverter.ToInt64(bytes, 0);
            return value;
        }
    }
}