﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceModule
{
    /// <summary>
    /// Enumeration to define piece types.
    /// </summary>
    public enum Piece { _Empty, _Unknown_OutOfField, _MoreThan1, _NextPieceMissing, _NextPieceOk, X, O };

    /// <summary>
    /// The "main" interface for the communication. Everything shoud go here, that all modules must to know.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// This event can be used by all modules to send post messages to the gui (so the user can follow the process).
        /// </summary>
        /// <remarks>asdf</remarks>
        event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

    }
    
    /// <summary>
    /// This class describes an object holding a post message
    /// </summary>
    public class PostMessageEventArgs : EventArgs
    {
        public string message{ get; set; }

        public PostMessageEventArgs (string message) : base()
	    {
            this.message = message;
	    }

        /// <returns>Returns the message as a string</returns>
        public override string ToString()
        {
            return "Post message: " + message;
        }
    }
}
