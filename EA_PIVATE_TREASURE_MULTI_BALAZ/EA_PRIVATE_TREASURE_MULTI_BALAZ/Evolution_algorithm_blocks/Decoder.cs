using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class Decoder
    {
        public Representation EncodePath(Path path)
        {
            // Representing absolute segment length
            // Positive number indicates move up segments
            // Negative number indicates move right segments
            List<int> values = new List<int>();

            int currentSegmentLength = 0;
            string currentSegmentType = "";
            for (int i = 1; i < path.Points.Count; i++)
            {
                if (i == 1)
                {
                    currentSegmentLength++;
                    currentSegmentType = IdentifySegmentDirection(path.Points[i - 1], path.Points[i]);
                }
                else
                {
                    string newSegmentType = IdentifySegmentDirection(path.Points[i - 1], path.Points[i]);
                    if (currentSegmentType == newSegmentType)
                        currentSegmentLength++;
                    else
                    {
                        if (currentSegmentType == "up")
                            values.Add(currentSegmentLength);
                        else
                            values.Add(-currentSegmentLength);

                        currentSegmentLength = 1;
                        currentSegmentType = newSegmentType;
                    }
                }
            }
            if (currentSegmentType == "up")
                values.Add(currentSegmentLength);
            else
                values.Add(-currentSegmentLength);

            return new Representation(path.Fitness, values);
        }

        private string IdentifySegmentDirection(Point first, Point second)
        {
            if (first.X == second.X)
                return "up";
            if (first.Y == second.Y)
                return "right";
            throw new NotSupportedException();
        }

        public Path DecodeRepresentation(Representation representation)
        {
            List<Point> points = new List<Point>(70)
            {
                new Point(1, 1)
            };

            for (int i = 0; i < representation.Values.Count; i++)
            {
                int absoluteValue = Math.Abs(representation.Values[i]);

                for (int j = 0; j < absoluteValue; j++)
                {
                    if (representation.Values[i] > 0)
                        points.Add(MoveUp(points[points.Count - 1]));
                    else
                        points.Add(MoveRight(points[points.Count - 1]));
                }
            }

            return new Path(points);
        }

        private Point MoveUp(Point current)
        {
            return new Point(current.X, current.Y + 1);
        }

        private Point MoveRight(Point currrent)
        {
            return new Point(currrent.X + 1, currrent.Y);
        }
    }
}
