using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Utils
{
    class Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static int ManhattanDistanceBetweenTwoPoints(Point first, Point second)
        {
            return (int)(Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y));
        }
    }
}
