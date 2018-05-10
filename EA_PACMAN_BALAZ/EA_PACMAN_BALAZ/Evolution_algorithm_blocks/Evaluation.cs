using Pacman.Representation;
using System;
using System.Collections.Generic;

namespace Pacman.Evolution_algorithm_blocks
{
    public class Evaluation
    {
        private bool firstCriteria;
        private bool secondCriteria;
        private bool thirdCriteria;

        public Evaluation(bool firstCriteria, bool secondCriteria, bool thirdCriteria)
        {
            this.firstCriteria = firstCriteria;
            this.secondCriteria = secondCriteria;
            this.thirdCriteria = thirdCriteria;
        }

        public Evaluation()
        { }

        public void EvaluateIndividual(Individual individual)
        {
            Decoder decoder = new Decoder();
            Path path = decoder.DecodeIndividual(individual);

            List<int> fitnessVector = new List<int>();

            // Has treasure seeker survived?
            if (firstCriteria)
            {
                bool hasSurvived = SimulatePath(individual, path);
                if (hasSurvived)
                    fitnessVector.Add(1);
                else
                    fitnessVector.Add(0);
            }
            // How much treasure was recovered
            if (secondCriteria)
            {
                fitnessVector.Add(CountCollectedTreasure(path));
            }
            // How safe is the path
            if (thirdCriteria)
            {
                fitnessVector.Add(CalculateSafety(path, individual));
            }

            individual.FitnessVector = fitnessVector;
        }

        private bool SimulatePath(Individual individual, Path path)
        {
            Point firstBeast = new Point(13, 15);
            Point secondBeast = new Point(25, 15);
            Point thirdBeast = new Point(37, 15);
            for (int i = 0; i < individual.Movements.Count; i++)
            {
                if (i % 2 == 1)
                {
                    // First beast behaviour
                    if (i < 11)
                    {
                        if (firstBeast.Y < path.Points[i + 1].Y)
                            firstBeast.Y++;
                        if (firstBeast.Y > path.Points[i + 1].Y)
                            firstBeast.Y--;
                    }
                    if (i == 11)
                    {
                        if (Math.Abs(path.Points[i + 1].Y - firstBeast.Y) <= 1)
                            return false;
                    }
                    // Second beast behaviour
                    if (i < 23)
                    {
                        if (secondBeast.Y < path.Points[i + 1].Y)
                            secondBeast.Y++;
                        if (secondBeast.Y > path.Points[i + 1].Y)
                            secondBeast.Y--;
                    }
                    if (i == 23)
                    {
                        if (Math.Abs(path.Points[i + 1].Y - secondBeast.Y) <= 1)
                            return false;
                    }
                    // Third beast behaviour
                    if (i < 35)
                    {
                        if (thirdBeast.Y < path.Points[i + 1].Y)
                            thirdBeast.Y++;
                        if (thirdBeast.Y > path.Points[i + 1].Y)
                            thirdBeast.Y--;
                    }
                    if (i == 35)
                    {
                        if (Math.Abs(path.Points[i + 1].Y - thirdBeast.Y) <= 1)
                            return false;
                    }
                }
            }

            return true;
        }

        private int CountCollectedTreasure(Path path)
        {
            int treasure = 0;
            foreach (Point point in path.Points)
            {
                if (point.Y < 15)
                    treasure += 14 - (15 - point.Y);
                else if (point.Y > 16)
                    treasure += 14 - (point.Y - 16);
                else
                    treasure += 14;
            }

            return treasure;
        }

        private int CalculateSafety(Path path, Individual individual)
        {
            int safety = 0;

            Point firstBeast = new Point(13, 15);
            Point secondBeast = new Point(25, 15);
            Point thirdBeast = new Point(37, 15);
            for (int i = 0; i < individual.Movements.Count; i++)
            {
                if (i % 2 == 1)
                {
                    // First beast behaviour
                    if (i < 11)
                    {
                        if (firstBeast.Y < path.Points[i + 1].Y)
                            firstBeast.Y++;
                        if (firstBeast.Y > path.Points[i + 1].Y)
                            firstBeast.Y--;
                    }
                    if (i == 11)
                    {
                        safety += Math.Abs(path.Points[i + 1].Y - firstBeast.Y);
                    }
                    // Second beast behaviour
                    if (i < 23)
                    {
                        if (secondBeast.Y < path.Points[i + 1].Y)
                            secondBeast.Y++;
                        if (secondBeast.Y > path.Points[i + 1].Y)
                            secondBeast.Y--;
                    }
                    if (i == 23)
                    {
                        safety += Math.Abs(path.Points[i + 1].Y - secondBeast.Y);
                    }
                    // Third beast behaviour
                    if (i < 35)
                    {
                        if (thirdBeast.Y < path.Points[i + 1].Y)
                            thirdBeast.Y++;
                        if (thirdBeast.Y > path.Points[i + 1].Y)
                            thirdBeast.Y--;
                    }
                    if (i == 35)
                    {
                        safety += Math.Abs(path.Points[i + 1].Y - thirdBeast.Y);
                    }
                }
            }

            return safety;
        }

        public List<Individual> EvaluateIndividuals(List<Individual> population)
        {
            List<Individual> clonedPopulation = CloneList(population);
            List<Individual> evaluatedPopulation = new List<Individual>();
            // Evaluate synthetic fitness based on domination
            int fitnessCounter = 1;
            while (clonedPopulation.Count > 0)
            {
                List<Individual> ParetSet = FindParetSet(clonedPopulation);
                if (ParetSet.Count == 0)
                {
                    foreach (Individual individual in clonedPopulation)
                    {
                        individual.Fitness = fitnessCounter;
                    }
                    evaluatedPopulation.AddRange(clonedPopulation);
                    clonedPopulation.Clear();
                }
                else
                {
                    foreach (Individual individual in ParetSet)
                    {
                        individual.Fitness = fitnessCounter;
                        clonedPopulation.Remove(individual);
                    }
                    evaluatedPopulation.AddRange(ParetSet);
                }
                fitnessCounter++;
            }

            return evaluatedPopulation;
        }

        private List<Individual> CloneList(List<Individual> template)
        {
            List<Individual> clonedList = new List<Individual>();

            foreach (Individual individual in template)
            {
                clonedList.Add(Individual.CloneIndividual(individual));
            }

            return clonedList;
        }

        public List<Individual> FindParetSet(List<Individual> population)
        {
            List<Individual> ParetSet = new List<Individual>();

            foreach (Individual individual in population)
            {
                Individual dominator = population.Find(ind => ind.Dominates(individual));
                if (dominator == null)
                    ParetSet.Add(individual);
            }

            return ParetSet;
        }
    }
}
