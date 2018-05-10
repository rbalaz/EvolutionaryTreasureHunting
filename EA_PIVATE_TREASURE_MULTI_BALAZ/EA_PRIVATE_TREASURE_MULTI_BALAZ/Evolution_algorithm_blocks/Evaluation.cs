using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateTreasure.Utils;

namespace PirateTreasure.Evolution_algorithm_blocks
{
    class Evaluation
    {
        private int[][] fitnessMap;

        public Evaluation()
        {
            FileStream stream = new FileStream("Treasure_Fitness_Map.txt", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);

            List<int[]> fitnessMapList = new List<int[]>();
            string line;
            int lineCounter = -1;
            while ((line = reader.ReadLine()) != null)
            {
                lineCounter++;
                string[] lineSegments = line.Split(' ');
                List<string> lineList = lineSegments.ToList();
                lineList.RemoveAll(item => item == "");
                lineSegments = lineList.ToArray();
                int[] fitnessArray = new int[lineSegments.Length];
                for (int i = 0; i < lineSegments.Length; i++)
                {
                    fitnessArray[i] = int.Parse(lineSegments[i]);
                }
                fitnessMapList.Add(fitnessArray);
            }

            fitnessMapList.Reverse();
            fitnessMap = fitnessMapList.ToArray();
            stream.Close();
            reader.Close();
        }

        public void EvaluatePopulation(List<Utils.Path> population)
        {
            foreach (Utils.Path path in population)
            {
                path.Fitness = EvaluatePath(path);
            }
        }

        public int EvaluatePath(Utils.Path path)
        {
            int fitness = 0;
            for (int i = 0; i < path.Points.Count; i++)
            {
                fitness += EvaluatePoint(path.Points[i]);
            }

            return fitness;
        }

        private int EvaluatePoint(Point point)
        {
            return fitnessMap[point.Y - 1][point.X - 1];
        }
    }
}
