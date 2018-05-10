using GoldMiners.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldMiners.Evolution_algorithm_blocks
{
    class Decoder
    {
        public List<Point> DecodeReference(Point reference)
        {
            List<Point> outerPoints = new List<Point>();

            for (int i = 0; i < 9; i++)
            {
                if (i == 4)
                    continue;
                Point current = new Point(reference.X + i, reference.Y);
                outerPoints.Add(current);
            }

            outerPoints.Add(new Point(reference.X, reference.Y - 1));
            outerPoints.Add(new Point(reference.X + 8, reference.Y - 1));

            for (int i = 0; i < 9; i++)
            {
                if (i == 4)
                    continue;
                Point current = new Point(reference.X + i, reference.Y - 2);
                outerPoints.Add(current);
            }

            for (int i = 3; i > -6; i--)
            {
                if (i < 1 && i > -3)
                    continue;
                Point current = new Point(reference.X + 3, reference.Y + i);
                outerPoints.Add(current);
            }

            outerPoints.Add(new Point(reference.X + 4, reference.Y + 3));
            outerPoints.Add(new Point(reference.X + 4, reference.Y - 5));

            for (int i = 3; i > -6; i--)
            {
                if (i < 1 && i > -3)
                    continue;
                Point current = new Point(reference.X + 5, reference.Y + i);
                outerPoints.Add(current);
            }

            return outerPoints;
        }

        public List<Point> ReconstructField(Point reference)
        {
            List<Point> outerPoints = new List<Point>();

            for (int i = 0; i < 4; i++)
            {
                Point current = new Point(reference.X + i, reference.Y);
                outerPoints.Add(current);
            }

            for (int i = 1; i < 4; i++)
            {
                Point current = new Point(reference.X + 3, reference.Y + i);
                outerPoints.Add(current);
            }

            for (int i = 4; i < 6; i++)
            {
                Point current = new Point(reference.X + i, reference.Y + 3);
                outerPoints.Add(current);
            }

            for (int i = 3; i > -1; i--)
            {
                Point current = new Point(reference.X + 5, reference.Y + i);
                outerPoints.Add(current);
            }

            for (int i = 6; i < 9; i++)
            {
                Point current = new Point(reference.X + i, reference.Y);
                outerPoints.Add(current);
            }

            for (int i = -1; i > -3; i--)
            {
                Point current = new Point(reference.X + 8, reference.Y + i);
                outerPoints.Add(current);
            }

            for (int i = 7; i > 4; i--)
            {
                Point current = new Point(reference.X + i, reference.Y - 2);
                outerPoints.Add(current);
            }

            for (int i = -3; i > -6; i--)
            {
                Point current = new Point(reference.X + 5, reference.Y + i);
                outerPoints.Add(current);
            }

            for (int i = 5; i > 3; i--)
            {
                Point current = new Point(reference.X + i, reference.Y - 5);
                outerPoints.Add(current);
            }

            for (int i = -5; i < -1; i++)
            {
                Point current = new Point(reference.X + 3, reference.Y + i);
                outerPoints.Add(current);
            }

            for (int i = 2; i > -1; i--)
            {
                Point current = new Point(reference.X + i, reference.Y - 2);
                outerPoints.Add(current);
            }

            Point last = new Point(reference.X, reference.Y - 1);
            outerPoints.Add(last);
            outerPoints.Add(reference);

            return outerPoints;
        }
    }
}
