using Pacman.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Evolution_algorithm_blocks
{
    public class Initialisation
    {
        private int initialPopulationCount;

        public Initialisation(int initialPopulationCount)
        {
            this.initialPopulationCount = initialPopulationCount;
        }

        public List<Individual> GenerateInitialPopulation()
        {
            Random random = new Random();
            // Every individual has starting position at X = 1 and Y = 15
            // Goal is any position with X = 40 and Y from <1,30>

            List<Individual> initialPopulation = new List<Individual>(initialPopulationCount);
            for (int i = 0; i < initialPopulationCount; i++)
            {
                // Every individual will have its movements vector with 39 values
                int Y = 15;
                List<int> movements = new List<int>(39);
                for (int j = 0; j < 39; j++)
                {
                    // 0 - X increments, Y decrements
                    // 1 - X increments, Y increments
                    if (Y == 30)
                    {
                        movements.Add(0);
                        Y--;
                    }
                    else if (Y == 1)
                    {
                        movements.Add(1);
                        Y++;
                    }
                    else
                    {
                        int randomMove = random.Next(0, 2);
                        movements.Add(randomMove);
                        if (randomMove == 0)
                        {
                            Y--;
                        }
                        else
                        {
                            Y++;
                        }
                    }
                }
                Individual individual = new Individual(movements);
                initialPopulation.Add(individual);
            }

            return initialPopulation;
        }
    }
}
