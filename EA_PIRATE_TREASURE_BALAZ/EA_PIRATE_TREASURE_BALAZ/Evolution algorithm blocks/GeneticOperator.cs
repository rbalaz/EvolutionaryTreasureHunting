using PirateTreasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class GeneticOperator
    {
        private int descendantsPopulationCount;
        private List<Representation> parents;
        private Random random;


        public GeneticOperator(int descendantsPopulationCount, List<Representation> parents)
        {
            this.descendantsPopulationCount = descendantsPopulationCount;
            this.parents = parents;
            random = new Random();
        }

        public GeneticOperator()
        {
            random = new Random();
        }

        public List<Representation> GenerateDescendants()
        {
            // We need as many descendants as we have parents
            // Parent count will be an even number
            // parents will be processed by sexual operator in pairs as they are ordered in the parents list
            List<Representation> descendants = new List<Representation>(descendantsPopulationCount);

            for (int i = 0; i < parents.Count; i = i + 2)
            {
                // Step 1: Both parents will undergo mutation separately
                Representation mutatedMother = MutateMember(parents[i]);
                Representation mutatedFather = MutateMember(parents[i + 1]);

                // Step 2: Attempt crossover for mutated parents
                List<Representation> children = CrossOverMembers(mutatedMother, mutatedFather);
                descendants.AddRange(children);
            }

            return descendants;
        }

        public Representation MutateMember(Representation parent)
        {
            // Step 1a: Choose which position will be mutated
            // Step 1b: Choose which mutation effect will be used
            // One of three possible mutation effects may occur:
            // a) prolong segment by 1
            // b) shorten segment by 1
            // c) turn segment around

            // Clone original parent building blocks
            List<int> mutatedValues = new List<int>();
            foreach (int value in parent.Values)
                mutatedValues.Add(value);

            // Choosing segment to mutate
            int segmentPosition = random.Next(0, parent.Values.Count);
            // Choosing mutation effect
            int mutationEffect = random.Next(0, 3);

            // Prolongs chosen segment by 1 and shortens nearest parallel segment by 1
            if (mutationEffect == 0)
            {
                bool isPositive = mutatedValues[segmentPosition] > 0;
                mutatedValues[segmentPosition]++;
                if (mutatedValues[segmentPosition] == 0)
                {
                    if (segmentPosition != 0 && segmentPosition != mutatedValues.Count - 1)
                    {
                        mutatedValues[segmentPosition - 1] += mutatedValues[segmentPosition + 1];
                        mutatedValues.RemoveRange(segmentPosition, 2);
                    }
                    else
                        mutatedValues.RemoveAt(segmentPosition);
                }
                int affectedSegmentIndex = FindNearestParalellSegment(mutatedValues, segmentPosition, isPositive);
                // Operator may not be applicable for some candidates
                if (affectedSegmentIndex < 0)
                    return parent;
                mutatedValues[affectedSegmentIndex]--;
                // If we delete a segment by shortening it
                // Adjacent segments need to be merged
                if (mutatedValues[affectedSegmentIndex] == 0)
                {
                    if (affectedSegmentIndex != 0 && affectedSegmentIndex != mutatedValues.Count - 1)
                    {
                        mutatedValues[affectedSegmentIndex - 1] += mutatedValues[affectedSegmentIndex + 1];
                        mutatedValues.RemoveRange(affectedSegmentIndex, 2);
                    }
                    else
                        mutatedValues.RemoveAt(affectedSegmentIndex);
                }
            }
            // Shortens chosen segment by 1 and prolongs nearest parallel segment by 1
            if (mutationEffect == 1)
            {
                bool isPositive = mutatedValues[segmentPosition] > 0;
                mutatedValues[segmentPosition]--;
                if (mutatedValues[segmentPosition] == 0)
                {
                    if (segmentPosition != 0 && segmentPosition != mutatedValues.Count - 1)
                    {
                        mutatedValues[segmentPosition - 1] += mutatedValues[segmentPosition + 1];
                        mutatedValues.RemoveRange(segmentPosition, 2);
                    }
                    else
                        mutatedValues.RemoveAt(segmentPosition);
                }
                int affectedSegmentIndex = FindNearestParalellSegment(mutatedValues, segmentPosition, isPositive);
                // Operator may not be applicable for some candidates
                if (affectedSegmentIndex < 0)
                    return parent;
                mutatedValues[affectedSegmentIndex]++;
                if (mutatedValues[affectedSegmentIndex] == 0)
                {
                    if (affectedSegmentIndex != 0 && affectedSegmentIndex != mutatedValues.Count - 1)
                    {
                        mutatedValues[affectedSegmentIndex - 1] += mutatedValues[affectedSegmentIndex + 1];
                        mutatedValues.RemoveRange(affectedSegmentIndex, 2);
                    }
                    else
                        mutatedValues.RemoveAt(affectedSegmentIndex);
                }
            }
            // Changes orientation of chosen segment
            if (mutationEffect == 2)
            {
                if (mutatedValues[segmentPosition] < -29)
                {
                    int difference = mutatedValues[segmentPosition] + 29;
                    mutatedValues[segmentPosition] -= difference;
                    mutatedValues[segmentPosition] *= -1;
                    mutatedValues.Insert(segmentPosition + 1, difference - mutatedValues[segmentPosition]);
                }
                else
                {
                    mutatedValues[segmentPosition] *= -1;
                    mutatedValues.Insert(segmentPosition + 1, mutatedValues[segmentPosition] * -1);
                }
                int mutatedValue = mutatedValues[segmentPosition];
                // Shorten existing nearest parallel segments by the value turned segment had
                int toShorten = Math.Abs(mutatedValues[segmentPosition]);
                while (toShorten > 0)
                {
                    int indexToShorten = FindNearestParalellSegment(mutatedValues, segmentPosition);
                    if (Math.Abs(mutatedValues[indexToShorten]) > toShorten)
                    {
                        mutatedValues[indexToShorten] =
                            (mutatedValues[indexToShorten]) / Math.Abs(mutatedValues[indexToShorten]) *
                            (Math.Abs(mutatedValues[indexToShorten]) - toShorten);
                        toShorten = 0;
                    }
                    else
                    {
                        toShorten -= Math.Abs(mutatedValues[indexToShorten]);
                        if (indexToShorten != 0 && indexToShorten != mutatedValues.Count && Math.Abs(indexToShorten - segmentPosition) != 1)
                        {
                            mutatedValues[indexToShorten - 1] += mutatedValues[indexToShorten + 1];
                            mutatedValues.RemoveRange(indexToShorten, 2);
                            if (indexToShorten < segmentPosition)
                                segmentPosition -= 2;
                        }
                        else
                        {
                            mutatedValues.RemoveAt(indexToShorten);
                            if (segmentPosition > indexToShorten)
                                segmentPosition--;
                        }
                    }
                }
                if (segmentPosition > 0)
                {
                    if (mutatedValues[segmentPosition] > 0 && mutatedValues[segmentPosition - 1] > 0)
                    {
                        mutatedValues[segmentPosition] += mutatedValues[segmentPosition - 1];
                        mutatedValues.RemoveAt(segmentPosition - 1);
                    }
                    if (mutatedValues[segmentPosition] < 0 && mutatedValues[segmentPosition - 1] < 0)
                    {
                        mutatedValues[segmentPosition] += mutatedValues[segmentPosition - 1];
                        mutatedValues.RemoveAt(segmentPosition - 1);
                    }
                }
            }

            return new Representation(mutatedValues);
        }

        private int FindNearestParalellSegment(List<int> mutatedValues, int segmentPosition)
        {
            bool isPositive = mutatedValues[segmentPosition] > 0;

            List<int> parallelSegmentsIndices = new List<int>();
            for (int i = 0; i < mutatedValues.Count; i++)
            {
                if (mutatedValues[i] > 0 == isPositive)
                    parallelSegmentsIndices.Add(i);
            }

            parallelSegmentsIndices.Remove(segmentPosition);

            int minDistance = mutatedValues.Count;
            int minDistanceIndex = -1;
            foreach (int index in parallelSegmentsIndices)
            {
                int currentDistance = Math.Abs(index - segmentPosition);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    minDistanceIndex = index;
                }
            }

            return minDistanceIndex;
        }

        private int FindNearestParalellSegment(List<int> mutatedValues, int segmentPosition, bool isPositive)
        {
            List<int> parallelSegmentsIndices = new List<int>();
            for (int i = 0; i < mutatedValues.Count; i++)
            {
                if (mutatedValues[i] > 0 == isPositive)
                    parallelSegmentsIndices.Add(i);
            }

            parallelSegmentsIndices.Remove(segmentPosition);

            int minDistance = mutatedValues.Count;
            int minDistanceIndex = -1;
            foreach (int index in parallelSegmentsIndices)
            {
                int currentDistance = Math.Abs(index - segmentPosition);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    minDistanceIndex = index;
                }
            }

            return minDistanceIndex;
        }

        public List<Representation> CrossOverMembers(Representation mother, Representation father)
        {
            // Step 1: Detect 2 consecutive intersections
            // Step 2: Swap segments between 2 consecutive intersection points

            Decoder decoder = new Decoder();
            Path motherCandidate = decoder.DecodeRepresentation(mother);
            Path fatherCandidate = decoder.DecodeRepresentation(father);

            // Iterate over mother candidate until a point is found which is not
            // the start or end point and father contains it
            // If no such point is found, paths are disjunct and cross over operator
            // is not applicable.
            int swappingFatherStartIndex = -1, swappingFatherEndIndex = -1;
            int swappingMotherStartIndex = -1, swappingMotherEndIndex = -1;
            for(int i = 1; i < motherCandidate.Points.Count; i++)
            {
                int fatherIndex = fatherCandidate.Points.FindIndex(
                    item => item.X == motherCandidate.Points[i].X && item.Y == motherCandidate.Points[i].Y);
                if (fatherIndex != -1)
                {
                    if (swappingFatherStartIndex == -1)
                    {
                        swappingFatherStartIndex = fatherIndex;
                        swappingMotherStartIndex = i;
                    }
                    else
                    {
                        swappingFatherEndIndex = fatherIndex;
                        swappingMotherEndIndex = i;
                        // Swapping segments are only valid if their boundary points
                        // have both coordinates different
                        Point start = fatherCandidate.Points[swappingFatherStartIndex];
                        Point end = fatherCandidate.Points[swappingFatherEndIndex];
                        if (start.X == end.X || start.Y == end.Y)
                        {
                            swappingFatherStartIndex = swappingFatherEndIndex;
                            swappingMotherStartIndex = swappingMotherEndIndex;
                            swappingFatherEndIndex = -1;
                            swappingMotherEndIndex = -1;
                        }
                        else
                            break;
                    }
                }
            }

            // Construct descedants if operator can be applied
            if (swappingFatherEndIndex > 0 && swappingFatherStartIndex > 0 &&
                swappingMotherEndIndex > 0 && swappingMotherStartIndex > 0)
            {
                List<Point> daughter = new List<Point>();
                List<Point> son = new List<Point>();

                // Add preliminary path parts before the swapping segment
                daughter.AddRange(motherCandidate.Points.Take(swappingMotherStartIndex));
                son.AddRange(fatherCandidate.Points.Take(swappingFatherStartIndex));
                // Add swapped segment
                daughter.AddRange(fatherCandidate.Points.Skip(swappingFatherStartIndex).Take(swappingFatherEndIndex - swappingFatherStartIndex));
                son.AddRange(motherCandidate.Points.Skip(swappingMotherStartIndex).Take(swappingMotherEndIndex - swappingMotherStartIndex));
                // Add tail parts of path
                daughter.AddRange(motherCandidate.Points.Skip(swappingMotherEndIndex));
                son.AddRange(fatherCandidate.Points.Skip(swappingFatherEndIndex));

                List<Representation> descendants = new List<Representation>(2);
                descendants.Add(decoder.EncodePath(new Path(daughter)));
                descendants.Add(decoder.EncodePath(new Path(son)));

                return descendants;
            }
            // If operator cannot be applied, mutated parents are returned unchanged
            return new List<Representation> { mother, father };
        }
    }
}
