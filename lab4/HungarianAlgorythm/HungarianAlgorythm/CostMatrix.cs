using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungarianAlgorythm
{
    public class CostMatrix
    {
        private class MatrixCell : IEquatable<MatrixCell>
        {
            public MatrixCell(int row, int coll)
            {
                Row = row;
                Coll = coll;
            }

            public int Row { get; }

            public int Coll { get; }

            public bool Equals(MatrixCell other)
            {
                return Row == other.Row && Coll == other.Coll;
            }

            public override string ToString()
            {
                return $"Row: {Row}, Coll: {Coll}.";
            }
        }

        public CostMatrix(int[,] matrix)
        {
            Matrix = matrix;
            MatrixBackup = new int[NumberOfRows, NumberOfColls];
            //matrix.CopyTo(MatrixBackup, 0);
            for (int i = 0; i < NumberOfRows; i++)
                for (int j = 0; j < NumberOfColls; j++)
                    MatrixBackup[i, j] = Matrix[i, j];
            MarkedColls = new bool[NumberOfColls];
            MarkedRows = new bool[NumberOfRows];
            HighlightedColls = new bool[NumberOfColls];
            HighlightedRows = new bool[NumberOfRows];
            AssignedCells = new List<MatrixCell>();
            CrossedCells = new List<MatrixCell>();
            NewlyMarkedColls = new List<int>();
            NewlyMarkedRows = new List<int>();
        }

        public int[,] Matrix { get; set; }

        private int[,] MatrixBackup { get; set; }

        public bool[] MarkedRows { get; } 
        
        public bool[] MarkedColls { get; }

        public bool[] HighlightedRows { get; }

        public bool[] HighlightedColls { get; }

        public int ResultSum { get; private set; }

        private List<MatrixCell> AssignedCells { get; }
        
        private List<MatrixCell> CrossedCells { get; }

        private List<int> NewlyMarkedRows { get; }

        private List<int> NewlyMarkedColls { get; }
        
        public int NumberOfRows
        {
            get 
            {
                return Matrix.GetLength(0);
            }
        }

        public int NumberOfColls
        {
            get
            {
                return Matrix.GetLength(1);
            }
        }

        public void SubstractFromRow(int rowNumber, int number)
        {
            ResultSum += number;
            int lengthOfRow = Matrix.GetLength(1);
            for (int i = 0; i < lengthOfRow; i++)
                Matrix[rowNumber, i] -= number;
        }

        public void SubstractFromColl(int collNumber, int number)
        {
            ResultSum += number;
            int lengthOfRow = Matrix.GetLength(0);
            for (int i = 0; i < lengthOfRow; i++)
                Matrix[i, collNumber] -= number;
        }

        public int GetMinOfTheRow(int rowNumber)
        {
            return Enumerable.Range(0, Matrix.GetLength(1))
                .Select(n => Matrix[rowNumber, n])
                .Min(x => x);
        }

        public int GetMinOfTheColl(int rowNumber)
        {
            return Enumerable.Range(0, Matrix.GetLength(0))
                .Select(n => Matrix[n, rowNumber])
                .Min(x => x);
        }

        public void Display()
        {
            Console.Write("".PadLeft(8));
            for (int i = 0; i < NumberOfColls; i++)
            {
                Console.Write((MarkedColls[i] ? "x" : "").PadLeft(8));
            }
            Console.WriteLine();
            for (int i = 0; i < NumberOfRows; i++)
            {
                Console.Write((MarkedRows[i] ? "x" : "").PadLeft(8));
                for (int j = 0; j < NumberOfColls; j++)
                {
                    if (HighlightedColls[j] || HighlightedRows[i])
                        Console.BackgroundColor = ConsoleColor.Green;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write($"{Matrix[i, j]}".PadLeft(8));
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
            
        }

        public void RestoreMarkedHighlighted()
        {
            for (int i = 0; i < NumberOfRows; i++)
            {
                HighlightedRows[i] = false;
                MarkedRows[i] = false;
            }
            for (int i = 0; i < NumberOfColls; i++)
            {
                HighlightedColls[i] = false;
                MarkedColls[i] = false;
            }
            AssignedCells.Clear();
            CrossedCells.Clear();
            NewlyMarkedColls.Clear();
            NewlyMarkedRows.Clear();
        }

        public void RestoreNewlyMarked()
        {
            NewlyMarkedColls.Clear();
            NewlyMarkedRows.Clear();
        }
        
        public void CoverZeroes()
        {
            DoAssignment();
            MarkUnassignedRows();

            while (true)
            {
                if (NewlyMarkedRows.Count() == 0)
                    break;
                MarkCollsHavingZeroesInNewRows();
                NewlyMarkedRows.Clear();

                if (NewlyMarkedColls.Count() == 0)
                    break;
                MarkRowsHavingAssignmentsInNewColls();
                NewlyMarkedColls.Clear();
            }

            Highlight();

            //internal functions

            void DoAssignment()
            {
                for(int rowNum = 0; rowNum < NumberOfRows; rowNum++)
                {
                    MatrixCell cell = FindZeroInARow(rowNum, 0);

                    if (cell != null)
                    {
                        AssignCell(cell);
                        // cross in a row
                        MatrixCell cellToCross = FindZeroInARow(rowNum, cell.Coll + 1);
                        while (cellToCross != null)
                        {
                            if (!IsCorssed(cellToCross))
                                CrossCell(cellToCross);

                            cellToCross = FindZeroInARow(rowNum, cellToCross.Coll + 1);
                        }

                        //Cross in a coll
                        cellToCross = FindZeroInAColl(cell.Coll, cell.Row + 1);
                        while (cellToCross != null)
                        {
                            if (!IsCorssed(cellToCross))
                                CrossCell(cellToCross);

                            cellToCross = FindZeroInAColl(cell.Coll, cellToCross.Row + 1);
                        }
                    }
                }

                void AssignCell(MatrixCell cell)
                {
                    AssignedCells.Add(cell);
                }

                void CrossCell(MatrixCell cell)
                {
                    CrossedCells.Add(cell);
                }

                bool IsCorssed(MatrixCell cell)
                {
                    return CrossedCells.Contains(cell); 
                }

                bool IsAssigned(MatrixCell cell)
                {
                    return AssignedCells.Contains(cell);
                }

                MatrixCell FindZeroInARow(int rowNum, int startingFrom)
                {
                    if (startingFrom >= NumberOfColls)
                        return null;
                    for (int i = startingFrom; i < NumberOfColls; i++)
                        if (Matrix[rowNum, i] == 0)
                        {
                            MatrixCell cell = new MatrixCell(rowNum, i);
                            if (!IsCorssed(cell))
                                return cell;
                        }
                    return null;
                }

                MatrixCell FindZeroInAColl(int collNum, int startingFrom)
                {
                    if (startingFrom >= NumberOfRows)
                        return null;
                    for (int i = startingFrom; i < NumberOfRows; i++)
                        if (Matrix[i, collNum] == 0) return new MatrixCell(i, collNum);
                    return null;
                }
            }

            void MarkUnassignedRows()
            {
                IEnumerable<int> rowsToMark = Enumerable.Range(0, NumberOfRows).Except(AssignedCells.Select(c => c.Row).Distinct());
                foreach (int rowNum in rowsToMark)
                    MarkRow(rowNum);
            }

            void MarkCollsHavingZeroesInNewRows()
            {
                foreach (int markedRow in NewlyMarkedRows)
                {
                    for (int i = 0; i < NumberOfColls; i++)
                    {
                        if (Matrix[markedRow, i] == 0)
                            MarkColl(i);    
                    }
                }                
            }

            void MarkRowsHavingAssignmentsInNewColls()
            {
                foreach (int markedColl in NewlyMarkedColls)
                {
                    IEnumerable RowsToMark = AssignedCells.Where(c => c.Coll == markedColl).Select(c => c.Row);
                    foreach (int row in RowsToMark)
                        MarkRow(row);
                }
            }

            void Highlight()
            {
                for(int i = 0; i < NumberOfRows; i++)
                {
                    if (!MarkedRows[i])
                        HighlightedRows[i] = true;
                }

                for (int i = 0; i < NumberOfColls; i++)
                {
                    if (MarkedColls[i])
                        HighlightedColls[i] = true;
                }
            }

            void MarkRow(int rowNum)
            {
                if (!MarkedRows[rowNum])
                {
                    MarkedRows[rowNum] = true;
                    NewlyMarkedRows.Add(rowNum);
                }
            }

            void MarkColl(int collNum)
            {
                if (!MarkedColls[collNum])
                {
                    MarkedColls[collNum] = true;
                    NewlyMarkedColls.Add(collNum);
                }
            }
        }

        public void SubstractSmallestUncovered()
        {
            int minVal = FindSmallestUncovered();

            // substract minval from nonhighlighted rows
            for(int i = 0; i < NumberOfRows; i++)
                if (!HighlightedRows[i])
                    SubstractFromRow(i, minVal);

            // add minVal to highlighted columns
            for (int j = 0; j < NumberOfColls; j++)
                if (HighlightedColls[j])
                    SubstractFromColl(j, minVal * -1);

            int FindSmallestUncovered()
            {
                int cmin = int.MaxValue;
                for(int i = 0; i < NumberOfRows; i++)
                {
                    for(int j = 0; j < NumberOfColls; j++)
                    {
                        if (!HighlightedRows[i] && !HighlightedColls[j] && Matrix[i, j] < cmin)
                            cmin = Matrix[i, j];
                    }
                }

                return cmin;
            }
        }

        public int NumberOfLines()
        {
            return HighlightedColls.Where(c => c).Count() + HighlightedRows.Where(c => c).Count();
        }

        public int[] OptimalAssignments()
        {
            List<MatrixCell> Cells = new List<MatrixCell>();
            for (int i = 0; i < NumberOfRows; i++)
                for (int j = 0; j < NumberOfColls; j++)
                    if (Matrix[i, j] == 0) Cells.Add(new MatrixCell(i, j));

            List<MatrixCell> chosenCells = new List<MatrixCell>();
            do
            {
                var keys = Cells.GroupBy(x => x.Row).Where(g => g.Count() == 1).Select(g => g.Key).ToList();
                var newCells = Cells.Where(c => keys.Contains(c.Row));
                foreach (var item in newCells)
                {
                    Cells = Cells.Where(c => c.Row != item.Row && c.Coll != item.Coll).ToList();
                }
                chosenCells.AddRange(newCells);
                Cells = Cells.Except(chosenCells).ToList();
            } while (chosenCells.Count() != Math.Max(NumberOfColls, NumberOfRows));

            int[] resultNumbers = new int[chosenCells.Count()];
            for (int i = 0; i < resultNumbers.Length; i++)
                resultNumbers[i] = MatrixBackup[chosenCells[i].Row, chosenCells[i].Coll];

            return resultNumbers;
        }
    }
}
