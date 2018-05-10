using GoldMiners.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldMiners.Evolution_algorithm_blocks
{
    class Selection
    {
        private List<Individual> population;
        private int parentsCount;

        public Selection(List<Individual> population, int parentsCount)
        {
            this.population = population;
            this.parentsCount = parentsCount;
        }

        public List<Individual> SelectParents(int q)
        {
            List<Individual> parents = new List<Individual>();
            Random random = new Random();
            for (int i = 0; i < parentsCount; i++)
            {
                List<Individual> competitors = new List<Individual>();
                for (int j = 0; j < q; j++)
                {
                    competitors.Add(population[random.Next(0, population.Count)]);
                }

                double minFitness = competitors.Min(item => item.Fitness);
                parents.Add(competitors.Where(item => item.Fitness == minFitness).ToList()[0]);
            }

            return parents;
        }
    }
}
