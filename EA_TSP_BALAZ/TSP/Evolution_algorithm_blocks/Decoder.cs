using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesPerson.Representation;
using TravellingSalesPerson.Utils;

namespace TravellingSalesPerson.Evolution_algorithm_blocks
{
    class Decoder
    {
        private ReferenceList referenceList;

        public Decoder(ReferenceList referenceList)
        {
            this.referenceList = referenceList;
        }

        public List<Point> DecodeIndividual(Individual individual)
        {
            List<Point> path = new List<Point>();

            foreach (int reference in individual.CitySequence)
            {
                path.Add(referenceList.PointOrder[reference - 1]);
            }

            return path;
        }
    }
}
