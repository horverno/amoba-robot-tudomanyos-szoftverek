using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RoboTicTacToe
{
    /// <summary>
    /// Test class to check the intermodule communication
    /// </summary>
    class TestGameLogics : IGameLogics
    {
        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

        public event EventHandler<RobotMovementRequestEventArgs> RobotMovementReqest;

        TestWindow parent;

        public TestGameLogics(IKinematics kinematics, IImageProcessing imageProcessing, TestWindow parent)
        {
            this.parent = parent;   // needed for post message test
            // setting up the communication between the modules:
            PostMessageShowRequest += PostMessageHandler;  // game logics handles its own post messages
            kinematics.PostMessageShowRequest += PostMessageHandler;   // game logics handles the post messages of the kinematics
            imageProcessing.PostMessageShowRequest += PostMessageHandler;  // game logics handles the post messages of the image processing
            kinematics.RobotStatusChanged += RobotStatusChangedHandler; // game logics handles the changes of the robot arm status (source: kinematics)
            imageProcessing.CameraStatusChanged += CameraStatusChangedHandler;  // game logics handles the changes of the camera status (source: image processing)
            imageProcessing.TableStateChanged += TableSetupChangedHandler;  // game logics handles the changes of the table set-up (source: image processing)
            kinematics.SignUpToRobotMovementRequestEvent(this);  // kinematics handles the robot movement requests (source: game logics)
        }

        public void OnPostMessageShowRequest(string message)
        {
            if(PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message));
        }

        public void OnRobotMovementRequiest(RobotMovement movement, Piece piece, int destCol, int destRow)
        {
            if(RobotMovementReqest != null)
                RobotMovementReqest(this, new RobotMovementRequestEventArgs(movement, piece, destCol, destRow));
        }

        private void PostMessageHandler(object sender, PostMessageEventArgs e)
        {
            parent.WritePostMessage(sender.ToString() + ": " + e.ToString());
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
    }
}
