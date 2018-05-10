using Medallion;
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
        //private double remapParameter;

        public Evolution(int evolutionCycles, int initialPopulationCount, int parentsCount, 
            int descendantsCount)
        {
            this.evolutionCycles = evolutionCycles;
            this.initialPopulationCount = initialPopulationCount;
            this.parentsCount = parentsCount;
            this.descendantsCount = descendantsCount;
            BestPathsPerGeneration = new List<Path>();
            //this.remapParameter = remapParameter;
        }

        public void RealizeEvolution()
        {
            Random random = new Random();
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
                Console.Write("Epoch #" + i + ".");
                // Reinitialisation happens every 1/10th iteration and randomly resets half of population
                // Elite 10% is untouched by this process
                if ((i % 500) == 0 && i != 0)
                    ReinitializePopulation(representations, (int)(3 * initialPopulationCount / 4));
                // Remap fitness using exponential remapping
                //RemapFitness(representations, remapParameter);
                // Selection
                Selection selection = new Selection(parentsCount, representations);
                List<Representation> parents = selection.SelectParents();
                //List<Representation> parents = selection.SelectCombinedParents();
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
                // Revaluate current population after fitness remapping
                //List<Path> currentPaths = new List<Path>();
                //foreach (Representation representation in representations)
                //{
                //    Path path = decoder.DecodeRepresentation(representation);
                //    currentPaths.Add(path);
                //}
                //evaluation.EvaluatePopulation(currentPaths);
                //for (int j = 0; j < representations.Count; j++)
                //{
                //    representations[j].Fitness = currentPaths[j].Fitness;
                //}
                // Replacement
                Replacement replacement = new Replacement(representations, descendants, initialPopulationCount);
                //representations = replacement.GenerationReplacement();
                //representations = replacement.NextGeneration();
                representations = replacement.DuplicationElimination(7, representations.Count / 20 < 3 ? representations.Count / 20 : 3, 20);
                Console.Write(" Maximum fitness: " + representations.Max(item => item.Fitness));
                // Save to export file
                SaveSixBestMembers(representations);
            }
        }

        private void SaveSixBestMembers(List<Representation> representations)
        {
            List<Representation> orderedList = representations.OrderByDescending(member => member.Fitness).ToList();
            double maxFitness = orderedList.Max(item => item.Fitness);
            List<Representation> elite = orderedList.Where(item => item.Fitness == maxFitness).ToList();
            List<Representation> uniqueSolutions = new List<Representation>
            {
                elite[0]
            };
            foreach (Representation member in elite)
            {
                bool isUnique = true;
                foreach (Representation solution in uniqueSolutions)
                {
                    if (member.Values.Count == solution.Values.Count)
                        isUnique = false;
                }
                    if (isUnique)
                        uniqueSolutions.Add(member);
            }
            orderedList.InsertRange(0, uniqueSolutions);
            Console.WriteLine(" Unique solutions found: " + uniqueSolutions.Count);

            Decoder decoder = new Decoder();
            List<Path> bestPaths = new List<Path>(6);
            for (int i = 0; i < 6; i++)
            {
                bestPaths.Add(decoder.DecodeRepresentation(orderedList[i]));
            }

            BestPathsPerGeneration.AddRange(bestPaths);
        }

        private void RemapFitness(List<Representation> population, double c)
        {
            population.OrderBy(item => item.Fitness);
            for (int i = 0; i < population.Count; i++)
            {
                population[i].Fitness = Math.Pow(c, population.Count - i - 1);
            }
        }

        private void ReinitializePopulation(List<Representation> oldGeneration, int membersToReplace)
        {
            // 1.) pick up unique elite members and remove them from current population
            List<Representation> newGeneration = new List<Representation>();
            List<Representation> eliteMembers = UniqueEliteSelection(oldGeneration.Count / 5 < 6 ? 6 : oldGeneration.Count / 10, oldGeneration);
            foreach (Representation representation in eliteMembers)
            {
                Representation member = oldGeneration.Find(item => item.Values.SequenceEqual(representation.Values));
                oldGeneration.Remove(member);
            }

            // 2.) randomly initialise new initialPopulationCount/2 members
            Initialisation initialisation = new Initialisation(membersToReplace < oldGeneration.Count - eliteMembers.Count ?
                membersToReplace : oldGeneration.Count - eliteMembers.Count);
            List<Path> newMembers = initialisation.GenerateInitialPopulation();
            Decoder decoder = new Decoder();
            List<Representation> switcherees = newMembers.Select(path => decoder.EncodePath(path)).ToList();

            // 3.) shuffle remains of current population and replace the first half with new member
            oldGeneration.Shuffle();
            oldGeneration.RemoveRange(0, switcherees.Count);
            oldGeneration.InsertRange(0, switcherees);

            // 4.) append elite members
            oldGeneration.InsertRange(0, eliteMembers);
        }

        private List<Representation> UniqueEliteSelection(int eliteCount, List<Representation> oldGeneration)
        {
            List<Representation> oldElite = oldGeneration.OrderByDescending(item => item.Fitness).Take(eliteCount).ToList();
            double maxFitness = oldElite.Max(item => item.Fitness);
            int maxFitnessCount = oldGeneration.Count(item => item.Fitness == maxFitness);
            if (maxFitnessCount > eliteCount)
            {
                List<Representation> uniqueOldElite = new List<Representation>();
                List<Representation> bestMembers = oldGeneration.Where(item => item.Fitness == maxFitness).ToList();

                for (int i = 0; i < bestMembers.Count; i++)
                {
                    bool isUnique = true;
                    for (int j = 0; j < uniqueOldElite.Count; j++)
                    {
                        if (bestMembers[i].Values.Count == uniqueOldElite[j].Values.Count)
                        {
                            isUnique = false;
                            break;
                        }
                    }
                    if (isUnique)
                        uniqueOldElite.Add(bestMembers[i]);
                }

                if (uniqueOldElite.Count < eliteCount)
                    uniqueOldElite.AddRange(oldElite.Take(eliteCount - uniqueOldElite.Count));

                return uniqueOldElite;
            }
            else
                return oldElite;
        }
    }
}
