using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterfaceModule;

namespace GameLogicsModule
{
    public class GameLogics : IGameLogics
    {
        // event definitions:
        public event EventHandler<RobotMovementRequestEventArgs> RobotMovementReqest;
        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;
        // game table size:
        private int colCount { private get; private set; }
        private int rowCount { private get; private set; }
        // componenet state indicators:
        private RobotStatus robotStatus;
        private GameStatus cameraStatus;
        /// <summary>the type of the next piece in the dispenser</summary>
        private Piece nextPiece;
        /// <summary>the game table</summary>
        private TicTacToeTable table;
        /// <summary>the object for calculating the next move</summary>
        GepiJatekos nextMoveCalculator;
        /// <summary>the piece type of the AI player</summary>
        Piece pieceOfTheRobot;

        /// <summary>Constructs a new AI player</summary>
        /// <param name="pieceOfTheRobot">the piece type of the player (to play with)</param>
        public GameLogics(Piece pieceOfTheRobot)
        {
            nextMoveCalculator = new GepiJatekos();
            this.pieceOfTheRobot = pieceOfTheRobot;
        }

        /// <param name="colCount">number of the columns</param>
        /// <param name="rowCount">number of the rows</param>
        /// <param name="pieceOfTheRobot">piece type to play with</param>
        private void newGame(int colCount, int rowCount, Piece pieceOfTheRobot)
        {
            this.colCount = colCount;
            this.rowCount = rowCount;
            table = new TicTacToeTable(colCount, rowCount, Piece._Empty);
            this.pieceOfTheRobot = pieceOfTheRobot;
        }

        /// <summary>This method analyses the given table and gives the next move.</summary>
        /// <param name="table">current table state</param>
        /// <param name="findForX"> whether to find a move for X (or for O)</param>
        /// <param name="resultColIndex">the column index of the target field</param>
        /// <param name="resultRowIndex">the row index of the target field</param>
        private void FindNextMove(Piece[,] table, bool findForX, out int resultColIndex, out int resultRowIndex)
        {
            OnPostMessageShowRequest("Calculating the next step...");
            if (this.table.lastPiece == Piece._Empty)   // if the table is empty: place in the middle
            {
                resultColIndex = colCount / 2;
                resultRowIndex = rowCount / 2;
                return;
            }
            int[] result = new int[2];
            result = nextMoveCalculator.nextStepGen(convertTable(table, findForX), colCount+1, rowCount+1);
            resultColIndex = result[0];
            resultRowIndex = result[1];
        }

        /// <summary>
        /// Verifies that is it safe to start the robot arm and if not, it informs the user about the reason.
        /// </summary>
        /// <returns>Returns true if it is allowed to move the arm.</returns>
        private bool CheckIfIsMoveAllowed()
        {
            switch (robotStatus)
            {
                case RobotStatus.Ready:
                    break;
                case RobotStatus.Moving:
                    OnPostMessageShowRequest("Parancsütközés: A robotkar már mozog!", true);
                    return false;
                case RobotStatus.Offline:
                    OnPostMessageShowRequest("A robotkar nem elérhető!", true);
                    return false;
                case RobotStatus.ServoError:
                    OnPostMessageShowRequest("Motor hiba!", true);
                    return false;
                case RobotStatus.OutOfPieces:
                    OnPostMessageShowRequest("Kifogyott a bábu adagoló!", true);
                    return false;
                default:
                    OnPostMessageShowRequest("Ismeretlen robotkar állapot!", true);
                    return false;
            }
            switch (cameraStatus)
            {
                case GameStatus.Offline:
                    OnPostMessageShowRequest("A kamera nem elérhető!", true);
                    return false;
                case GameStatus.Online:
                    break;
                case GameStatus.SearchBoard:
                case GameStatus.BoardDetected_3x3:
                case GameStatus.BoardDetected_4x4:
                    return false;
                default:
                    OnPostMessageShowRequest("Ismeretlen kamera állapot!", true);
                    return false;
            }
            if (nextPiece != pieceOfTheRobot)
            {
                OnPostMessageShowRequest("Nem megfelelő bábu van az adagolóban!", true);
            }
            OnPostMessageShowRequest("", true); // deletes the previous error message
            return true;
        }

        /// <summary>
        /// Tests if it is the turn of the robot and it is clear to  move and if yes, it calculates and starts it.
        /// Must be called whenever an event occurs, that may makes necessary the robot's action.
        /// </summary>
        private void TryToMove()
        {
            try
            {
                int destCol, destRow;
                if (CheckIfIsMoveAllowed())
                {
                    FindNextMove(table.getTable(), pieceOfTheRobot == Piece.X, out destCol, out destRow);
                    if (CheckIfIsMoveAllowed()) // because it may changed
                    {
                        OnRobotMovementRequiest(RobotMovement.PlacePiece, pieceOfTheRobot, destCol, destRow);
                    }
                }
            }
            catch (Exception ex)
            {
                OnPostMessageShowRequest(ex.Message, true);
            }
        }
        
        public void OnPostMessageShowRequest(string message, bool important)
        {
            if (PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message, important));
        }

        public void OnPostMessageShowRequest(string message)
        {
            OnPostMessageShowRequest(message, false);
        }

        public void OnRobotMovementRequiest(RobotMovement movement, Piece piece, int destCol, int destRow)
        {
            if (RobotMovementReqest != null)
                RobotMovementReqest(this, new RobotMovementRequestEventArgs(movement, piece, destCol, destRow));
        }

        public void RobotStatusChangedHandler(object sender, RobotStatusChangedEventArgs e)
        {
            robotStatus = e.CurrentStatus;
            OnPostMessageShowRequest("Robot status changed:\n" + e.ToString());
            TryToMove();
        }

        public void CameraStatusChangedHandler(object sender, GameStatusChangedEventArgs e)
        {
            cameraStatus = e.CurrentStatus;
            OnPostMessageShowRequest("Camera status changed:\n" + e.ToString());
            TryToMove();
            if (cameraStatus == GameStatus.BoardDetected_4x4)
            {
                newGame(4, 4, Piece.O);
            }
        }

        public void TableSetupChangedHandler(object sender, TableStateChangedEventArgs e)
        {
            OnPostMessageShowRequest("Table set-up changed");
            try
            {
                if (table.UpdateTable(e.table) != pieceOfTheRobot)
                {
                    TryToMove();
                }
            }
            catch (Exception ex)
            {
                OnPostMessageShowRequest(ex.Message, true); // notifying the user so he/she can fix the error
            }
            // First test of the project:
            //if (IsMoveAllowed())
            //{
            //    int[] result = new int[2];
            //    OnPostMessageShowRequest("Calculating the next step...");
            //    result = nextMoveCalculator.nextStepGen(convertTable(e.table), colCount+1, rowCount+1);
            //    OnPostMessageShowRequest("Next step: " + result[0].ToString() + " - " + result[1].ToString());
            //    OnRobotMovementRequiest(RobotMovement.PlacePiece, Piece.O, result[0], result[1]); 
            //}
        }

        public void NextPieceChangedHandler(object sender, NextPieceChangedEventArgs e)
        {
            nextPiece = e.NextPieceStatus;
            OnPostMessageShowRequest("Pickup field status has changed:\n" + e.ToString());
            TryToMove();
        }
        /// <summary>Provides compatibility between the nextStepGen() method and the rest of the program.</summary>
        /// <param name="source">game table to convert</param>
        /// <param name="swapPieceTypes">needed because the nextStepGen() method can only calculate for "O"</param>
        /// <returns>Returns the converted table.</returns>
        private int[,] convertTable(Piece[,] source, bool swapPieceTypes)
        {
            if (source == null) throw new Exception("Hiányzó játéktér!");
            int cols = source.GetLength(0);
            int rows = source.GetLength(1);
            if (cols <= 0 || rows <= 0 || cols > colCount || rows > rowCount) throw new Exception("Hibás pályaméret!");
            int[,] result = new int[cols,rows];
            for (int i = 0; i < cols; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    switch (source [i,j])
                    {
                        case Piece.O:
                            result[colCount - 1 - j, rowCount - 1 - i] = (swapPieceTypes ? 1 : 0);
                            break;
                        case Piece.X:
                            result[colCount - 1 - j, rowCount - 1 - i] = (swapPieceTypes ? 0 : 1);
                            break;
                        case Piece._Empty:
                            result[colCount - 1 - j, rowCount - 1 - i] = 2;
                            break;
                        /*case Piece._OutOfField:
                            break;
                        case Piece._MoreThan1:
                            break;
                        case Piece._NextPieceMissing:
                            break;
                        case Piece._NextPieceOk:
                            break;*/
                        default:
                            throw new Exception("Mezőhiba [" + i + ", " + j + "]: " + source[i, j].ToString());
                    }
                }
            }
            return result;
        }

        public override string toString()
        {
            return pieceOfTheRobot.ToString() + " játékos";
        }
    }
}
