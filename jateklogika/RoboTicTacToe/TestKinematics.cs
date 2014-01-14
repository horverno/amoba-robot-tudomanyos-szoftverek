using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InterfaceModule;

namespace MainModule
{
    /// <summary>
    /// Test class to check the intermodule communication
    /// </summary>
    class TestKinematics : IKinematics
    {
        public RobotStatus status { get; private set; }
        
        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;
        public event EventHandler<RobotStatusChangedEventArgs> RobotStatusChanged;
        
        public TestKinematics()
        {
            setStatus(RobotStatus.Ready);
        }

        private void setStatus(RobotStatus newStatus) 
        {
            status = newStatus;
            OnRobotStatusChanged(status);
        }

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
            // do something to start the movement
            setStatus(RobotStatus.Moving);
            OnPostMessageShowRequest("movement (" + e.ToString() + ") started...");
            // sleep (estimated moving time())
            setStatus(RobotStatus.Ready);
            OnPostMessageShowRequest("movement stopped");
        }

    }
}
