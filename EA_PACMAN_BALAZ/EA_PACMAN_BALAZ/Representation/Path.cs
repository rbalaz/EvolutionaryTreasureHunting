using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Representation
{
    public class Path
    {
        public List<Point> Points { get; private set; }

        public Path(List<Point> points)
        {
            Points = points;
        }
    }
}
