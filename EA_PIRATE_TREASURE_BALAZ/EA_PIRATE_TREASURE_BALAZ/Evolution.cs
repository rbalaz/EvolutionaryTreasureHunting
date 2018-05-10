using PirateTreasure.Evolution_algorithm_blocks;
using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PirateTreasure
{
    class Evolution
    {
        private int evolutionCycles;
        private int initialPopulationCount;
        private int parentsCount;
        private int descendantsCount;
        public List<Path> BestPathsPerGeneration { get; private set; }

        public Evolution(int evolutionCycles, int initialPopulationCount, int parentsCount, int descendantsCount)
        {
            this.evolutionCycles = evolutionCycles;
            this.initialPopulationCount = initialPopulationCount;
            this.parentsCount = parentsCount;
            this.descendantsCount = descendantsCount;
            BestPathsPerGeneration = new List<Path>();
        }

        public void RealizeEvolution()
        {
            Initialisation initialisation = new Initialisation(initialPopulationCount);
            // Initialisation - validated 
            List<Path> population = initialisation.GenerateInitialPopulation();
            for (int i = 0; i < population.Count; i++)
            {
                if (StaticOperations.ValidatePath(population[i]) == false)
                    throw new NotSupportedException();
            }
            // Evaluation
            Evaluation evaluation = new Evaluation();
            evaluation.EvaluatePopulation(population);
            // Encoding
            List<Representation> representations = new List<Representation>();
            Decoder decoder = new Decoder();
            foreach (Path path in population)
            {
                Representation representation = decoder.EncodePath(path);
                representations.Add(representation);
            }
            // Evolution cycle
            for (int i = 0; i < evolutionCycles; i++)
            {
                // Selection
                Selection selection = new Selection(parentsCount, representations);
                List<Representation> parents = selection.SelectParents();
                // Genetic operator - validated
                GeneticOperator geneticOperator = new GeneticOperator(descendantsCount, parents);
                List<Representation> descendants = geneticOperator.GenerateDescendants();
                // Decoding
                List<Path> descendantPaths = new List<Path>();
                foreach (Representation representation in descendants)
                {
                    Path path = decoder.DecodeRepresentation(representation);
                    if (StaticOperations.ValidatePath(path) == false)
                        throw new NotSupportedException();
                    descendantPaths.Add(path);
                }
                // Evaluation
                evaluation.EvaluatePopulation(descendantPaths);
                for (int j = 0; j < descendants.Count; j++)
                {
                    descendants[j].Fitness = descendantPaths[j].Fitness;
                }
                // Replacement
                Replacement replacement = new Replacement(parents, descendants, initialPopulationCount);
                representations = replacement.NextGeneration();
                // Save to export file
                SaveTwoBestMembers(representations);
            }
        }

        private void SaveTwoBestMembers(List<Representation> representations)
        {
            double maxFitness = representations.Max(item => item.Fitness);
            Representation best = representations.Find(item => item.Fitness == maxFitness);

            double secondBestFitness = 0;
            int secondBestIndex = 0;
            for (int i = 0; i < representations.Count; i++)
            {
                if (representations[i] == best)
                    continue;

                if (representations[i].Fitness > secondBestFitness)
                {
                    secondBestFitness = representations[i].Fitness;
                    secondBestIndex = i;
                }
            }

            Representation secondBest = representations[secondBestIndex];

            Decoder decoder = new Decoder();

            BestPathsPerGeneration.Add(decoder.DecodeRepresentation(best));
            BestPathsPerGeneration.Add(decoder.DecodeRepresentation(secondBest));
        }
    }
}
