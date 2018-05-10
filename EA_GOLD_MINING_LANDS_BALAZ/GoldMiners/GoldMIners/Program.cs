using GoldMiners.Evolution_algorithm_blocks;
using GoldMiners.Representation;
using GoldMiners.Utils;
using System;
using System.Collections.Generic;

namespace GoldMiners
{
    class Program
    {
        static void Main(string[] args)
        {
            int populationCount = 0;
            while (true)
            {
                Console.Write("Number of individuals: ");
                string populationCountString = Console.ReadLine();
                if (int.TryParse(populationCountString, out populationCount))
                    break;
            }

            int goldFieldCount = 0;
            while (true)
            {
                Console.Write("Number of gold fields: ");
                string goldFieldCountString = Console.ReadLine();
                if (int.TryParse(goldFieldCountString, out goldFieldCount))
                    break;
            }

            int evolutionCycles = 0;
            while (true)
            {
                Console.Write("Number of evolution cycles: ");
                string evolutionCyclesString = Console.ReadLine();
                if (int.TryParse(evolutionCyclesString, out evolutionCycles))
                    break;
            }

            Evolution evolution = new Evolution(populationCount, evolutionCycles, goldFieldCount);
            evolution.RealiseEvolution();

            Console.ReadLine();
            //DecoderTesting();
        }

        public static void DecoderTesting()
        {
            List<Point> references = new List<Point>();
            Point reference = new Point(3, 21);
            references.Add(reference);
            reference = new Point(29, 7);
            references.Add(reference);
            reference = new Point(6, 27);
            references.Add(reference);
            reference = new Point(8, 18);
            references.Add(reference);
            reference = new Point(28, 9);
            references.Add(reference);
            reference = new Point(14, 11);
            references.Add(reference);
            reference = new Point(9, 13);
            references.Add(reference);
            reference = new Point(31, 14);
            references.Add(reference);
            reference = new Point(30, 23);
            references.Add(reference);
            reference = new Point(23, 25);
            references.Add(reference);
            reference = new Point(20, 15);
            references.Add(reference);
            reference = new Point(11, 17);
            references.Add(reference);
            reference = new Point(27, 12);
            references.Add(reference);
            reference = new Point(12, 5);
            references.Add(reference);
            reference = new Point(30, 11);
            references.Add(reference);

            Individual individual = new Individual(references);

            bool status = StaticOperations.ValidateIndividual(individual);
        }
    }
}
