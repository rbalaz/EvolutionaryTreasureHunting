using GoldMiners.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldMiners.Evolution_algorithm_blocks
{
    class Evaluation
    {
        public void EvaluateIndividual(Individual individual)
        {
            Decoder decoder = new Decoder();

            List<Point> outerPoints = new List<Point>();
            List<Point> innerPoints = new List<Point>();
            foreach (Point reference in individual.References)
            {
                outerPoints.AddRange(decoder.DecodeReference(reference));
                innerPoints.AddRange(GetInnerPoints(reference));
            }

            List<Tuple<Point, int>> outerInnerMap = new List<Tuple<Point, int>>();
            foreach (Point point in innerPoints)
            {
                int index = GetItemIndex(outerInnerMap, point);
                if (index > -1)
                {
                    outerInnerMap[index] = new Tuple<Point, int>(outerInnerMap[index].Item1, outerInnerMap[index].Item2 + 1);
                }
                else
                {
                    outerInnerMap.Add(new Tuple<Point, int>(point, 0));
                }
            }
            foreach (Point point in outerPoints)
            {
                int index = GetItemIndex(outerInnerMap, point);
                if (index > -1)
                {
                    outerInnerMap[index] = new Tuple<Point, int>(outerInnerMap[index].Item1, outerInnerMap[index].Item2 + 1);
                }
            }

            List<Tuple<Point, int>> outerMap = new List<Tuple<Point, int>>();
            foreach (Point point in outerPoints)
            {
                int index = GetItemIndex(outerMap, point);
                if (index > -1)
                {
                    outerMap[index] = new Tuple<Point, int>(outerMap[index].Item1, outerMap[index].Item2 + 1);
                }
                else
                {
                    outerMap.Add(new Tuple<Point, int>(point, 0));
                }
            }

            double fitness = 0;
            foreach (Tuple<Point, int> item in outerInnerMap)
            {
                if (item.Item2 > 0)
                {
                    fitness += Math.Pow(item.Item2 + 1, 2);
                }
            }

            foreach (Tuple<Point, int> item in outerMap)
            {
                if (item.Item2 > 0)
                {
                    fitness += item.Item2;
                }
            }

            individual.Fitness = fitness + fitness * (1 / CalculateMinorityFitness(individual));
            //individual.Fitness = fitness;
        }

        private int GetItemIndex(List<Tuple<Point, int>> map, Point reference)
        {
            foreach (Tuple<Point, int> item in map)
            {
                if (item.Item1.X == reference.X && item.Item1.Y == reference.Y)
                {
                    return map.IndexOf(item);
                }
            }

            return -1;
        }

        private List<Point> GetInnerPoints(Point reference)
        {
            List<Point> innerPoints = new List<Point>();

            for (int i = 1; i < 8; i++)
            {
                Point current = new Point(reference.X + i, reference.Y - 1);
                innerPoints.Add(current);
            }

            for (int i = 2; i > -5; i--)
            {
                if (i == -1)
                    continue;
                Point current = new Point(reference.X + 4, reference.Y + i);
                innerPoints.Add(current);
            }

            return innerPoints;
        }

        private double CalculateMinorityFitness(Individual individual)
        {
            double totalDistance = 0.0;

            for (int i = 0; i < individual.References.Count; i++)
            {
                for (int j = 0; j < individual.References.Count; j++)
                {
                    totalDistance += Point.PointDistance2D(individual.References[i], individual.References[j]);
                }
            }

            return totalDistance;
        }
    }
}
