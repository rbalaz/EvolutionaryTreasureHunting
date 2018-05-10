using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateTreasure.Utils;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class Evaluation
    {
        public void EvaluatePopulation(List<Path> population)
        {
            foreach (Path path in population)
            {
                path.Fitness = EvaluatePath(path);
            }
        }

        private double EvaluatePath(Path path)
        {
            double fitness = 0.0;
            for (int i = 0; i < path.Points.Count; i++)
            {
                fitness += EvaluatePoint(path.Points[i]);
            }

            return fitness;
        }

        private double EvaluatePoint(Point point)
        {
            return Math.Max(1.0 / 30.0 * point.Y, 1 / 40 * (41 - point.X));
        }
    }
}
