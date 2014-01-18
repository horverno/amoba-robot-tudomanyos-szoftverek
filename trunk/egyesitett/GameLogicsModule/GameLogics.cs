using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterfaceModule;

namespace GameLogicsModule
{
    public class GameLogics : IGameLogics
    {
        public event EventHandler<RobotMovementRequestEventArgs> RobotMovementReqest;
        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

        const int colCount = 4;
        const int rowCount = 4;

        private RobotStatus robotStatus;
        private GameStatus cameraStatus;
        private Piece nextPiece;

        private TicTacToeTable table;

        GepiJatekos nextStepCalculator;

        public GameLogics()
        {
            nextStepCalculator = new GepiJatekos();
            table = new TicTacToeTable(colCount, rowCount, Piece._Empty);
        }

        public void OnRobotMovementRequiest(RobotMovement movement, Piece piece, int destCol, int destRow)
        {
            if (RobotMovementReqest != null)
                RobotMovementReqest(this, new RobotMovementRequestEventArgs(movement, piece, destCol, destRow));
        }

        public void RobotStatusChangedHandler(object sender, RobotStatusChangedEventArgs e)
        {
            robotStatus = e.CurrentStatus;
            Console.WriteLine("Robot status changed:\n" + e.ToString(), "sender: " + sender.ToString());
        }

        public void CameraStatusChangedHandler(object sender, GameStatusChangedEventArgs e)
        {
            cameraStatus = e.CurrentStatus;
            Console.WriteLine("Camera status changed:\n" + e.ToString(), "sender: " + sender.ToString());
        }

        public void TableSetupChangedHandler(object sender, TableStateChangedEventArgs e)
        {
            if (robotStatus == RobotStatus.Ready && cameraStatus == GameStatus.Online && nextPiece == Piece.O)
            {
                Console.WriteLine("Table set-up changed:\n" + e.ToString(), "sender: " + sender.ToString());
                int[] result = new int[2];
                result = nextStepCalculator.nextStepGen(convertTable(e.table), colCount+1, rowCount+1);
                Console.WriteLine("next step: " + result[0].ToString() + " - " + result[1].ToString());
                OnRobotMovementRequiest(RobotMovement.PlacePiece, Piece.O, result[0], result[1]); 
            }
        }

        public void NextPieceChangedHandler(object sender, NextPieceChangedEventArgs e)
        {
            nextPiece = e.NextPieceStatus;
            Console.WriteLine("Pickup field status has changed:\n" + e.ToString(), "sender: " + sender.ToString());
        }

        private static int[,] convertTable(Piece[,] source)
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
                            result[colCount -1 - j, rowCount - 1 - i] = 0;
                            break;
                        case Piece.X:
                            result[colCount - 1 - j, rowCount - 1 - i] = 1;
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
    }
}
