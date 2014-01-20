using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceModule
{
    /// <summary>
    /// Enumeration to define piece types.
    /// </summary>
    public enum Piece { O, X, _Empty, _OutOfField, _MoreThan1, _NextPieceMissing, _NextPieceOk };

    /// <summary>
    /// The "main" interface for the communication. Everything shoud go here, that all modules must to know.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// This event can be used by all modules to send post messages to the gui (so the user can follow the process).
        /// </summary>
        event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

    }
    
    /// <summary>This class describes an object holding a post message.</summary>
    public class PostMessageEventArgs : EventArgs
    {
        public string message { get; set; }
        public bool important { get; set; }

        public PostMessageEventArgs (string message, bool important) : base()
	    {
            this.message = message;
            this.important = important;
	    }

        public PostMessageEventArgs (string message) : this(message, false){}

        /// <returns>Returns the message as a string.</returns>
        public override string ToString()
        {
            return (important?"IMPORTANT MESSAGE: ":"Post message: ") + message;
        }
    }
}
