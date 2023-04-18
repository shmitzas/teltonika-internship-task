using System;
using System.Collections.Generic;
using System.Linq;

namespace teltonika_internship_task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter full file path: ");
            var filePath = Console.ReadLine();

            List<GpsData> gpsData = new List<GpsData>();
            var ioHandler = new IOHandler();
            gpsData = ioHandler.ReadGpsDataFromFile(filePath);

        }
    }
}
