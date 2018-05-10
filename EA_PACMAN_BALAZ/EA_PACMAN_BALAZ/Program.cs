using Pacman.Evolution_algorithm_blocks;
using Pacman.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman
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

            bool firstCriteria = false;
            while (true)
            {
                Console.Write("Include survival criteria? ");
                string firstCriteriaString = Console.ReadLine();
                if (firstCriteriaString.ToLower().Contains("y"))
                {
                    firstCriteria = true;
                    break;
                }
                else if (firstCriteriaString.ToLower().Contains("n"))
                {
                    break;
                }
            }

            bool secondCriteria = false;
            while (true)
            {
                Console.Write("Include treasure criteria? ");
                string secondCriteriaString = Console.ReadLine();
                if (secondCriteriaString.ToLower().Contains("y"))
                {
                    secondCriteria = true;
                    break;
                }
                else if (secondCriteriaString.ToLower().Contains("n"))
                {
                    break;
                }
            }

            bool thirdCriteria = false;
            while (true)
            {
                Console.Write("Include safety criteria? ");
                string thirdCriteriaString = Console.ReadLine();
                if (thirdCriteriaString.ToLower().Contains("y"))
                {
                    thirdCriteria = true;
                    break;
                }
                else if (thirdCriteriaString.ToLower().Contains("n"))
                {
                    break;
                }
            }

            if (firstCriteria == false && secondCriteria == false && thirdCriteria == false)
            {
                Console.WriteLine("Invalid setting.");
                Console.ReadLine();
            }
            else
            {
                Evolution evolution = new Evolution(populationCount, evolutionCycles,
                    firstCriteria, secondCriteria, thirdCriteria);
                evolution.RealiseEvolution();
            }
        }
    }
}
