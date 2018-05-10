﻿using PirateTreasure.Evolution_algorithm_blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PirateTreasure.Utils
{
    static class StaticOperations
    {
        public static double Point2DDistance(Point first, Point second)
        {
            return Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));
        }

        public static bool ValidatePath(Path path)
        {
            // Tests validity of path generated by genetic operators
            // Path must comply to a 3 simple rules:
            // 1. EVERY NEIGHBOURING POINTS MUST HAVE EXACTLY ONE DIFFERENT COORDINATE BY ONE POINT
            for (int i = 0; i < path.Points.Count - 1; i++)
            {
                double xDifference = path.Points[i + 1].X - path.Points[i].X;
                double yDifference = path.Points[i + 1].Y - path.Points[i].Y;

                if (xDifference < 0 || xDifference > 1)
                    return false;
                if (yDifference < 0 || yDifference > 1)
                    return false;
                if (xDifference == 1 && yDifference == 1)
                    return false;
                if (xDifference == 0 && yDifference == 0)
                    return false;
            }

            // 2. PATH NEEDS TO CONTAIN EXACTLY 69 POINTS
            if (path.Points.Count != 69)
                return false;

            // 3. PATH NEEDS TO START IN (1,1) AND END IN (40,30)
            if (path.Points[0].X != 1 || path.Points[0].Y != 1)
                return false;
            if (path.Points[68].X != 40 || path.Points[68].Y != 30)
                return false;

            return true;
        }

        public static bool ValidateRepresentation(Representation representation)
        {
            // Tests validity of representation
            // Representation must comply to these rules: 
            // 1. REPRESENTATION MUST ALWAYS HAVE SEGMENTS WITH DIFFERENT SIGNS SUCCEEDING EACH OTHER
            for (int i = 0; i < representation.Values.Count - 1; i++)
            {
                // Zero length segments are not allowed
                if (representation.Values[i] * representation.Values[i + 1] >= 0)
                    return false;
            }
            // 2. REPRESENTATION NEEDS TO HAVE NEGATIVE SEGMENTS SUM OF -39 AND POSITIVE SEGMENTS SUM OF 29
            int negativeSum = 0;
            int positiveSum = 0;
            for (int i = 0; i < representation.Values.Count; i++)
            {
                if (representation.Values[i] < 0)
                    negativeSum += representation.Values[i];
                else
                    positiveSum += representation.Values[i];
            }
            if (negativeSum != -39 || positiveSum != 29)
                return false;

            return true;
        }

        public static bool ValidatePath(List<int> points)
        {
            Representation representation = new Representation(points);
            Decoder decoder = new Decoder();
            return ValidatePath(decoder.DecodeRepresentation(representation));
        }

        public static int TotalDistanceBetweenPaths(Representation first, Representation second)
        {
            Decoder decoder = new Decoder();
            Path firstPath = decoder.DecodeRepresentation(first);
            Path secondPath = decoder.DecodeRepresentation(second);

            // Calculates Manhattan distance between all corresponding pairs of points
            int totalDistance = 0;
            for (int i = 0; i < firstPath.Points.Count; i++)
            {
                totalDistance += Point.ManhattanDistanceBetweenTwoPoints(firstPath.Points[i], secondPath.Points[i]);
            }

            return totalDistance;
        }

        public static List<Representation> CloneList(List<Representation> oldList)
        {
            List<Representation> newList = new List<Representation>();
            foreach (Representation member in oldList)
            {
                Representation newMember = new Representation(member.Fitness, member.Values);
                newList.Add(newMember);
            }

            return newList;
        }
    }
}
