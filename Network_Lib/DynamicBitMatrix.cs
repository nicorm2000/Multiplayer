using System.Collections;
using System.Collections.Generic;

namespace Net
{
    public class DynamicBitMatrix
    {
        private List<BitArray> matrix;
        private int columns;

        /// <summary>
        /// Initializes a new instance of the DynamicBitMatrix class with the specified number of columns.
        /// </summary>
        /// <param name="columns">The number of columns in the matrix.</param>
        public DynamicBitMatrix(int columns)
        {
            this.columns = columns;
            matrix = new List<BitArray>();
        }

        /// <summary>
        /// Gets the value of a specific bit in the matrix.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>True if the bit is set; otherwise, false.</returns>
        public bool Get(int row, int column)
        {
            if (row < matrix.Count)
            {
                return matrix[row][column];
            }
            return false;
        }

        /// <summary>
        /// Sets the value of a specific bit in the matrix.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="value">The value to set.</param>
        public void Set(int row, int column, bool value)
        {
            while (row >= matrix.Count)
            {
                matrix.Add(new BitArray(columns));
            }
            matrix[row][column] = value;
        }

        /// <summary>
        /// Clears all bits in a specific row.
        /// </summary>
        /// <param name="row">The row index to clear.</param>
        public void ClearRow(int row)
        {
            if (row < matrix.Count)
            {
                matrix[row].SetAll(false);
            }
        }

        public int Rows => matrix.Count;
        public int Columns => columns;
    }
}