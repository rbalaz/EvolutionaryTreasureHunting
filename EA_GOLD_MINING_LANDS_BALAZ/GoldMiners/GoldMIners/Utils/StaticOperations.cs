using GoldMiners.Evolution_algorithm_blocks;
using GoldMiners.Representation;
using System.Collections.Generic;
using System.Linq;

namespace GoldMiners.Utils
{
    static class StaticOperations
    {
        public static bool ValidateIndividual(Individual individual)
        {
            foreach (Point reference in individual.References)
            {
                if (reference.X < 1 || reference.X > 32 || reference.Y < 6 || reference.Y > 27)
                    return false;
            }

            return true;
        }
    }
}
