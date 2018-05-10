using GoldMiners.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldMiners.Evolution_algorithm_blocks
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
            List<Individual> orderedOldGeneration = oldGeneration.OrderBy(item => item.Fitness).ToList();
            List<Individual> orderedDescendants = descendants.OrderBy(item => item.Fitness).ToList();

            List<Individual> newGeneration = new List<Individual>();
            newGeneration.AddRange(orderedOldGeneration.Take(orderedOldGeneration.Count / 10));

            newGeneration.AddRange(orderedDescendants.Take(newGenerationCount - newGeneration.Count));

            return newGeneration;
        }

        public List<Individual> KillBestIndividuals()
        {
            List<Individual> allIndividuals = new List<Individual>();
            allIndividuals.AddRange(oldGeneration);
            allIndividuals.AddRange(descendants);
            allIndividuals = allIndividuals.OrderByDescending(item => item.Fitness).ToList();

            return allIndividuals.Take(newGenerationCount).ToList();
        }
    }
}
