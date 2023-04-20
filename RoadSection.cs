using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teltonika_internship_task
{
    public class RoadSection
    {
        public double Distance { get; set; }
        public double Time { get; set; }
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double AverageSpeed { get; set; }
        public override string ToString()
        {
            return string.Format("Distance: {0:0.00} km,\nTime: {1} seconds,\nStart Coordinates: {2:0.0000000}, {3:0.0000000},\nEnd Coordinates: {4:0.0000000}, {5:0.0000000},\nStart Time: {6}, End Time: {7},\nAverage Speed: {8:0.00} km/h",
                               Distance, Time, StartLatitude, StartLongitude, EndLatitude, EndLongitude, StartTime.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.ToString("yyyy-MM-dd HH:mm:ss"), AverageSpeed);
        }
        public string Format()
        {
            return string.Format("Start Coordinates: {0:0.0000000}, {1:0.0000000},\nEnd Coordinates: {2:0.0000000}, {3:0.0000000},\nStart Time: {4}, End Time: {5},\nAverage Speed: {6:0.00} km/h",
                               StartLatitude, StartLongitude, EndLatitude, EndLongitude, StartTime.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.ToString("yyyy-MM-dd HH:mm:ss"), AverageSpeed);
        }
    }
}
