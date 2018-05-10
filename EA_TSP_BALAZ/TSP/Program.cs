using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesPerson
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

            int evolutionCycles = 0;
            while (true)
            {
                Console.Write("Number of evolution cycles: ");
                string evolutionCyclesString = Console.ReadLine();
                if (int.TryParse(evolutionCyclesString, out evolutionCycles))
                    break;
            }

            Evolution evolution = new Evolution(populationCount, evolutionCycles);
            evolution.RealiseEvolution();

            Console.ReadLine();
            //ListHandling();
        }

        static void ListHandling()
        {
            List<int> testList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            testList.RemoveRange(8, 2);
        }
    }
}
