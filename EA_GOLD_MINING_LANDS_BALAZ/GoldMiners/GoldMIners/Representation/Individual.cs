using System.Collections.Generic;

namespace GoldMiners.Representation
{
    class Individual
    {
        public List<Point> References { get; private set; }
        public double Fitness { get; set; }

        public Individual(List<Point> references)
        {
            References = references;
        }

        public Individual(List<Point> references, double fitness)
        {
            References = references;
            Fitness = fitness;
        }

        public static Individual CloneIndividual(Individual template)
        {
            List<Point> points = new List<Point>();

            foreach (Point reference in template.References)
            {
                Point newReference = new Point(reference.X, reference.Y);
                points.Add(newReference);
            }

            return new Individual(points, template.Fitness);
        }
    }
}
