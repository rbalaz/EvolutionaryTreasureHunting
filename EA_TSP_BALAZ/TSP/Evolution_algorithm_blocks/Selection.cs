using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Representation;

namespace TravellingSalesPerson.Evolution_algorithm_blocks
{
    class Selection
    {
        private int parentsCount;
        private List<Individual> population;

        public Selection(List<Individual> population, int parentsCount)
        {
            this.parentsCount = parentsCount;
            this.population = population;
        }

        public List<Individual> SelectParents(int q)
        {
            Random random = new Random();
            List<Individual> parents = new List<Individual>();
            for (int i = 0; i < parentsCount; i++)
            {
                List<Individual> competitors = new List<Individual>();
                for (int j = 0; j < q; j++)
                {
                    int index = random.Next(0, population.Count);
                    competitors.Add(population[index]);
                }

                double minFitness = competitors.Min(item => item.Fitness);
                parents.Add(competitors.Where(item => item.Fitness == minFitness).ToArray()[0]);
            }

            return parents;
        }
    }
}
