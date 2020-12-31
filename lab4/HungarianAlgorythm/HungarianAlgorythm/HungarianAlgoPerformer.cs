using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungarianAlgorythm
{
    public class HungarianAlgoPerformer
    {
        public CostMatrix CostMatrix { get; set; }

        public HungarianAlgoPerformer(CostMatrix costMatrix)
        {
            CostMatrix = costMatrix;
        }

        public void Execute()
        {
            // substract minimum of the row
            for (int i = 0; i < CostMatrix.NumberOfRows; i++)
                CostMatrix.SubstractFromRow(i, CostMatrix.GetMinOfTheRow(i));

            // substract minimum of the coll
            for (int i = 0; i < CostMatrix.NumberOfColls; i++)
                CostMatrix.SubstractFromColl(i, CostMatrix.GetMinOfTheColl(i));
            
            CostMatrix.CoverZeroes();

            while (CostMatrix.NumberOfLines() != Math.Max(CostMatrix.NumberOfRows, CostMatrix.NumberOfColls))
            {
                CostMatrix.SubstractSmallestUncovered();
                CostMatrix.RestoreMarkedHighlighted();                
                CostMatrix.CoverZeroes();
            }
        }
    }
}
