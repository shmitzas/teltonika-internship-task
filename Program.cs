using System;
using System.Collections.Generic;
using System.Linq;
using Geolocation;

namespace teltonika_internship_task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter full file path: ");
            var filePath = Console.ReadLine();

            var ioHandler = new IOHandler();
            var gpsData = ioHandler.ReadGpsDataFromFile(filePath);

            DrawSatellitesHistogram(gpsData);

            Console.WriteLine();
            DrawSpeedHistogram(gpsData);

            Console.WriteLine();
            CalculateRoads(gpsData);
        }
        private static void DrawSatellitesHistogram(List<GpsData> gpsDataList)
        {
            // Find the min and max number of satellites
            int minSatellites = gpsDataList.Min(data => data.Satellites);
            int maxSatellites = gpsDataList.Max(data => data.Satellites);

            // Create a dictionary to count the number of GPS data objects for each number of satellites
            Dictionary<int, int> satellitesCounts = new Dictionary<int, int>();
            for (int i = minSatellites; i <= maxSatellites; i++)
            {
                satellitesCounts.Add(i, 0);
            }
            foreach (GpsData gpsData in gpsDataList)
            {
                satellitesCounts[gpsData.Satellites]++;
            }

            // Find the maximum count of GPS data objects for a single number of satellites
            int maxCount = satellitesCounts.Values.Max();
            int minCount = satellitesCounts.Values.Min();
            // Scale the histogram to a maximum of 20 # characters per column
            int scaleFactor = (int)Math.Ceiling(maxCount / 20.0);

            // Define the column width and spacing
            const int colWidth = 2;
            const int colSpacing = 1;

            // Draw the histogram
            Console.WriteLine("Satellites Histogram:");
            Console.WriteLine("hits");

            for (int i = maxCount / scaleFactor; i > 0; i--)
            {
                string rowLabel = $"{' ',4} |";
                if (i == maxCount / scaleFactor)
                {
                    rowLabel = $"{maxCount,4} |";
                }
                if (i == 1)
                {
                    rowLabel = $"{0,4} |";
                }
                string row = "";
                for (int j = minSatellites; j <= maxSatellites; j++)
                {
                    int count = satellitesCounts[j] / scaleFactor;
                    string colLabel = $"{j,4}";
                    if (count >= i)
                    {
                        row += new string('#', colWidth);
                    }
                    else
                    {
                        if (i == 1 && satellitesCounts[j] > 0)
                        {
                            row += new string('#', colWidth);
                        }
                        else
                        {
                            row += new string(' ', colWidth);
                        }
                    }

                    row += new string(' ', colSpacing);
                }
                Console.WriteLine(rowLabel + row);
            }

            // Print the x-axis labels
            string x_axis = new string(' ', 6);
            for (int i = minSatellites; i <= maxSatellites; i++)
            {
                if (i < 10)
                {
                    x_axis += $"0{i,1}";
                }
                else
                {
                    x_axis += $"{i,1}";
                }
                x_axis += new string(' ', colSpacing);
            }
            Console.WriteLine(x_axis);
        }

        private static void DrawSpeedHistogram(List<GpsData> gpsDataList)
        {

            // Determine the range of speeds in the GPS data
            double minSpeed = gpsDataList.Min(g => g.Speed);
            double maxSpeed = gpsDataList.Max(g => g.Speed);
            double speedRange = maxSpeed - minSpeed;
            int binCount = (int)Math.Round(maxSpeed / 10);
            // Calculate the bin size based on the range and bin count
            double binSize = Math.Ceiling(speedRange / binCount);

            // Initialize the bins
            int[] bins = new int[binCount];

            // Count the number of GPS data points in each bin
            int totalCount = 0;
            foreach (GpsData gpsData in gpsDataList)
            {
                int binIndex = (int)((gpsData.Speed - minSpeed) / binSize);
                if (binIndex < 0 || binIndex >= binCount)
                {
                    // Skip data points outside the range of the histogram
                    continue;
                }
                bins[binIndex]++;
                totalCount++;
            }

            // Draw the histogram
            Console.WriteLine($"{"Speed Histogram:",-44} | {"hits",8}");
            for (int i = 0; i < binCount; i++)
            {
                double binStart = minSpeed + i * binSize;
                double binEnd = binStart + binSize - 1;
                string binLabel = $"{(int)binStart} - {(int)binEnd}";
                double binPercentage = (double)bins[i] / totalCount * 100;
                string binProgress = new string('#', (int)Math.Round(binPercentage));
                Console.WriteLine($"[{binLabel,9}] | {binProgress,-30} | {bins[i],8:F0}");
            }
        }

        private static void CalculateRoads(List<GpsData> gpsDataList)
        {
            var roadSections = new List<RoadSection>();
            var startPoint = new GpsData();
            double distance = 0;
            for (int i = 0; i < gpsDataList.Count()-1; i++)
            {
                if(distance == 0)
                { 
                    startPoint = gpsDataList[i];
                }
                var point1 = new Coordinate
                {
                    Latitude = gpsDataList[i].Latitude,
                    Longitude = gpsDataList[i].Longitude
                };
                var point2 = new Coordinate
                {
                    Latitude = gpsDataList[i+1].Latitude,
                    Longitude = gpsDataList[i+1].Longitude
                };
                distance += GeoCalculator.GetDistance(point1, point2, distanceUnit: DistanceUnit.Kilometers, decimalPlaces: 3);
                                
                if (distance >= 100.000)
                {
                    TimeSpan totalTime = gpsDataList[i + 1].GpsTime - startPoint.GpsTime;
                    var roadSection = new RoadSection
                    {
                        Distance = Math.Round(distance, 3),
                        Time = totalTime.TotalSeconds,
                        StartLatitude = startPoint.Latitude,
                        StartLongitude = startPoint.Longitude,
                        EndLatitude = gpsDataList[i+1].Latitude,
                        EndLongitude = gpsDataList[i+1].Longitude,
                        StartTime = startPoint.GpsTime,
                        EndTime = gpsDataList[i+1].GpsTime,
                        AverageSpeed = distance / totalTime.TotalHours
                    };
                    roadSections.Add(roadSection);
                    distance = 0;
                    totalTime = TimeSpan.Zero;
                }
            }
            var fastestRoadSection = roadSections.OrderBy(r => r.Time).First();
            Console.WriteLine($"Fastest road section of at least 100km was driven over {fastestRoadSection.Time}s and was {fastestRoadSection.Distance}km long.");
            Console.WriteLine($"{fastestRoadSection.Format()}");
        }
    }

}
