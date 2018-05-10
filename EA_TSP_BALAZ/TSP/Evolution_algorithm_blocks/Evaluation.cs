using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Representation;
using TravellingSalesPerson.Utils;

namespace TravellingSalesPerson.Evolution_algorithm_blocks
{
    class Evaluation
    {
        private ReferenceList referenceList;

        public Evaluation(ReferenceList referenceList)
        {
            this.referenceList = referenceList;
        }

        public void EvaluateIndividual(Individual individual)
        {
            Decoder decoder = new Decoder(referenceList);
            List<Point> path = decoder.DecodeIndividual(individual);

            double totalDistance = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                totalDistance += Point.PointDistance2D(path[i], path[i + 1]);
            }
            totalDistance += Point.PointDistance2D(path[0], path[path.Count - 1]);

            individual.Fitness = totalDistance;
        }
    }
}
