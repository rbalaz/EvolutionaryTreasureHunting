using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Utils
{
    class Representation
    {
        public double Fitness { get; set; }
        public List<int> Values { get; private set; }

        public Representation(double fitness, List<int> values)
        {
            Fitness = fitness;
            Values = values;
        }

        public Representation(List<int> values)
        {
            Values = values;
        }

        public void PrintValues()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[" + Values[0]);
            foreach (int value in Values.Skip(1))
            {
                builder.Append(";"+value);
            }
            builder.Append("]");

            Console.WriteLine(builder.ToString());
        }
    }
}
