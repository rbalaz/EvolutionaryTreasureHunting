using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Representation
{
    public class Individual
    {
        // Vector of movements
        // 0 - down right
        // 1 - up right
        public List<int> Movements { get; private set; }
        // Synthetic fitness constructed from relevant criteria
        public int Fitness { get; set; }
        // Fitnesses by criteria
        public List<int> FitnessVector { get; set; }

        public Individual(List<int> movements)
        {
            Movements = movements;
        }

        public bool Dominates(Individual subject)
        {
            // Check if any fitness values are lower -> that automatically means no domination
            for (int i = 0; i < FitnessVector.Count; i++)
            {
                if (FitnessVector[i] < subject.FitnessVector[i])
                    return false;
            }

            // If no lower values are found, there has to be at least one higher value for domination
            for (int i = 0; i < FitnessVector.Count; i++)
            {
                if (FitnessVector[i] > subject.FitnessVector[i])
                    return true;
            }

            return false;
        }

        public bool DominatedBy(Individual subject)
        {
            for (int i = 0; i < FitnessVector.Count; i++)
            {
                if (FitnessVector[i] > subject.FitnessVector[i])
                    return false;
            }

            return true;
        }

        public static Individual CloneIndividual(Individual individual)
        {
            List<int> movementsClone = new List<int>();
            foreach (int movement in individual.Movements)
            {
                movementsClone.Add(movement);
            }

            List<int> fitnessVector = new List<int>();
            foreach (int fitness in individual.FitnessVector)
            {
                fitnessVector.Add(fitness);
            }

            Individual clone = new Individual(movementsClone)
            {
                FitnessVector = fitnessVector
            };

            return clone;
        }

        public string GetFitnessVectorString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (int fitness in FitnessVector)
            {
                builder.Append(" " + fitness);
            }

            return builder.ToString();
        }

        public bool IsIdentical(Individual individual)
        {
            for(int i = 0; i < FitnessVector.Count; i++)
            {
                if (individual.FitnessVector[i] != FitnessVector[i])
                    return false;
            }

            return true;
        }
    }
}
