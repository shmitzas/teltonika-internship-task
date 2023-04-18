using System;

namespace teltonika_internship_task
{
    public class GpsData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime GpsTime { get; set; }
        public double Speed { get; set; }
        public double Angle { get; set; }
        public double Altitude { get; set; }
        public int Satellites { get; set; }
        public override string ToString()
        {
            return string.Format("Latitude: {0:0.000000}, Longitude: {1:0.000000}, GPS Time: {2}, Speed: {3:0.00} km/h, Angle: {4:0.00} degrees, Altitude: {5} m, Satellites: {6}",
                Latitude, Longitude, GpsTime.ToString("yyyy-MM-dd HH:mm:ss"), Speed, Angle, Altitude, Satellites);
        }
    }
}