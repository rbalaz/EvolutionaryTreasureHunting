using Medallion;
using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class Replacement
    {
        private List<Representation> oldGeneration;
        private List<Representation> descendants;
        private int nextGenerationCount;

        public Replacement(List<Representation> oldGeneration, List<Representation> descendants, int nextGenerationCount)
        {
            this.oldGeneration = oldGeneration;
            this.descendants = descendants;
            this.nextGenerationCount = nextGenerationCount;
        }

        public List<Representation> DuplicationElimination(int mutationAttempts, 
            int similarMemberCountThreshold, int similarityThreshold)
        {
            // New population will consist of
            // a) elite ten percent of unique old members
            // b) similar descendants will be repeatadly mutated to achieve higher chance of 
            // keeping multiple candidates

            // Choose ten percent of unique members
            List<Representation> eliteMembers = UniqueEliteSelection(oldGeneration.Count / 10 < 6 ? 6 : oldGeneration.Count / 10);
            // Test all descendants
            List<Representation> survivingDescendants = new List<Representation>();

            Decoder decoder = new Decoder();
            Evaluation evaluation = new Evaluation();
            for (int i = 0; i < descendants.Count; i++)
            {
                Representation descendant = descendants[i];
                double startFitness = descendant.Fitness;
                List<double> distances = new List<double>();
                foreach (Representation oldMember in oldGeneration)
                    distances.Add(StaticOperations.TotalDistanceBetweenPaths(descendant, oldMember));

                int similarCount = distances.Count(item => item <= similarityThreshold);
                if (similarCount > similarMemberCountThreshold)
                {
                    GeneticOperator geneticOperator = new GeneticOperator();
                    int attempts = 0;
                    while (similarCount > similarMemberCountThreshold && attempts < mutationAttempts)
                    {
                        descendant = geneticOperator.MutateMember(descendant);

                        distances.Clear();
                        foreach (Representation oldMember in oldGeneration)
                            distances.Add(StaticOperations.TotalDistanceBetweenPaths(descendant, oldMember));

                        similarCount = distances.Count(item => item <= similarityThreshold);
                        attempts++;

                        // Prevents repetitive mutation to have an aspiring solution stuck at lower fitness values
                        if (IsUnique(descendant) && evaluation.EvaluatePath(decoder.DecodeRepresentation(descendant)) > startFitness)
                            break;
                    }
                    survivingDescendants.Add(descendant);
                }
                else
                    survivingDescendants.Add(descendant);
            }

            // Some members were mutated and their fitness is unknown
            foreach (Representation descendant in survivingDescendants)
            {
                if (descendant.Fitness == 0)
                {
                    descendant.Fitness = evaluation.EvaluatePath(decoder.DecodeRepresentation(descendant));
                }
            }
            survivingDescendants.OrderByDescending(item => item.Fitness);

            eliteMembers.AddRange(survivingDescendants.Take(nextGenerationCount - eliteMembers.Count));

            return eliteMembers;
        }

        public List<Representation> GenerationReplacement()
        {
            List<Representation> everyone = new List<Representation>();
            
            everyone.AddRange(oldGeneration);
            everyone.AddRange(descendants);
            everyone = everyone.OrderByDescending(item => item.Fitness).ToList();

            List<Representation> newGeneration = new List<Representation>();
            List<Representation> eliteMembers = UniqueEliteSelection(oldGeneration.Count / 10 < 6 ? 6 : oldGeneration.Count / 10);
            foreach (Representation representation in eliteMembers)
            {
                Representation member = everyone.Find(item => item.Values.SequenceEqual(representation.Values));
                everyone.Remove(member);
            }
            everyone.Shuffle();
            newGeneration.AddRange(eliteMembers);
            newGeneration.AddRange(everyone.Take(oldGeneration.Count - eliteMembers.Count));

            return newGeneration;
        }

        public List<Representation> GenerationReplacement2()
        {
            List<Representation> everyone = new List<Representation>();

            everyone.AddRange(descendants);
            everyone = everyone.OrderByDescending(item => item.Fitness).ToList();

            List<Representation> newGeneration = new List<Representation>();
            List<Representation> eliteMembers = UniqueEliteSelection(oldGeneration.Count / 10 < 6 ? 6 : oldGeneration.Count / 10);
            
            everyone.Shuffle();
            newGeneration.AddRange(eliteMembers);
            newGeneration.AddRange(everyone.Take((int)(0.9 * oldGeneration.Count)));

            return newGeneration;
        }

        public List<Representation> NextGeneration()
        {
            // New generation will consist of both old and new members
            // Genetic operators generate as many descendants as there are parents
            // Every descendant will compete against a small selection of old generation
            // From that selection, the most similar member is chosen and competes against the descendant
            // Winner remains in population
            List<Representation> newGeneration = StaticOperations.CloneList(oldGeneration);
            List<Representation> eliteMembers = UniqueEliteSelection(oldGeneration.Count / 10 < 6 ? 6 : oldGeneration.Count / 10);
            foreach (Representation representation in eliteMembers)
            {
                Representation member = newGeneration.Find(item => item.Values.SequenceEqual(representation.Values));
                newGeneration.Remove(member);
            }
            List<Representation> survivingDescendatns = new List<Representation>();
            descendants.Shuffle();
            descendants = descendants.Skip(eliteMembers.Count).ToList();
            Random random = new Random();
            foreach (Representation descendant in descendants)
            {
                // Generate random selection of old members
                int sampleCount = random.Next(3, 7);
                int[] sampleIndices = new int[sampleCount];
                for (int i = 0; i < sampleCount; i++)
                {
                    sampleIndices[i] = random.Next(0, newGeneration.Count);
                }
                List<Representation> sampleMembers = new List<Representation>();
                for (int i = 0; i < sampleCount; i++)
                {
                    sampleMembers.Add(newGeneration[sampleIndices[i]]);
                }

                // Calculate, which of the selected members is more similar to current member
                int[] distancesField = new int[sampleCount];
                for (int i = 0; i < sampleMembers.Count; i++)
                {
                    distancesField[i] = StaticOperations.TotalDistanceBetweenPaths(descendant, sampleMembers[i]);
                }
                int minDistanceIndex = 0;
                for (int i = 0; i < distancesField.Length; i++)
                {
                    if (distancesField[minDistanceIndex] < distancesField[i])
                        minDistanceIndex = i;
                }

                // Stochastic competition
                // Descendant and old member fight in stochastic probabilistic tournament
                // based on their fitness
                int selectingNumber = random.Next(0, (int)(descendant.Fitness + sampleMembers[minDistanceIndex].Fitness));
                if (selectingNumber <= (int)descendant.Fitness)
                {
                    survivingDescendatns.Add(descendant);
                    newGeneration.Remove(sampleMembers[minDistanceIndex]);
                }

                // Compare fitness of descendant and calculated similar member
                // Winner goes into new generation
                //if (descendant.Fitness > sampleMembers[minDistanceIndex].Fitness)
                //{
                //    survivingDescendatns.Add(descendant);
                //    newGeneration.Remove(sampleMembers[minDistanceIndex]);
                //}
            }

            // New generation consists of old generation members that won their duels and 
            // descendants that eliminated losing old generation members
            newGeneration.AddRange(survivingDescendatns);
            newGeneration.AddRange(eliteMembers);

            return newGeneration;
        }

        // Returns at most eliteCount best UNIQUE members from oldGeneration
        private List<Representation> UniqueEliteSelection(int eliteCount)
        {
            List<Representation> orderedOldGeneration = oldGeneration.OrderByDescending(item => item.Fitness).ToList();
            List<Representation> uniqueElite = new List<Representation>();

            for (int i = 0; i < eliteCount; i++)
            {
                Representation currentBest = orderedOldGeneration[0];
                orderedOldGeneration.RemoveAll(item => item.Values.SequenceEqual(currentBest.Values));
                uniqueElite.Add(currentBest);
            }

            return uniqueElite;
        }

        private bool IsUnique(Representation candidate)
        {
            foreach (Representation representation in oldGeneration)
            {
                if (candidate.Values.SequenceEqual(representation.Values))
                    return false;
            }
            foreach (Representation representation in descendants)
            {
                if (candidate.Values.SequenceEqual(representation.Values))
                    return false;
            }

            return true;
        }
    }
}
