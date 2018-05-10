using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesPerson.Representation
{
    class Individual
    {
        public List<int> CitySequence { get; private set; }
        public double Fitness { get; set; }

        public Individual(List<int> citySequence)
        {
            CitySequence = citySequence;
        }
    }
}
