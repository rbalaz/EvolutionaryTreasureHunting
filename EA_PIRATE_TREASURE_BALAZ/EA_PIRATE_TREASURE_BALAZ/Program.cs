using PirateTreasure.Evolution_algorithm_blocks;
using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PirateTreasure
{
    class Program
    {
        static void Main(string[] args)
        {
            int evolutionCycles = -1;
            while (true)
            {
                Console.Write("Enter the number of evolution cycles: ");
                string cyclesString = Console.ReadLine();
                if (int.TryParse(cyclesString, out evolutionCycles))
                    break;
            }
            int initialPopulationCount = -1;
            while (true)
            {
                Console.Write("Enter the number of members in initial population: ");
                string populationString = Console.ReadLine();
                if (int.TryParse(populationString, out initialPopulationCount))
                    break;
            }

            Evolution evolution = new Evolution(evolutionCycles, initialPopulationCount,
                initialPopulationCount, initialPopulationCount);
            evolution.RealizeEvolution();
            SaveToFile(evolution.BestPathsPerGeneration);
        }

        static void SaveToFile(List<Utils.Path> bestPathsPerGeneration)
        {
            FileStream stream = new FileStream("jedince", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            for (int i = 0; i < bestPathsPerGeneration.Count; i = i + 2)
            {
                writer.WriteLine("# generacia " + (i / 2 + 1));
                for (int j = 0; j < bestPathsPerGeneration[i].Points.Count; j++)
                {
                    writer.Write(bestPathsPerGeneration[i].Points[j].X.ToString().PadLeft(2) + " ");
                    writer.Write(bestPathsPerGeneration[i].Points[j].Y.ToString().PadLeft(2) + " ");
                    writer.Write(bestPathsPerGeneration[i + 1].Points[j].X.ToString().PadLeft(2) + " ");
                    writer.WriteLine(bestPathsPerGeneration[i + 1].Points[j].Y.ToString().PadLeft(2));
                }
                writer.WriteLine();
                writer.WriteLine();
            }
        }
    }
}
