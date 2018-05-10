using Pacman.Evolution_algorithm_blocks;
using Pacman.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pacman.Utilities
{
    public static class StaticOperations
    {
        public static bool ValidateIndividual(Individual individual)
        {
            Decoder decoder = new Decoder();
            Path path = decoder.DecodeIndividual(individual);

            foreach (Point point in path.Points)
            {
                if (point.Y < 1 || point.Y > 30)
                    return false;
            }

            if (individual.Movements.Count < 39)
                return false;

            return true;
        }
    }
}
