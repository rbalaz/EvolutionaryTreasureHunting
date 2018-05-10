using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Representation;

namespace TravellingSalesPerson.Evolution_algorithm_blocks
{
    class Replacement
    {
        private List<Individual> oldGeneration;
        private List<Individual> descendants;
        private int newGenerationCount;

        public Replacement(List<Individual> oldGeneration, List<Individual> descendants, int newGenerationCount)
        {
            this.oldGeneration = oldGeneration;
            this.descendants = descendants;
            this.newGenerationCount = newGenerationCount;
        }

        public List<Individual> NextGeneration()
        {
            List<Individual> nextGeneration = new List<Individual>();
            List<Individual> orderedOldGeneration = oldGeneration.OrderBy(item => item.Fitness).ToList();
            nextGeneration.AddRange(orderedOldGeneration.Take(newGenerationCount / 10));

            List<Individual> orderedDescendants = descendants.OrderBy(item => item.Fitness).ToList();
            nextGeneration.AddRange(orderedDescendants.Take(newGenerationCount - nextGeneration.Count));

            return nextGeneration;
        }
    }
}
