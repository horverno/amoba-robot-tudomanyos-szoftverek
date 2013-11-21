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
    class TestKinematics : IKinematics
    {
        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;
        public event EventHandler<RobotStatusChangedEventArgs> RobotStatusChanged;

        public void OnPostMessageShowRequest(string message)
        {
            if(PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message));
        }

        public void OnRobotStatusChanged(RobotStatus newStatus)
        {
            if(RobotStatusChanged != null)
                RobotStatusChanged(this, new RobotStatusChangedEventArgs(newStatus));
        }

        public void SignUpToRobotMovementRequestEvent(IGameLogics gameLogics)
        {
            gameLogics.RobotMovementReqest += RobotMovementRequestHandler;
        }

        private void RobotMovementRequestHandler(object sender, RobotMovementRequestEventArgs e)
        {
            MessageBox.Show("arguments:\n" + e.ToString(), "sender: " + sender.ToString());
        }

    }
}
