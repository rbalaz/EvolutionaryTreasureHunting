using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class Initialisation
    {
        private int initialPopulationCount;
        private Random random;

        public Initialisation(int initialPopulationCount)
        {
            this.initialPopulationCount = initialPopulationCount;
            random = new Random();
        }

        public List<Path> GenerateInitialPopulation()
        {
            List<Path> initialPopulation = new List<Path>(initialPopulationCount);

            // Allowed operators: Move Up, Move Right
            // Initial population is generated using random move generator
            // From every position, any of the two operators can be chosen if applicable
            // Operators are being chosen with the same probability
            for (int i = 0; i < initialPopulationCount; i++)
            {
                // Every path has exactly 70 points
                // First and last point are fixed, only the points in between 
                // are affected by evolution
                List<Point> points = new List<Point>(70)
                {
                    new Point(1, 1)
                };

                for (int j = 0; j < 68; j++)
                {
                    points.Add(RandomMove(points[points.Count - 1]));
                }

                initialPopulation.Add(new Path(points));
            }

            return initialPopulation;
        }

        private Point RandomMove(Point currentEndPoint)
        {
            // Move Up operator applicable?
            if (currentEndPoint.Y == 30)
            {
                return new Point(currentEndPoint.X + 1, currentEndPoint.Y);
            }
            // Move Right operator applicable?
            else if (currentEndPoint.X == 40)
            {
                return new Point(currentEndPoint.X, currentEndPoint.Y + 1);
            }
            else
            {
                int choice = random.Next(1,3);
                // Choice 1 - apply Move Up operator
                if (choice == 1)
                    return new Point(currentEndPoint.X, currentEndPoint.Y + 1);
                // Choice 2 - apply Move Right operator
                else
                    return new Point(currentEndPoint.X + 1, currentEndPoint.Y);
            }
        }
    }
}
