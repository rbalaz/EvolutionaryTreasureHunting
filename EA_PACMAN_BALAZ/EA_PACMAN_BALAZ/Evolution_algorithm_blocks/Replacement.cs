using Pacman.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Evolution_algorithm_blocks
{
    public class Replacement
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
            List<Individual> allIndividuals = new List<Individual>();
            allIndividuals.AddRange(oldGeneration);
            allIndividuals.AddRange(descendants);
            List<Individual> nextGeneration = new List<Individual>();
            Evaluation evaluation = new Evaluation();
            int fitnessCounter = 1;
            while (nextGeneration.Count < newGenerationCount)
            {
                allIndividuals = evaluation.EvaluateIndividuals(allIndividuals);
                int minFitness = allIndividuals.Min(item => item.Fitness);
                List<Individual> candidates = allIndividuals.FindAll(item => item.Fitness == minFitness);
                foreach (Individual individual in candidates)
                {
                    individual.Fitness = fitnessCounter;
                }

                if (nextGeneration.Count + candidates.Count <= newGenerationCount)
                {
                    nextGeneration.AddRange(candidates);
                    foreach (Individual candidate in candidates)
                    {
                        allIndividuals.Remove(candidate);
                    }
                }
                else
                {
                    List<int> crowdingDistances = new List<int>();
                    foreach (Individual individual in candidates)
                    {
                        crowdingDistances.Add(CalculateCrowdingDistance(individual, allIndividuals));
                    }
                    while (nextGeneration.Count < newGenerationCount)
                    {
                        int maxCrowdingDistance = crowdingDistances.Max();
                        int maxCrowdingDistanceIndex = crowdingDistances.FindIndex(item => item == maxCrowdingDistance);
                        nextGeneration.Add(candidates[maxCrowdingDistance]);
                        crowdingDistances.RemoveAt(maxCrowdingDistance);
                        candidates.RemoveAt(maxCrowdingDistance);
                    }
                }
                fitnessCounter++;
            }

            return nextGeneration;
        }

        private int CalculateCrowdingDistance(Individual individual, List<Individual> population)
        {
            List<int> closestValues = new List<int>();
            for (int i = 0; i < individual.FitnessVector.Count; i++)
                closestValues.Add(int.MaxValue);

            foreach (Individual member in population)
            {
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
