using Pacman.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Evolution_algorithm_blocks
{
    public class Decoder
    {
        public Path DecodeIndividual(Individual individual)
        {
            List<Point> points = new List<Point>();
            Point point = new Point(1, 15);
            points.Add(point);

            for (int i = 0; i < individual.Movements.Count; i++)
            {
                if (individual.Movements[i] == 0)
                {
                    Point newPoint = new Point(points[i].X + 1, points[i].Y - 1);
                    points.Add(newPoint);
                }
                if (individual.Movements[i] == 1)
                {
                    Point newPoint = new Point(points[i].X + 1, points[i].Y + 1);
                    points.Add(newPoint);
                }
            }

            Path path = new Path(points);

            return path;
        }

        public List<Point> GetBeastsLocations(Individual individual)
        {
            Path path = DecodeIndividual(individual);

            Point firstBeast = new Point(13, 15);
            Point secondBeast = new Point(25, 15);
            Point thirdBeast = new Point(37, 15);
            for (int i = 0; i < individual.Movements.Count; i++)
            {
                if (i % 2 == 1)
                {
                    // First beast behaviour
                    if (i < 11)
                    {
                        if (firstBeast.Y < path.Points[i + 1].Y)
                            firstBeast.Y++;
                        if (firstBeast.Y > path.Points[i + 1].Y)
                            firstBeast.Y--;
                    }
                    // Second beast behaviour
                    if (i < 23)
                    {
                        if (secondBeast.Y < path.Points[i + 1].Y)
                            secondBeast.Y++;
                        if (secondBeast.Y > path.Points[i + 1].Y)
                            secondBeast.Y--;
                    }
                    // Third beast behaviour
                    if (i < 35)
                    {
                        if (thirdBeast.Y < path.Points[i + 1].Y)
                            thirdBeast.Y++;
                        if (thirdBeast.Y > path.Points[i + 1].Y)
                            thirdBeast.Y--;
                    }
                }
            }

            return new List<Point> { firstBeast, secondBeast, thirdBeast};
        }
    }
}
