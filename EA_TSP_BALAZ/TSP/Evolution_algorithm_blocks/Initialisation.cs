using Medallion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Representation;
using TravellingSalesPerson.Utils;

namespace TravellingSalesPerson.Evolution_algorithm_blocks
{
    class Initialisation
    {
        private int initialPopulationCount;
        private ReferenceList referenceList;

        public Initialisation(int initialPopulationCount, ReferenceList referenceList)
        {
            this.initialPopulationCount = initialPopulationCount;
            this.referenceList = referenceList;
        }

        public List<Individual> InitialisePopulation()
        {
            List<Individual> initialPopulation = new List<Individual>();

            for (int i = 0; i < initialPopulationCount; i++)
            {
                List<int> sequence = Enumerable.Range(1, referenceList.PointOrder.Count).ToList();
                sequence.Shuffle();

                Individual individual = new Individual(sequence);
                initialPopulation.Add(individual);
            }

            return initialPopulation;
        }
    }
}
