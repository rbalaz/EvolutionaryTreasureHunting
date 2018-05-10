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
                if (choice == 1 )
                    return new Point(currentEndPoint.X, currentEndPoint.Y + 1);
                // Choice 2 - apply Move Right operator
                else
                    return new Point(currentEndPoint.X + 1, currentEndPoint.Y);
            }
        }

        public List<Path> GenerateInitialPopulation2()
        {
            List<Path> initialPopulation = new List<Path>(initialPopulationCount);
            // Random point-wise generation with equal moves
            initialPopulation.AddRange(EqualPopulationGeneration(initialPopulationCount / 3));
            // Random point-wise generation with random generated probability moves
            initialPopulation.AddRange(NonEqualPopulationGeneration(initialPopulationCount / 3));
            // Random segment-wise generation
            initialPopulation.AddRange(RandomSegmentPopulationGeneration(initialPopulationCount - initialPopulation.Count));

            return initialPopulation;
        }

        private List<Path> EqualPopulationGeneration(int populationCount)
        {
            List<Path> paths = new List<Path>(populationCount); 
            
            // Operators are being chosen with the same probability
            for (int i = 0; i < populationCount; i++)
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

                paths.Add(new Path(points));
            }

            return paths;
        }

        private List<Path> NonEqualPopulationGeneration(int populationCount)
        {
            List<Path> paths = new List<Path>(populationCount);

            // Operators are being chosen with the same probability
            for (int i = 0; i < populationCount; i++)
            {
                // Every path has exactly 70 points
                // First and last point are fixed, only the points in between 
                // are affected by evolution
                List<Point> points = new List<Point>(70)
                {
                    new Point(1, 1)
                };

                double upChance = random.NextDouble();
                double rightChance = 1 - upChance;
                for (int j = 0; j < 68; j++)
                {
                    points.Add(RandomMove(points[points.Count - 1], upChance, rightChance));
                }

                paths.Add(new Path(points));
            }

            return paths;
        }

        private Point RandomMove(Point currentEndPoint, double upChance, double rightChance)
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
                double choice = random.NextDouble();
                // Choice 1 - apply Move Up operator
                if (choice <= upChance)
                    return new Point(currentEndPoint.X, currentEndPoint.Y + 1);
                // Choice 2 - apply Move Right operator
                else
                    return new Point(currentEndPoint.X + 1, currentEndPoint.Y);
            }
        }

        private List<Path> RandomSegmentPopulationGeneration(int populationCount)
        {
            List<Path> paths = new List<Path>();
            

            for (int i = 0; i < populationCount; i++)
            {
                List<Point> points = new List<Point>
                {
                    new Point(1, 1)
                };

                int upRemaining = 29;
                int rightRemaining = 39;

                int sign, segmentLength;
                while (upRemaining > 0 || rightRemaining > 0)
                {
                    sign = random.Next(0, 2);
                    
                    // Up segment
                    if (sign == 0 && upRemaining > 0)
                    {
                        segmentLength = random.Next(0, upRemaining) + 1;
                        for (int j = 0; j < segmentLength; j++)
                        {
                            points.Add(MoveUp(points[points.Count - 1]));
                        }
                        upRemaining -= segmentLength;
                    }
                    // Right segment
                    if(sign == 1 && rightRemaining > 0)
                    {
                        segmentLength = random.Next(0, rightRemaining) + 1;
                        for (int j = 0; j < segmentLength; j++)
                        {
                            points.Add(MoveRight(points[points.Count - 1]));
                        }
                        rightRemaining -= segmentLength;
                    }
                }
                paths.Add(new Path(points));
            }

            return paths;
        }

        private Point MoveRight(Point reference)
        {
            return new Point(reference.X + 1, reference.Y);
        }

        private Point MoveUp(Point reference)
        {
            return new Point(reference.X, reference.Y + 1);
        }
    }
}
