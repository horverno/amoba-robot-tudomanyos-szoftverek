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

        public Piece lastPiece{ get; private set; }

        public TicTacToeTable(int colCount, int rowCount, Piece fillWith) : base(colCount, rowCount, fillWith)
        {
            maxChainLength = colCount > rowCount ? colCount : rowCount;
            lastPiece = Piece._Empty;
        }
        /// <summary>
        /// Registers the new move on the table if it is valid.
        /// If there are any conflicts between the last and the current table state, it throws and exception instead.
        /// </summary>
        /// <param name="newTableSetup">the changed state of the table</param>
        /// <returns>Returns the type of the new piece so the game logics can decide to move or not.</returns>
        public Piece UpdateTable(Piece[,] newTableSetup)
        {
            int newMoveColIndex = -1;
            int newMoveRowIndex = -1;
            for (int i = 0; i < colCount; ++i)
            {
                for (int j = 0; j < rowCount; ++j)
                {
                    if (newTableSetup[i, j] != table[i, j])
                    {
                        if (newMoveRowIndex != -1 || newMoveRowIndex != -1) throw new Exception("Hiba: Egynél több változás a táblán!");
                        newMoveColIndex = i;
                        newMoveRowIndex = j;
                    }
                    switch (newTableSetup[i,j])
                    {
                        case Piece.O:
                        case Piece.X:
                        case Piece._Empty:
                            break;
                        default:    // else: field error
                            throw new Exception("Mezőhiba [" + ++i + ". sor, " + ++j + ". oszlop]: " + newTableSetup[i, j].ToString());
                    }
                }
            }
            if (newTableSetup[newMoveColIndex, newMoveRowIndex]==lastPiece)
            {
                throw new Exception("Nem megengedett lépés a következő mezőn: [" +
                    ++newMoveColIndex + ".sor, " + ++newMoveRowIndex + ".oszlop] (a másik játékos jön)!");
            }
            setField(newMoveColIndex, newMoveRowIndex, newTableSetup[newMoveColIndex, newMoveRowIndex]);
            return newTableSetup[newMoveColIndex, newMoveRowIndex];
        }

        /// <summary>
        /// Experimanetal code to find a more efficient way to generate the next step. 
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
