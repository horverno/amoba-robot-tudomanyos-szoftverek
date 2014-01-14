using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicsModule
{
    /// <summary>
    /// Experimental class to try an other method of storing a table.
    /// </summary>
    /// <typeparam name="T">The object type to be stored.</typeparam>
    class Table<T>
    {
        protected T[,] table;
        public int colCount { get; private set; }
        public int rowCount { get; private set; }

        public Table(int colCount, int rowCount, T fillWith)
        {
            this.colCount = colCount;
            this.rowCount = rowCount;
            table = new T[colCount, rowCount];
            fillTableWith(fillWith);
        }

        public void fillTableWith(T item)
        {
            for (int i = 0; i < colCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    table[i, j] = item;
                }
            }
        }

        public T getField(int col, int row)
        {
            return table[col, row];
        }

        public void setField(int col, int row, T item)
        {
            table[col, row] = item;
        }



    }
}
