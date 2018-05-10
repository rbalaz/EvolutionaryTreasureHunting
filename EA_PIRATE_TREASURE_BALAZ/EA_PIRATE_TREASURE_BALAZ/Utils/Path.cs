using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Utils
{
    class Path
    {
        public List<Point> Points { get; private set; }
        public double Fitness { get; set; }

        public Path(List<Point> points)
        {
            Points = points;
        }

        public void PrintPoints()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Point point in Points)
                builder.Append("[" + point.X + ";" + point.Y + "] ");

            Console.WriteLine(builder.ToString());
        }
    }
}
