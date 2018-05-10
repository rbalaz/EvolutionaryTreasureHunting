using System.Collections.Generic;
using System.Linq;
using TravellingSalesPerson.Representation;

namespace TravellingSalesPerson.Utils
{
    static class StaticOperations
    {
        public static bool ValidateIndividual(Individual individual)
        {
            if (individual.CitySequence.Count < 52)
                return false;

            int priorCount = individual.CitySequence.Count;

            List<int> distinctSequence = individual.CitySequence.Distinct().ToList();

            if (priorCount != distinctSequence.Count)
                return false;
            else
                return true;
        }
    }
}
