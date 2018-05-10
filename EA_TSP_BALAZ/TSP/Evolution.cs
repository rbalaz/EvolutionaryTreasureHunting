using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Evolution_algorithm_blocks;
using TravellingSalesPerson.Representation;
using TravellingSalesPerson.Utils;

namespace TravellingSalesPerson
{
    class Evolution
    {
        private int initialPopulationCount;
        private int evolutionCycles;
        private List<Individual> bestIndividualPerGeneration;

        public Evolution(int initialPopulationCount, int evolutionCycles)
        {
            this.initialPopulationCount = initialPopulationCount;
            this.evolutionCycles = evolutionCycles;
            bestIndividualPerGeneration = new List<Individual>();
        }

        public void RealiseEvolution()
        {
            // Initialisation
            ReferenceList referenceList = new ReferenceList("tsp.riesenia");
            Initialisation initialisation = new Initialisation(initialPopulationCount, referenceList);
            List<Individual> population = initialisation.InitialisePopulation();

            // Evaluation
            Evaluation evaluation = new Evaluation(referenceList);
            foreach (Individual individual in population)
            {
                evaluation.EvaluateIndividual(individual);
            }

            // Validation
            foreach (Individual individual in population)
            {
                if (StaticOperations.ValidateIndividual(individual) == false)
                    throw new NotSupportedException();
            }

            // Evolution cycles
            for (int i = 0; i < evolutionCycles; i++)
            {
                Console.Write("Epoch #" + i);
                // Selection
                Selection selection = new Selection(population, population.Count);
                List<Individual> parents = selection.SelectParents(2);

                // Genetic operators
                GeneticOperators geneticOperators = new GeneticOperators();
                List<Individual> descendants = new List<Individual>();
                for (int j = 0; j < parents.Count; j = j + 2)
                {
                    descendants.AddRange(geneticOperators.GenerateDescendants(parents[j], parents[j + 1]));
                }

                // Validation
                foreach (Individual individual in descendants)
                {
                    if (StaticOperations.ValidateIndividual(individual) == false)
                        throw new NotSupportedException();
                }

                // Evaluation
                foreach (Individual individual in descendants)
                {
                    evaluation.EvaluateIndividual(individual);
                }

                // Replacement
                Replacement replacement = new Replacement(population, descendants, population.Count);
                population = replacement.NextGeneration();

                // Save best individual
                List<Individual> orderedPopulation = population.OrderBy(item => item.Fitness).ToList();
                bestIndividualPerGeneration.Add(orderedPopulation[0]);
                Console.WriteLine(" Minimum fitness: " + orderedPopulation[0].Fitness);
            }

            SaveBestIndividualsToFile(referenceList);
        }

        private void SaveBestIndividualsToFile(ReferenceList referenceList)
        {
            FileStream stream = new FileStream("jedince", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            for (int i = 0; i < bestIndividualPerGeneration.Count; i++)
            {
                writer.WriteLine("# generacia " + i);
                int index;
                Point point;
                for (int j = 0; j < bestIndividualPerGeneration[i].CitySequence.Count; j++)
                {
                    index = bestIndividualPerGeneration[i].CitySequence[j];
                    point = referenceList.PointOrder[index - 1];
                    writer.WriteLine(point.ToString());
                }
                index = bestIndividualPerGeneration[i].CitySequence[0];
                point = referenceList.PointOrder[index - 1];
                writer.WriteLine(point.ToString());

                writer.WriteLine();
                writer.WriteLine();
            }
        }
    }
}
