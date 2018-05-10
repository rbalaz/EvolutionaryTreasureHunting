using System;
using System.Collections.Generic;
using System.Linq;
using Pacman.Representation;
using Pacman.Evolution_algorithm_blocks;
using System.IO;
using Pacman.Utilities;
using Medallion;

namespace Pacman
{
    public class Evolution
    {
        private int initialPopulationCount;
        private int evolutionCycles;
        private bool firstCriteria;
        private bool secondCriteria;
        private bool thirdCriteria;
        private List<Individual> bestIndividuals;

        public Evolution(int initialPopulationCount, int evolutionCycles,
            bool firstCriteria, bool secondCriteria, bool thirdCriteria)
        {
            this.initialPopulationCount = initialPopulationCount;
            this.evolutionCycles = evolutionCycles;
            this.firstCriteria = firstCriteria;
            this.secondCriteria = secondCriteria;
            this.thirdCriteria = thirdCriteria;
            bestIndividuals = new List<Individual>();
        }

        public void RealiseEvolution()
        {
            // Initialise population - validated
            Initialisation initialisation = new Initialisation(initialPopulationCount);
            List<Individual> population = initialisation.GenerateInitialPopulation();

            foreach (Individual individual in population)
            {
                if (StaticOperations.ValidateIndividual(individual) == false)
                    throw new NotSupportedException();
            }

            // Evaluate synthethic fitness of population
            Evaluation evaluation = new Evaluation(firstCriteria, secondCriteria, thirdCriteria);
            foreach (Individual individual in population)
            {
                // Only criteria with true value will be considered
                evaluation.EvaluateIndividual(individual);
            }

            //Evolution cycles
            for (int i = 0; i < evolutionCycles; i++)
            {
                Console.Write("Epoch #" + i);
                // Selection - q-tournament
                Selection selection = new Selection(population, initialPopulationCount);
                List<Individual> parents = selection.SelectParents(2);

                // Genetic operators
                List<Individual> descendants = new List<Individual>();
                GeneticOperators geneticOperators = new GeneticOperators();
                for (int j = 0; j < parents.Count; j = j + 2)
                {
                    descendants.AddRange(geneticOperators.GenerateDescendants(parents[j], parents[j + 1]));
                }

                // Evaluation
                foreach (Individual individual in descendants)
                {
                    // Only criteria with true value will be considered
                    evaluation.EvaluateIndividual(individual);
                }

                // Replacement 
                Replacement replacement = new Replacement(population, descendants, initialPopulationCount);
                population = replacement.NextGeneration();

                // Save best individual
                if (firstCriteria)
                {
                    List<Individual> paretSetSurvivors = population.Where(ind => ind.Fitness == 1 && ind.FitnessVector[0] == 1).ToList();
                    if (paretSetSurvivors.Count == 0)
                        bestIndividuals.Add(population[0]);
                    else
                    {
                        paretSetSurvivors.Shuffle();
                        bestIndividuals.Add(paretSetSurvivors[0]);
                    }

                    Console.WriteLine(" Paret set count: " + EvaluateParetSet(population));
                }
                else
                {
                    List<Individual> paretSetSurvivors = population.Where(ind => ind.Fitness == 1).ToList();
                    paretSetSurvivors.Shuffle();
                    bestIndividuals.Add(paretSetSurvivors[0]);
                    Console.WriteLine(" Paret set count: " + EvaluateParetSet(population));
                }
            }

            SaveBestIndividualsInFile();
        }

        private void SaveBestIndividualsInFile()
        {
            FileStream stream = new FileStream("jedince", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            Decoder decoder = new Decoder();

            for(int i = 0; i < bestIndividuals.Count; i++)
            {
                writer.WriteLine("# generacia " + i);
                Representation.Path path = decoder.DecodeIndividual(bestIndividuals[i]);
                foreach (Point point in path.Points)
                {
                    writer.WriteLine(point.ToString());
                }
                writer.WriteLine();
                List<Point> beasts = decoder.GetBeastsLocations(bestIndividuals[i]);
                foreach (Point point in beasts)
                {
                    writer.WriteLine(point.ToString());
                    writer.WriteLine();
                }
                writer.WriteLine();
            }

            writer.Close();
            stream.Close();
        }

        private int EvaluateParetSet(List<Individual> population)
        {
            List<Individual> uniqueParetSetMembers = new List<Individual>();

            foreach (Individual individual in population)
            {
                bool isUnique = true;
                foreach (Individual paretSetIndividual in uniqueParetSetMembers)
                {
                    if (individual.IsIdentical(paretSetIndividual))
                    {
                        isUnique = false;
                        break;
                    }
                }
                if (isUnique)
                    uniqueParetSetMembers.Add(individual);
            }

            return uniqueParetSetMembers.Count;
        }
    }
}
