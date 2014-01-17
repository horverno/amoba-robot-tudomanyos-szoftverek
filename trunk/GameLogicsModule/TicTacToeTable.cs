using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterfaceModule;

namespace GameLogicsModule
{
    public class TicTacToeTable : Table<Piece>
    {
        /// <summary>
        /// This value is always 2 in the Tic Tac Toe game.
        /// </summary>
        public const int PLAYER_COUNT = 2;

        public int maxChainLength { get; private set; }

        public TicTacToeTable(int colCount, int rowCount, Piece fillWith) : base(colCount, rowCount, fillWith)
        {
            maxChainLength = colCount > rowCount ? colCount : rowCount;
            fillTableWith(fillWith);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Returns a result array [x, y]:
        /// x = 0 if X, 1 if O
        /// y = chain lenght - 1
        /// field = chain count
        /// </returns>
        public int[,] getHorizontalChains(){
            int[,] result = new int[PLAYER_COUNT, maxChainLength];
            for (int i = 0; i < colCount; i++)  // all rows
            {
                int xCount = 0;
                int oCount = 0;
                for (int j = 0; j < rowCount; j++)  // all fields of the row
                {
                    if (table[i, j] == Piece.X)
                    {
                        if (oCount > 0)
                        {
                            result[1, oCount - 1]++;
                            oCount = 0;
                        }
                        else xCount++;
                    }
                    else if (table[i, j] == Piece.O)
                    {
                        if (xCount > 0)
                        {
                            result[0, xCount]++;
                            xCount = 0;
                        }
                        else oCount++;
                    }
                    else xCount = oCount = 0;
                }
            }
            return result;
        }
    }
}
