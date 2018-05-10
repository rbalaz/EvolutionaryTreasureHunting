using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class Replacement
    {
        private List<Representation> oldGeneration;
        private List<Representation> descendants;
        private int nextGenerationCount;

        public Replacement(List<Representation> oldGeneration, List<Representation> descendants, int nextGenerationCount)
        {
            this.oldGeneration = oldGeneration;
            this.descendants = descendants;
        }

        public List<Representation> NextGeneration()
        {
            // Generation replacement
            return descendants;
        }
    }
}
