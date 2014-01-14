using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InterfaceModule;

namespace GameLogicsModule
{
    public class GameLogics : IGameLogics
    {

        public event EventHandler<RobotMovementRequestEventArgs> RobotMovementReqest;

        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;
        
        TicTacToeTable<Piece> gameTable;
        
        public GameLogics(IKinematics kinematics, IImageProcessing imageProcessing) : this()
        {
            // setting up the communication between the modules:
            PostMessageShowRequest += PostMessageHandler;  // game logics handles its own post messages
            kinematics.PostMessageShowRequest += PostMessageHandler;   // game logics handles the post messages of the kinematics
            imageProcessing.PostMessageShowRequest += PostMessageHandler;  // game logics handles the post messages of the image processing
            kinematics.RobotStatusChanged += RobotStatusChangedHandler; // game logics handles the changes of the robot arm status (source: kinematics)
            imageProcessing.CameraStatusChanged += CameraStatusChangedHandler;  // game logics handles the changes of the camera status (source: image processing)
            imageProcessing.TableStateChanged += TableSetupChangedHandler;  // game logics handles the changes of the table set-up (source: image processing)
            kinematics.SignUpToRobotMovementRequestEvent(this);  // kinematics handles the robot movement requests (source: game logics)
        }

        public GameLogics()
        {
            gameTable = new TicTacToeTable<Piece>(4, 4, Piece._Empty);
            gameTable.setField(0, 0, Piece.X);
            gameTable.setField(0, 2, Piece.X);
            gameTable.setField(0, 3, Piece.X);
            gameTable.setField(1, 0, Piece.O);
            gameTable.setField(1, 0, Piece.O);
            gameTable.setField(2, 1, Piece.O);
            gameTable.setField(3, 2, Piece.O);
            OnPostMessageShowRequest("The game logics module has been started...");
        }

        public void OnPostMessageShowRequest(string message)
        {
            if (PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message));
        }

        public void OnRobotMovementRequiest(RobotMovement movement, Piece piece, int destCol, int destRow)
        {
            if (RobotMovementReqest != null)
                RobotMovementReqest(this, new RobotMovementRequestEventArgs(movement, piece, destCol, destRow));
        }

        private void PostMessageHandler(object sender, PostMessageEventArgs e)
        {
            MessageBox.Show(e.ToString(), sender.ToString());
        }

        private void RobotStatusChangedHandler(object sender, RobotStatusChangedEventArgs e)
        {
            MessageBox.Show("Robot status changed:\n" + e.ToString(), "sender: " + sender.ToString());
        }

        private void CameraStatusChangedHandler(object sender, CameraStatusChangedEventArgs e)
        {
            MessageBox.Show("Camera status changed:\n" + e.ToString(), "sender: " + sender.ToString());
        }

        private void TableSetupChangedHandler(object sender, TableStateChangedEventArgs e)
        {
            MessageBox.Show("Table set-up changed:\n" + e.ToString(), "sender: " + sender.ToString());
        }

        public int[,] TestHorizontal()
        {
            return gameTable.getHorizontalChains();
        }

        public static string TestResultToString(int[,] result)
        {
            StringBuilder sb = new StringBuilder("Result:\n");
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    sb.Append(result[i,j] + " ");
                }
                sb.Append("\n");
            }
            return sb.Append("\n").ToString();
        }
    
    }
}
