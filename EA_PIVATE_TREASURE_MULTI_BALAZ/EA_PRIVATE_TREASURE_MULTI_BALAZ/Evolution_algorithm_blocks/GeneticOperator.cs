using Medallion;
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
                if (StaticOperations.ValidateRepresentation(mutatedMother) == false)
                    throw new NotSupportedException();
                Representation mutatedFather = MutateMember(parents[i + 1]);
                if (StaticOperations.ValidateRepresentation(mutatedFather) == false)
                    throw new NotSupportedException();

                // Step 2: Attempt crossover for mutated parents
                List<Representation> children = CrossOverMembers(mutatedMother, mutatedFather);
                foreach (Representation representation in children)
                {
                    if (StaticOperations.ValidateRepresentation(representation) == false)
                        throw new NotSupportedException();
                }
                descendants.AddRange(children);
            }

            return descendants;
        }

        public Representation MutateMember(Representation parent)
        {
            // Step 1a: Choose which position will be mutated
            // Step 1b: Choose which mutation effect will be used
            
            // Clone original parent building blocks
            List<int> mutatedValues = new List<int>();
            foreach (int value in parent.Values)
                mutatedValues.Add(value);

            // Choosing segment to mutate
            int segmentPosition = random.Next(0, parent.Values.Count);
            // Choosing mutation effect
            int mutationEffect = random.Next(0, 8);

            // Prolong operator
            if (mutationEffect == 0)
            {
                ProlongOperator(mutatedValues, segmentPosition);
            }
            // Shorten operator
            if (mutationEffect == 1)
            {
                ShortenOperator(mutatedValues, segmentPosition);
            }
            // Insert break
            if (mutationEffect == 2)
            {
                BreakOperator(mutatedValues, segmentPosition);
            }
            // Insert operator of segment tuple (x,-y) or (-x,y) 
            if (mutationEffect == 3)
            {
                TupleOperator(mutatedValues, segmentPosition);
            }
            // Changes orientation of chosen segment
            if (mutationEffect == 4)
            {
                SwitchOperator(mutatedValues, segmentPosition);
            }
            // Regroup operator
            if (mutationEffect == 5)
            {
                RegroupOperator(mutatedValues);
            }
            // Inverse operator
            if (mutationEffect == 6)
            {
                mutatedValues.Reverse();
            }

            return new Representation(mutatedValues);
        }

        #region Mutation operators
        private List<int> ProlongOperator(List<int> mutatedValues, int segmentPosition)
        {
            int prolongLength = (int)Math.Ceiling(Math.Abs(SimpleRNG.GetNormal())) + 1;
            int sign = mutatedValues[segmentPosition] / Math.Abs(mutatedValues[segmentPosition]);

            // Prolonging operator may not be applicable in full length 
            // If segment extends throughout the whole field, operator is not applicable at all
            if (sign < 0)
            {
                if (mutatedValues[segmentPosition] + sign * prolongLength < -39)
                {
                    prolongLength = Math.Abs(-39 - mutatedValues[segmentPosition]);
                }
            }
            else
            {
                if (mutatedValues[segmentPosition] + sign * prolongLength > 29)
                {
                    prolongLength = 29 - mutatedValues[segmentPosition];
                }
            }

            if (prolongLength == 0)
                return mutatedValues;

            mutatedValues[segmentPosition] = sign * (Math.Abs(mutatedValues[segmentPosition]) + prolongLength);
            int toShorten = prolongLength;
            while (toShorten > 0)
            {
                int randomParallelSegmentIndex = FindRandomParalellSegment(mutatedValues, segmentPosition);
                if (Math.Abs(mutatedValues[randomParallelSegmentIndex]) > toShorten)
                {
                    mutatedValues[randomParallelSegmentIndex] =
                        sign * (Math.Abs(mutatedValues[randomParallelSegmentIndex]) - toShorten);
                    toShorten = 0;
                }
                else
                {
                    toShorten -= Math.Abs(mutatedValues[randomParallelSegmentIndex]);
                    if (randomParallelSegmentIndex > 0 & randomParallelSegmentIndex < mutatedValues.Count - 1)
                    {
                        mutatedValues[randomParallelSegmentIndex - 1] += mutatedValues[randomParallelSegmentIndex + 1];
                        mutatedValues.RemoveRange(randomParallelSegmentIndex, 2);

                        if (segmentPosition > randomParallelSegmentIndex)
                        {
                            segmentPosition -= 2;
                        }
                    }
                    else
                    {
                        mutatedValues.RemoveAt(randomParallelSegmentIndex);
                        if (randomParallelSegmentIndex == 0)
                            segmentPosition -= 1;
                    }
                }
            }

            return mutatedValues;
        }

        private List<int> ShortenOperator(List<int> mutatedValues, int segmentPosition)
        {
            int prolongLength = (int)Math.Ceiling(Math.Abs(SimpleRNG.GetNormal())) + 1;
            int sign = mutatedValues[segmentPosition] / Math.Abs(mutatedValues[segmentPosition]);

            // Operator can be applied with length less or equal to difference between 
            // maximum length of segment and longest segment
            if (sign < 0)
            {
                int minimum = mutatedValues.Min();
                if (prolongLength > Math.Abs(-39 - minimum))
                {
                    prolongLength = Math.Abs(-39 - minimum);
                }
            }
            else
            {
                int maximum = mutatedValues.Max();
                if (prolongLength > 29 - maximum)
                {
                    prolongLength = 29 - maximum;
                }
            }
            if (prolongLength == 0)
                return mutatedValues;

            // Maximum allowed value for shortening operator is the length of the segment
            if (prolongLength > Math.Abs(mutatedValues[segmentPosition]))
                prolongLength = Math.Abs(mutatedValues[segmentPosition]);

            mutatedValues[segmentPosition] = sign * (Math.Abs(mutatedValues[segmentPosition]) - prolongLength);
            if (mutatedValues[segmentPosition] == 0)
            {
                if (segmentPosition == 0 || segmentPosition == mutatedValues.Count - 1)
                {
                    mutatedValues.RemoveAt(segmentPosition);
                    segmentPosition = -1;
                }
                else
                {
                    mutatedValues[segmentPosition - 1] += mutatedValues[segmentPosition + 1];
                    mutatedValues.RemoveRange(segmentPosition, 2);
                    segmentPosition = -1;
                }
            }

            int randomParallelSegmentIndex = FindRandomParalellSegment(mutatedValues, segmentPosition, sign > 0);
            mutatedValues[randomParallelSegmentIndex] = sign * (Math.Abs(mutatedValues[randomParallelSegmentIndex]) + prolongLength);

            return mutatedValues;
        }

        private List<int> BreakOperator(List<int> mutatedValues, int segmentPosition)
        {
            // 1.) Choose random position in the selected segment
            int selectedPosition = random.Next(1, Math.Abs(mutatedValues[segmentPosition]));
            // 2.) Check if break operator can be applied
            int upMovements = 0;
            int rightMovements = 0;
            for (int i = 0; i < segmentPosition; i++)
            {
                if (mutatedValues[i] < 0)
                    rightMovements -= mutatedValues[i];
                else
                    upMovements += mutatedValues[i];
            }
            // Operator can only be applied if segments don't have either X or Y coordinate at maximum possible value
            if (upMovements == 29 || rightMovements == 39)
                return mutatedValues;
            // Operator cannot be applied to segments of length 1
            if (Math.Abs(mutatedValues[segmentPosition]) == 1)
                return mutatedValues;

            // 3.) Generate length of breaking segment using normal distribution
            int breakLength = (int)Math.Abs(Math.Ceiling(SimpleRNG.GetNormal()));
            if (breakLength == 0)
                return mutatedValues;
            // Shorten break length to maximum allowed value if needed
            if (mutatedValues[segmentPosition] < 0)
            {
                if (upMovements + breakLength > 30)
                    breakLength = 30 - upMovements;
            }
            else
            {
                if (rightMovements + breakLength > 40)
                    breakLength = 40 - rightMovements;
            }

            // 4.) Mutate parent
            int currentChangedSegmentPosition = segmentPosition + 1;
            int sign = mutatedValues[segmentPosition] / (Math.Abs(mutatedValues[segmentPosition]));
            // a) Shorten segment that is going to be broken
            int otherHalfOfBrokenSegment = sign * (Math.Abs(mutatedValues[segmentPosition]) - selectedPosition);
            mutatedValues[segmentPosition] = sign * selectedPosition;
            // b) Add perpendicular breaking segment
            mutatedValues.Insert(segmentPosition + 1, -1 * sign * breakLength);
            // c) Add remaining part of shortened segment to the path
            mutatedValues.Insert(segmentPosition + 2, otherHalfOfBrokenSegment);
            // d) Shorten next following segment parallel with the break segment
            int toShorten = breakLength;
            while (toShorten > 0)
            {
                int randomParallelSegmentIndex = FindRandomParalellSegment(mutatedValues, currentChangedSegmentPosition);
                if (Math.Abs(mutatedValues[randomParallelSegmentIndex]) > toShorten)
                {
                    mutatedValues[randomParallelSegmentIndex] =
                        sign * -1 * (Math.Abs(mutatedValues[randomParallelSegmentIndex]) - toShorten);
                    toShorten = 0;
                }
                else
                {
                    toShorten -= Math.Abs(mutatedValues[randomParallelSegmentIndex]);
                    if (randomParallelSegmentIndex > 0 & randomParallelSegmentIndex < mutatedValues.Count - 1)
                    {
                        mutatedValues[randomParallelSegmentIndex - 1] += mutatedValues[randomParallelSegmentIndex + 1];
                        mutatedValues.RemoveRange(randomParallelSegmentIndex, 2);

                        if (currentChangedSegmentPosition > randomParallelSegmentIndex)
                        {
                            currentChangedSegmentPosition -= 2;
                        }
                    }
                    else
                    {
                        mutatedValues.RemoveAt(randomParallelSegmentIndex);
                        if (randomParallelSegmentIndex == 0)
                            currentChangedSegmentPosition -= 1;
                    }
                }
            }

            return mutatedValues;
        }

        private List<int> TupleOperator(List<int> mutatedValues, int segmentPosition)
        {
            // This operator serves the purpose which the other operators lack 
            // It adds 2 more segments into existing values
            // This operator can always be applied regardless of segment status and 
            // any segment position can be subject to it

            int firstSegmentLength = 1;
            int secondSegmentLength = 1;

            int sign = mutatedValues[segmentPosition] / Math.Abs(mutatedValues[segmentPosition]);

            List<int> newTuple = new List<int> { sign * -1 * firstSegmentLength, sign * secondSegmentLength };
            // Tuple is placed after selected position
            mutatedValues.InsertRange(segmentPosition + 1, newTuple);

            int currentChangedSegmentPosition = segmentPosition + 1;
            int toShorten = firstSegmentLength;
            while (toShorten > 0)
            {
                int randomParallelSegmentIndex = FindRandomParalellSegment(mutatedValues, currentChangedSegmentPosition);
                if (Math.Abs(mutatedValues[randomParallelSegmentIndex]) > toShorten)
                {
                    mutatedValues[randomParallelSegmentIndex] =
                        sign * -1 * (Math.Abs(mutatedValues[randomParallelSegmentIndex]) - toShorten);
                    toShorten = 0;
                }
                else
                {
                    toShorten -= Math.Abs(mutatedValues[randomParallelSegmentIndex]);
                    if (randomParallelSegmentIndex > 0 & randomParallelSegmentIndex < mutatedValues.Count - 1)
                    {
                        mutatedValues[randomParallelSegmentIndex - 1] += mutatedValues[randomParallelSegmentIndex + 1];
                        mutatedValues.RemoveRange(randomParallelSegmentIndex, 2);

                        if (currentChangedSegmentPosition > randomParallelSegmentIndex)
                        {
                            currentChangedSegmentPosition -= 2;
                        }
                    }
                    else
                    {
                        mutatedValues.RemoveAt(randomParallelSegmentIndex);
                        if (randomParallelSegmentIndex == 0)
                            currentChangedSegmentPosition -= 1;
                    }
                }
            }

            currentChangedSegmentPosition++;
            toShorten = secondSegmentLength;
            while (toShorten > 0)
            {
                int randomParallelSegmentIndex = FindRandomParalellSegment(mutatedValues, currentChangedSegmentPosition);
                if (Math.Abs(mutatedValues[randomParallelSegmentIndex]) > toShorten)
                {
                    mutatedValues[randomParallelSegmentIndex] =
                        sign * (Math.Abs(mutatedValues[randomParallelSegmentIndex]) - toShorten);
                    toShorten = 0;
                }
                else
                {
                    toShorten -= Math.Abs(mutatedValues[randomParallelSegmentIndex]);
                    if (randomParallelSegmentIndex > 0 & randomParallelSegmentIndex < mutatedValues.Count - 1)
                    {
                        mutatedValues[randomParallelSegmentIndex - 1] += mutatedValues[randomParallelSegmentIndex + 1];
                        mutatedValues.RemoveRange(randomParallelSegmentIndex, 2);

                        if (currentChangedSegmentPosition > randomParallelSegmentIndex)
                        {
                            currentChangedSegmentPosition -= 2;
                        }
                    }
                    else
                    {
                        mutatedValues.RemoveAt(randomParallelSegmentIndex);
                        if (randomParallelSegmentIndex == 0)
                            currentChangedSegmentPosition -= 1;
                    }
                }
            }

            return mutatedValues;
        }

        private List<int> RegroupOperator(List<int> mutatedValues)
        {
            List<int> positiveValues = mutatedValues.Where(item => item > 0).ToList();
            List<int> negativeValues = mutatedValues.Where(item => item < 0).ToList();

            // Shuffle lists
            positiveValues.Shuffle();
            negativeValues.Shuffle();

            // Pick starting segment direction
            int sign;
            if (positiveValues.Count == negativeValues.Count)
                sign = random.Next(0, 2);
            else
                sign = positiveValues.Count > negativeValues.Count ? 1 : 0;

            // Reconstruct member
            int memberLength = mutatedValues.Count;
            mutatedValues.Clear();
            if (sign == 1)
            {
                mutatedValues.Add(positiveValues[0]);
                positiveValues.RemoveAt(0);
            }
            else
            {
                mutatedValues.Add(negativeValues[0]);
                negativeValues.RemoveAt(0);
            }
            for (int i = 1; i < memberLength; i++)
            {
                if (mutatedValues[i - 1] > 0)
                {
                    mutatedValues.Add(negativeValues[0]);
                    negativeValues.RemoveAt(0);
                }
                else
                {
                    mutatedValues.Add(positiveValues[0]);
                    positiveValues.RemoveAt(0);
                }
            }

            return mutatedValues;
        }

        private List<int> SwitchOperator(List<int> mutatedValues, int segmentPosition)
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
                int indexToShorten = FindRandomParalellSegment(mutatedValues, segmentPosition);
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
                    if (indexToShorten != 0 && indexToShorten != mutatedValues.Count - 1 && Math.Abs(indexToShorten - segmentPosition) != 1)
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

            return mutatedValues;
        }
        #endregion

        #region Utilities
        private int FindRandomParalellSegment(List<int> mutatedValues, int segmentPosition)
        {
            bool isPositive = mutatedValues[segmentPosition] > 0;

            List<int> parallelSegmentsIndices = new List<int>();
            for (int i = 0; i < mutatedValues.Count; i++)
            {
                if (mutatedValues[i] > 0 == isPositive)
                    parallelSegmentsIndices.Add(i);
            }

            parallelSegmentsIndices.Remove(segmentPosition);

            parallelSegmentsIndices.Shuffle();

            if (parallelSegmentsIndices.Count > 0)
                return parallelSegmentsIndices[0];
            else
                return segmentPosition;
        }

        private int FindRandomParalellSegment(List<int> mutatedValues, int segmentPosition, bool isPositive)
        {
            List<int> parallelSegmentsIndices = new List<int>();
            for (int i = 0; i < mutatedValues.Count; i++)
            {
                if (mutatedValues[i] > 0 == isPositive)
                    parallelSegmentsIndices.Add(i);
            }

            parallelSegmentsIndices.Shuffle();

            return parallelSegmentsIndices[0];
        }
        #endregion

        #region Cross-over operators
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
        #endregion
    }
}