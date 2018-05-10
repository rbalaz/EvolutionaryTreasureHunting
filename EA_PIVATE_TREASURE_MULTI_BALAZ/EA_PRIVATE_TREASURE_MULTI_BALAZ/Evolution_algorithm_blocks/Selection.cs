using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class Selection
    {
        private int parentsPopulationCount;
        private List<Representation> currentPopulation;

        public Selection(int parentsPopulationCount, List<Representation> currentPopulation)
        {
            this.parentsPopulationCount = parentsPopulationCount;
            this.currentPopulation = currentPopulation;
        }

        public List<Representation> SelectCombinedParents()
        {
            // Recalculate fitness based on similarity criteria
            // Choose random reference member
            Random random = new Random();
            int referenceIndex = random.Next(0, currentPopulation.Count);
            RecalculateFitness(currentPopulation[referenceIndex]);
            // Stochastic universal selection
            List<double> rouletteSegments = new List<double>();
            // Calculate estimated parents selection using proportional selection
            double averageFitness = currentPopulation.Average(item => item.CombinedFitness);
            foreach (Representation representation in currentPopulation)
            {
                double rouletteSegment = representation.CombinedFitness / averageFitness;
                rouletteSegments.Add(rouletteSegment);
            }
            // Generate values for each of the roulette segmentors
            // First value is generated randomly from interval <0,rho>
            // Every other segmentor is derived from the first value because distance 
            // between neighbouring segmentors is always exactly 1
            double firstValue = random.NextDouble() * parentsPopulationCount;
            List<double> chosenValues = new List<double>();
            double currentValue = firstValue;
            chosenValues.Add(firstValue);
            while (chosenValues.Count < parentsPopulationCount)
            {
                currentValue += 1;
                if (currentValue > parentsPopulationCount)
                    currentValue -= parentsPopulationCount;
                chosenValues.Add(currentValue);
            }
            // Choose parents
            List<Representation> parents = new List<Representation>();
            foreach (double chosenValue in chosenValues)
            {
                parents.Add(currentPopulation[CumulativeDistributionIndex(rouletteSegments, chosenValue)]);
            }

            return parents;
        }

        public List<Representation> SelectParents()
        {
            // Stochastic universal selection
            List<double> rouletteSegments = new List<double>();
            // Calculate estimated parents selection using proportional selection
            double averageFitness = currentPopulation.Average(item => item.Fitness);
            foreach (Representation representation in currentPopulation)
            {
                double rouletteSegment = representation.Fitness / averageFitness;
                rouletteSegments.Add(rouletteSegment);
            }
            // Generate values for each of the roulette segmentors
            // First value is generated randomly from interval <0,rho>
            // Every other segmentor is derived from the first value because distance 
            // between neighbouring segmentors is always exactly 1
            Random random = new Random();
            double firstValue = random.NextDouble() * parentsPopulationCount;
            List<double> chosenValues = new List<double>();
            double currentValue = firstValue;
            chosenValues.Add(firstValue);
            while (chosenValues.Count < parentsPopulationCount)
            {
                currentValue += 1;
                if (currentValue > parentsPopulationCount)
                    currentValue -= parentsPopulationCount;
                chosenValues.Add(currentValue);
            }
            // Choose parents
            List<Representation> parents = new List<Representation>();
            foreach (double chosenValue in chosenValues)
            {
                parents.Add(currentPopulation[CumulativeDistributionIndex(rouletteSegments, chosenValue)]);
            }

            return parents;
        }

        private void RecalculateFitness(Representation reference)
        {
            // Recalculates fitness of members based on similarity criteria
            double weight = 0.4;
            foreach (Representation member in currentPopulation)
            {
                int distance = StaticOperations.TotalDistanceBetweenPaths(reference, member);
                // The higher the distance, the better 
                double newFitness = member.Fitness * (weight + (1 - weight) * distance);
                member.CombinedFitness = newFitness;
            }
        }

        private int FindMostCommonMemberIndex()
        {
            int mostCommonIndex = -1;
            int mostCommonCount = int.MinValue;

            for(int i = 0; i < currentPopulation.Count; i++)
            {
                int currentCount = 0;
                for (int j = i + 1; j < currentPopulation.Count; j++)
                {
                    if (currentPopulation[i].Values.SequenceEqual(currentPopulation[j].Values))
                        currentCount++;
                }
                if (currentCount > mostCommonCount)
                {
                    currentCount = mostCommonCount;
                    mostCommonIndex = i;
                }
            }

            return mostCommonIndex;
        }

        private int CumulativeDistributionIndex(List<double> rouletteSegments, double chosenValue)
        {
            double cumulativeSum = 0;
            for (int i = 0; i < rouletteSegments.Count; i++)
            {
                cumulativeSum += rouletteSegments[i];
                if (chosenValue < cumulativeSum)
                    return i;
            }

            throw new NullReferenceException();
        }
    }
}
