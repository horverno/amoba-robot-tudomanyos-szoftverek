using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterfaceModule;

namespace KinematicsModule
{
    public class Kinematics : IKinematics
    {
        
        public RobotStatus status {
            get
            {
                return status;
            }
            private set
            {
                status = value;
                OnRobotStatusChanged(status);
            }
        }

        public event EventHandler<RobotStatusChangedEventArgs> RobotStatusChanged;

        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

        public Kinematics()
        {
        }

        private void testRobotStatusChangedEvent(RobotStatus newStatus)
        {
            status = newStatus;
        }

        public void OnPostMessageShowRequest(string message)
        {
            if (PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message));
        }


        public void OnRobotStatusChanged(RobotStatus newStatus)
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new RobotStatusChangedEventArgs(newStatus));
        }

        public void SignUpToRobotMovementRequestEvent(IGameLogics gameLogics)
        {
            gameLogics.RobotMovementReqest += RobotMovementRequestHandler;
        }

        private void RobotMovementRequestHandler(object sender, RobotMovementRequestEventArgs e)
        {
            // do something to start the movement
            status = RobotStatus.Moving;
            OnPostMessageShowRequest("movement (" + e.ToString() + ") started...");
            // sleep (estimated moving time())
            //status = RobotStatus.Ready;
            OnPostMessageShowRequest("movement stopped");
        }
    }
}
