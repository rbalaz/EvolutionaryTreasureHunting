using GoldMiners.Representation;
using GoldMiners.Utils;
using System;
using System.Collections.Generic;

namespace GoldMiners.Evolution_algorithm_blocks
{
    class GeneticOperators
    {
        private Random random;

        public GeneticOperators()
        {
            random = new Random();
        }

        public List<Individual> GenerateDescendants(Individual mother, Individual father)
        {
            Individual motherClone = Individual.CloneIndividual(mother);
            Individual fatherClone = Individual.CloneIndividual(father);

            double value = random.NextDouble();
            Individual mutatedMother = null, mutatedFather = null;
            int counter = 0;
            for (int i = 0; i < 5; i++)
            {
                mutatedMother = MutateIndividual(motherClone);
                if (StaticOperations.ValidateIndividual(mutatedMother))
                    break;
                counter++;
            }
            if (counter == 5)
                mutatedMother = mother;

            counter = 0;
            for (int i = 0; i < 5; i++)
            {
                mutatedFather = MutateIndividual(fatherClone);
                if (StaticOperations.ValidateIndividual(mutatedFather))
                    break;
                counter++;
            }
            if (counter == 5)
                mutatedFather = father;

            List<Individual> crossedOvers = CrossOverIndividuals(mutatedMother, mutatedFather);

            return crossedOvers;
        }

        private Individual MutateIndividual(Individual individual)
        {
            int mutationEffect = random.Next(0, 2);
            if (mutationEffect == 0)
            {
                double value = random.NextDouble();
                for (int i = 0; i < individual.References.Count; i++)
                {
                    double chance = random.NextDouble();

                    if (chance < value)
                    {
                        int xChange = (int)(SimpleRNG.GetNormal());
                        int yChange = (int)(SimpleRNG.GetNormal());
                        int xSign = Math.Sign(SimpleRNG.GetNormal());
                        int ySign = Math.Sign(SimpleRNG.GetNormal());

                        individual.References[i].X += xSign * xChange;
                        individual.References[i].Y += ySign * yChange;
                    }
                }
            }
            else
            {
                double value = random.NextDouble();
                for (int i = 0; i < individual.References.Count; i++)
                {
                    double chance = random.NextDouble();
                    if (chance < value)
                    {
                        int randX = random.Next(1, 33);
                        int randY = random.Next(6, 28);

                        individual.References[i].X = randX;
                        individual.References[i].Y = randY;
                    }
                }
            }

            return individual;
        }


        private List<Individual> CrossOverIndividuals(Individual mutatedMother, Individual mutatedfather)
        {
            List<int> perturbationVector = new List<int>();
            for (int i = 0; i < mutatedMother.References.Count; i++)
            {
                perturbationVector.Add(random.Next(0, 2));
            }

            List<Point> son = new List<Point>();
            List<Point> daughter = new List<Point>();

            for (int i = 0; i < perturbationVector.Count; i++)
            {
                if (perturbationVector[i] == 0)
                {
                    son.Add(mutatedMother.References[i]);
                    daughter.Add(mutatedfather.References[i]);
                }
                else
                {
                    son.Add(mutatedfather.References[i]);
                    daughter.Add(mutatedMother.References[i]);
                }
            }

            List<Individual> descendants = new List<Individual>
            {
                new Individual(son),
                new Individual(daughter)
            };

            return descendants;
        }
    }
}
