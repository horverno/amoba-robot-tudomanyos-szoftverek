using System;

namespace RoboTicTacToe
{
    /// <summary>
    /// This is the enumeration of all the movement types the robot arm can do.
    /// </summary>
    public enum RobotMovement { CleanUp, PlacePiece, Cheer, Grieve };
    /// <summary>
    /// This is the enumeration of the possible statuses of the robot arm.
    /// </summary>
    public enum RobotStatus { Ready, Moving, Offline, ServoError, OutOfPieces };

    /// <summary>
    /// This interface represents the communication between the kinematics and the game logic modules.
    /// </summary>
    interface IKinematics : IGame
    {
        /// <summary>
        /// This event must be "called" whenever the status of the robot has changed.
        /// </summary>
        event EventHandler<RobotStatusChangedEventArgs> RobotStatusChanged;

        /// <summary>
        /// This method is called by the game logics to establish the incoming communication.
        /// </summary>
        /// <param name="gameLogics">The reference of the caller</param>
        /// <example>gameLogics.RobotMovementReqest += RobotMovementRequestHandler;</example>
        void SignUpToRobotMovementRequestEvent(IGameLogics gameLogics);

    }

    /// <summary>
    /// An instance of this class contains the information about the new status of the robot.
    /// </summary>
    class RobotStatusChangedEventArgs : EventArgs
    {
        public RobotStatus CurrentStatus { get; set; }

        public RobotStatusChangedEventArgs(RobotStatus newStatus) : base()
        {
            this.CurrentStatus = newStatus;
        }

        public override string ToString()
        {
            return "New status: " + CurrentStatus;
        }
    }
}
