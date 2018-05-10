using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Representation;
using TravellingSalesPerson.Utils;

namespace TravellingSalesPerson.Evolution_algorithm_blocks
{
    class GeneticOperators
    {
        Random random = new Random();

        public List<Individual> GenerateDescendants(Individual father, Individual mother)
        {
            Individual mutatedFather = MutateMember(father);
            Individual mutatedMother = MutateMember(mother);

            if (StaticOperations.ValidateIndividual(mutatedFather) == false)
                throw new NotSupportedException();
            if (StaticOperations.ValidateIndividual(mutatedMother) == false)
                throw new NotSupportedException();

            List<Individual> crossOverIndividuals = PMX(mutatedMother, mutatedFather);

            foreach (Individual individual in crossOverIndividuals)
            {
                if (StaticOperations.ValidateIndividual(individual) == false)
                    throw new NotSupportedException();
            }

            return crossOverIndividuals;
        }

        private Individual MutateMember(Individual individual)
        {
            List<int> individualValues = new List<int>();
            foreach (int value in individual.CitySequence)
            {
                individualValues.Add(value);
            }

            int mutationEffect = random.Next(0, 2);

            if (mutationEffect == 0)
            {
                int swapFirst = random.Next(0, individualValues.Count);
                int swapSecond = random.Next(0, individualValues.Count);

                int temp = individualValues[swapFirst];
                individualValues[swapFirst] = individualValues[swapSecond];
                individualValues[swapSecond] = temp;
            }
            else
            {
                int start = random.Next(0, individualValues.Count - 1);
                int end = random.Next(start + 1, individualValues.Count - 1);

                List<int> inversedSequence = individualValues.Skip(start).Take(end - start + 1).ToList();
                inversedSequence.Reverse();

                individualValues.RemoveRange(start, end - start + 1);
                individualValues.InsertRange(start, inversedSequence);
            }

            Individual mutatedIndividual = new Individual(individualValues);
            return mutatedIndividual;
        }

        private List<Individual> PMX(Individual mother, Individual father)
        {
            int firstBreak = random.Next(1, mother.CitySequence.Count / 2);
            int secondBreak = random.Next(firstBreak + 1, mother.CitySequence.Count);

            // First descendant
            List<int> firstValues = new List<int>();
            firstValues.AddRange(mother.CitySequence.Take(firstBreak));
            List<int> finalRange = mother.CitySequence.Skip(secondBreak).ToList();
            List<int> middlePart = father.CitySequence.Skip(firstBreak).Take(secondBreak - firstBreak).ToList();
            List<int> middlePartMap = mother.CitySequence.Skip(firstBreak - 1).Take(secondBreak - firstBreak).ToList();
            List<int> missingValues = Enumerable.Range(1, mother.CitySequence.Count).ToList();
            foreach (int value in firstValues)
            {
                missingValues.Remove(value);
            }
            foreach (int value in finalRange)
            {
                missingValues.Remove(value);
            }
            for (int i = 0; i < middlePart.Count; i++)
            {
                // If there is no conflict, value is untouched
                if (missingValues.Contains(middlePart[i]))
                    missingValues.Remove(middlePart[i]);
                // If conflict is reached, try using mapping to other parent
                else
                {
                    // If mapping is successful, use that value
                    if (missingValues.Contains(middlePartMap[i]))
                    {
                        middlePart[i] = middlePartMap[i];
                        missingValues.Remove(middlePartMap[i]);
                    }
                    // If not, generate random missing value
                    else
                    {
                        int index = random.Next(0, missingValues.Count);
                        middlePart[i] = missingValues[index];
                        missingValues.RemoveAt(index);
                    }
                }
            }
            List<int> fullValues = new List<int>();
            fullValues.AddRange(firstValues);
            fullValues.AddRange(middlePart);
            fullValues.AddRange(finalRange);
            Individual daughter = new Individual(fullValues);

            // Second descendant
            firstValues = new List<int>();
            firstValues.AddRange(father.CitySequence.Take(firstBreak));
            finalRange = father.CitySequence.Skip(secondBreak).ToList();
            middlePart = mother.CitySequence.Skip(firstBreak - 1).Take(secondBreak - firstBreak).ToList();
            middlePartMap = father.CitySequence.Skip(firstBreak - 1).Take(secondBreak - firstBreak).ToList();
            missingValues = Enumerable.Range(1, father.CitySequence.Count).ToList();
            foreach (int value in firstValues)
            {
                missingValues.Remove(value);
            }
            foreach (int value in finalRange)
            {
                missingValues.Remove(value);
            }
            for (int i = 0; i < middlePart.Count; i++)
            {
                // If there is no conflict, value is untouched
                if (missingValues.Contains(middlePart[i]))
                    missingValues.Remove(middlePart[i]);
                // If conflict is reached, try using mapping to other parent
                else
                {
                    // If mapping is successful, use that value
                    if (missingValues.Contains(middlePartMap[i]))
                    {
                        middlePart[i] = middlePartMap[i];
                        missingValues.Remove(middlePartMap[i]);
                    }
                    // If not, generate random missing value
                    else
                    {
                        int index = random.Next(0, missingValues.Count);
                        middlePart[i] = missingValues[index];
                        missingValues.RemoveAt(index);
                    }
                }
            }
            fullValues = new List<int>();
            fullValues.AddRange(firstValues);
            fullValues.AddRange(middlePart);
            fullValues.AddRange(finalRange);
            Individual son = new Individual(fullValues);

            return new List<Individual> { daughter, son };
        }
    }
}
