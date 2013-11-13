using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboTicTacToe
{
    /// <summary>
    /// Enumeration to define piece types.
    /// </summary>
    public enum Piece { _Empty, _Unknown, X, O };

    /// <summary>
    /// The "main" interface for the communication. Everything shoud go here, that all modules must to know.
    /// </summary>
    interface IGame
    {
        /// <summary>
        /// This event can be used by all modules to send post messages to the gui (so the user can see working process).
        /// Raise:  PostMessage(this, new PostMessageEventArgs("message"))
        /// Handle: EventHandler<PostMessageEventArgs> messageHandler = PostMessage;
        /// </summary>
        event EventHandler<PostMessageEventArgs> PostMessage;

    }

    class PostMessageEventArgs : EventArgs
    {
        public string message{ get; set; }

        public PostMessageEventArgs (string message) : base()
	    {
            this.message = message;
	    }
    }
}
