using HungarianAlgorythm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {               
            int[,] matrix = new int[,] { { 1500, 4000, 4500}, { 2000, 6000, 3500 }, { 2000, 4000, 2500 } };
            CostMatrix costMatrix = new CostMatrix(matrix);
            costMatrix.Display();
            
            HungarianAlgoPerformer performer = new HungarianAlgoPerformer(costMatrix);
            performer.Execute();
            costMatrix.Display();
            Console.WriteLine($"Result: {costMatrix.ResultSum}");
            int[] optAssignments = performer.CostMatrix.OptimalAssignments();

            foreach (int item in optAssignments)
            {
                Console.Write($"{item}, ");
            }
        }
    }
}
// alternative start data
//int[,] matrix = new int[,] { { 2500, 4000, 3500}, { 4000, 6000, 3500}, { 2000, 4000, 2500} };