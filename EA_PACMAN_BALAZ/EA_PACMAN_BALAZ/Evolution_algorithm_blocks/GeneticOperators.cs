using Pacman.Representation;
using Pacman.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Evolution_algorithm_blocks
{
    public class GeneticOperators
    {
        private Random random;

        public GeneticOperators()
        {
            random = new Random();
        }

        public List<Individual> GenerateDescendants(Individual mother, Individual father)
        {
            Individual mutatedMother = null;
            for (int i = 0; i < 5; i++)
            {
                mutatedMother = MutateIndividual(mother);
                if (StaticOperations.ValidateIndividual(mutatedMother))
                    break;
            }
            if (StaticOperations.ValidateIndividual(mutatedMother) == false)
                mutatedMother = mother;

            Individual mutatedFather = null;
            for (int i = 0; i < 5; i++)
            {
                mutatedFather = MutateIndividual(father);
                if (StaticOperations.ValidateIndividual(mutatedFather))
                    break;
            }
            if (StaticOperations.ValidateIndividual(mutatedFather) == false)
                mutatedFather = father;

            List<Individual> descendants = CrossOverIndividuals(mutatedMother, mutatedFather);
            if (StaticOperations.ValidateIndividual(descendants[0]) == false)
                descendants[0] = mother;
            if (StaticOperations.ValidateIndividual(descendants[1]) == false)
                descendants[1] = father;

            return descendants;
        }

        private Individual MutateIndividual(Individual individual)
        {
            Individual clone = Individual.CloneIndividual(individual);
            double chance = random.NextDouble();
            for (int i = 0; i < individual.Movements.Count; i++)
            {
                double value = random.NextDouble();
                if (value > chance)
                {
                    clone.Movements[i] = clone.Movements[i] == 0 ? 1 : 0;
                }
            }

            return clone;
        }

        private List<Individual> CrossOverIndividuals(Individual mother, Individual father)
        {
            List<int> perturbationVector = new List<int>(39);
            for (int i = 0; i < mother.Movements.Count; i++)
            {
                perturbationVector.Add(random.Next(0, 2));
            }

            List<int> daughterMovements = new List<int>();
            List<int> sonMovements = new List<int>();

            for (int i = 0; i < mother.Movements.Count; i++)
            {
                if (perturbationVector[i] == 0)
                {
                    daughterMovements.Add(father.Movements[i]);
                    sonMovements.Add(mother.Movements[i]);
                }
                else
                {
                    daughterMovements.Add(mother.Movements[i]);
                    sonMovements.Add(father.Movements[i]);
                }
            }

            Individual son = new Individual(sonMovements);
            Individual daughter = new Individual(daughterMovements);

            return new List<Individual> { son, daughter };
        }
    }
}
