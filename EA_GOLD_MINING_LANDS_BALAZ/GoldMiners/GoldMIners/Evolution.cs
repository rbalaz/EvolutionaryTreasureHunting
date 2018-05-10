using GoldMiners.Evolution_algorithm_blocks;
using GoldMiners.Representation;
using GoldMiners.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoldMiners
{
    class Evolution
    {
        private int initialPopulationCount;
        private int evolutionCycles;
        private int goldFieldCount;
        private double bestFitness;
        private int bestFitnessEpoch;

        List<Individual> bestIndividualsPerGeneration;

        public Evolution(int initialPopulationCount, int evolutionCycles, int goldFieldCount)
        {
            this.initialPopulationCount = initialPopulationCount;
            this.evolutionCycles = evolutionCycles;
            this.goldFieldCount = goldFieldCount;
            bestFitness = double.MaxValue;
            bestFitnessEpoch = 0;

            bestIndividualsPerGeneration = new List<Individual>(evolutionCycles);
        }

        public void RealiseEvolution()
        {
            // Initialise population
            Initialisation initialisation = new Initialisation(initialPopulationCount, goldFieldCount);
            List<Individual> population = initialisation.InitialisePopulation();
            // Validate population
            for(int i = 0; i < population.Count; i++)
            {
                if (StaticOperations.ValidateIndividual(population[i]) == false)
                    throw new NotSupportedException();
            }

            // Evaluate population
            Evaluation evaluation = new Evaluation();
            for (int i = 0; i < population.Count; i++)
            {
                evaluation.EvaluateIndividual(population[i]);
            }

            // Evolution cycle
            for (int i = 0; i < evolutionCycles; i++)
            {
                Console.Write("# Epoch " + (i + 1));
                // Selection
                Selection selection = new Selection(population, population.Count);
                // Q tournament
                List<Individual> parents = selection.SelectParents(4);

                // Genetic operators
                List<Individual> descendants = new List<Individual>();
                GeneticOperators geneticOperators = new GeneticOperators();
                for (int j = 0; j < parents.Count; j = j + 2)
                {
                    descendants.AddRange(geneticOperators.GenerateDescendants(parents[j], parents[j + 1]));
                }

                // Evaluation
                for (int j = 0; j < descendants.Count; j++)
                {
                    evaluation.EvaluateIndividual(descendants[j]);
                }

                // Replacement 
                Replacement replacement = new Replacement(population, descendants, population.Count);
                if (i - bestFitnessEpoch < 100)
                    population = replacement.NextGeneration();
                else
                {
                    population = replacement.KillBestIndividuals();
                    bestFitness = double.MaxValue;
                }

                foreach (Individual individual in population)
                {
                    if (StaticOperations.ValidateIndividual(individual) == false)
                        throw new NotSupportedException();
                }

                // Save best member
                List<Individual> orderedPopulation = population.OrderBy(ind => ind.Fitness).ToList();
                bestIndividualsPerGeneration.Add(orderedPopulation[0]);
                Console.WriteLine(" Minimum fitness: " + orderedPopulation[0].Fitness + ".");

                if (orderedPopulation[0].Fitness < bestFitness)
                {
                    bestFitness = orderedPopulation[0].Fitness;
                    bestFitnessEpoch = i;
                }

                if (orderedPopulation[0].Fitness == 0)
                    break;
            }

            SaveDataToFile();
        }

        private void SaveDataToFile()
        {
            FileStream stream = new FileStream("jedince", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            for (int i = 0; i < bestIndividualsPerGeneration.Count; i++)
            {
                writer.WriteLine("# generacia " + (i + 1));
                Decoder decoder = new Decoder();
                List<List<Point>> allPoints = new List<List<Point>>();
                foreach (Point reference in bestIndividualsPerGeneration[i].References)
                {
                    allPoints.Add(decoder.ReconstructField(reference));
                }

                for (int j = 0; j < allPoints[0].Count; j++)
                {
                    for(int k = 0; k < allPoints.Count; k++)
                    {
                        writer.Write(allPoints[k][j].ToString() + " ");
                    }
                    writer.WriteLine();
                }

                writer.WriteLine();
                writer.WriteLine();
            }

            writer.Close();
            stream.Close();
        }
    }
}
