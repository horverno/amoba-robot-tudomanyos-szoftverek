using System;

namespace RoboTicTacToe
{
    public enum RobotMovement { Cheer, Grieve };
    public enum RobotStatus { Ready, Moving, Offline, ServoError, OutOfPieces };

    /// <summary>
    /// This interface represents the communication between the kinematics and the game logic modules.
    /// </summary>
    interface IKinematics : IGame
    {
        void NewGame(byte colCount, byte rowCount);
        void PlacePiece(Piece pieceType, byte destCol, byte destRow);
        void Move(RobotMovement movementType);

        event EventHandler RobotStatusChanged;
    }

    class RobotStatusChangedEventArgs
    {
        private RobotStatus CurrentStatus { get; set; }

        public RobotStatusChangedEventArgs(RobotStatus newStatus)
        {
            this.CurrentStatus = newStatus;
        }
    }
}
