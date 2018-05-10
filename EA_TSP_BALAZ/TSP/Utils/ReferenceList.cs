using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Representation;

namespace TravellingSalesPerson.Utils
{
    class ReferenceList
    {
        public List<Point> PointOrder { get; private set; }

        public ReferenceList(string fileName)
        {
            PointOrder = new List<Point>();

            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                string[] coordinates = line.Split(' ');
                for (int i = 0; i < coordinates.Length; i = i + 2)
                {
                    int x = int.Parse(coordinates[i]);
                    int y = int.Parse(coordinates[i + 1]);
                    Point point = new Point(x, y);

                    PointOrder.Add(point);
                }
            }

            reader.Close();
            stream.Close();
        }

        public int GetPointIndex(Point point)
        {
            foreach (Point p in PointOrder)
            {
                if (p.X == point.X && p.Y == point.Y)
                {
                    return PointOrder.IndexOf(p) + 1;
                }
            }

            return -1;
        }
    }
}
