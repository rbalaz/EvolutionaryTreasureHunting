using System;
using System.Collections.Generic;
using GoldMiners.Representation;
using GoldMiners.Utils;

namespace GoldMiners.Evolution_algorithm_blocks
{
    class Initialisation
    {
        private int initialPopulationCount;
        private int goldFieldCount;

        public Initialisation(int initialPopulationCount, int goldFieldCount)
        {
            this.initialPopulationCount = initialPopulationCount;
            this.goldFieldCount = goldFieldCount;
        }

        public List<Individual> InitialisePopulation()
        {
            List<Individual> initialPopulation = new List<Individual>();
            // Boundaries
            // X must be at least 1 and has to be at most 32
            // Y must be at least 6 and has to be at most 27
            Random random = new Random();
            for (int i = 0; i < initialPopulationCount; i++)
            {
                List<Point> referencePoints = new List<Point>();
                for (int j = 0; j < goldFieldCount; j++)
                {
                    int randX = random.Next(1, 33);
                    int randY = random.Next(6, 28);
                    Point reference = new Point(randX, randY);
                    referencePoints.Add(reference);
                }


                Individual individual = new Individual(referencePoints);
                initialPopulation.Add(individual);
            }

            return initialPopulation;
        }
    }
}
