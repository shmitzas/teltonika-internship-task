using System;


namespace teltonika_internship_task
{
    public class SpeedData
    {
        public int[] bins { get; set; }
        public int binCount { get; set; }
        public int totalCount { get; set; }
        public double minSpeed { get; set; }
        public double binSize { get; set; }
    }
}
