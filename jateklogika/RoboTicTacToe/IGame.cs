using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboTicTacToe
{
    /// <summary>
    /// Enumeration to define piece types.
    /// </summary>
    public enum Piece { X, O };

    /// <summary>
    /// The "main" interface for the communication. Everything shoud go here, that all modules must to know.
    /// </summary>
    interface IGame
    {
    }
}
