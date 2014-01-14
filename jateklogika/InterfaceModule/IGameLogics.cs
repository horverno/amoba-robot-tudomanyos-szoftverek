using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceModule
{
    /// <summary>
    /// This interface describes the communication with the GameLogic module for the other modules.
    /// </summary>
    public interface IGameLogics : IGame
    {
        /// <summary>
        /// With this event the game logics can call for a movement (from the kinematics)
        /// </summary>
        event EventHandler<RobotMovementRequestEventArgs> RobotMovementReqest;

    }

    /// <summary>
    /// This class represents a movement request  the robot (requested by the game logics).
    /// </summary>
    public class RobotMovementRequestEventArgs : EventArgs
    {
        public RobotMovement movementType { get; set; }
        public Piece pieceType { get; set; }
        public int destCol { get; set; }
        public int destRow { get; set; }

        /// <summary>
        /// Constructor for making a new movement request.
        /// </summary>
        /// <param name="movementType">the kind of the movement</param>
        /// <param name="pieceType">The type of the piece must be placed (only needed when placing a piece onto the table)</param>
        /// <param name="destCol">the destination column of the movement (only needed when placing a piece onto the table)</param>
        /// <param name="destRow">the destination row of the movement (only needed when placing a piece onto the table)</param>
        public RobotMovementRequestEventArgs(RobotMovement movementType, Piece pieceType, int destCol, int destRow) : base()
        {
            this.pieceType = pieceType;
            this.movementType = movementType;
            this.destCol = destCol;
            this.destRow = destRow;
        }
        
        /// <returns>Returns the string representation of the movement request</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder("Type of movement: ").Append(movementType);
            result.Append("\nPiece type: ").Append(pieceType);
            result.Append("\nDestination: (column ").Append(destCol).Append(", row ").Append(destRow).Append(")");
            return result.ToString();
        }
    }
}
