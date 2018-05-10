using Pacman.Representation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pacman.Evolution_algorithm_blocks
{
    public class Selection
    {
        private List<Individual> population;
        private int parentsCount;

        public Selection(List<Individual> population, int parentsCount)
        {
            this.population = population;
            this.parentsCount = parentsCount;
        }

        public List<Individual> SelectParents(int q)
        {
            // Q tournament selection
            Evaluation evaluation = new Evaluation();
            Random random = new Random();
            population = evaluation.EvaluateIndividuals(population);

            List<Individual> parents = new List<Individual>(parentsCount);
            for (int i = 0; i < parentsCount; i++)
            {
                List<Individual> competitors = new List<Individual>();
                for (int j = 0; j < q; j++)
                {
                    int index = random.Next(0, population.Count);
                    competitors.Add(population[index]);
                }

                // Find best synthetic fitness
                int maxFitness = competitors.Min(item => item.Fitness);
                List<Individual> winners = competitors.Where(item => item.Fitness == maxFitness).ToList();
                if (winners.Count == 1)
                    parents.AddRange(winners);
                else
                {
                    List<int> crowdingDistances = new List<int>();
                    foreach (Individual individual in winners)
                    {
                        crowdingDistances.Add(CalculateCrowdingDistance(individual, population));
                    }
                    int maxCrowdingDistance = crowdingDistances.Max();
                    int maxCrowdingDistanceIndex = crowdingDistances.FindIndex(item => item == maxCrowdingDistance);
                    parents.Add(winners[maxCrowdingDistanceIndex]);
                }
            }

            return parents;
        }

        private int CalculateCrowdingDistance(Individual individual, List<Individual> population)
        {
            List<int> closestValues = new List<int>();
            for (int i = 0; i < individual.FitnessVector.Count; i++)
                closestValues.Add(int.MaxValue);

            bool firstOccurence = true;
            foreach (Individual member in population)
            {
                if (member.FitnessVector.SequenceEqual(individual.FitnessVector) && firstOccurence)
                {
                    firstOccurence = false;
                    continue;
                }

                for (int i = 0; i < member.FitnessVector.Count; i++)
                {
                    int distance = Math.Abs(member.FitnessVector[i] - individual.FitnessVector[i]);
                    if (distance < closestValues[i])
                        closestValues[i] = distance;
                }
            }

            return closestValues.Sum();
        }
    }
}
