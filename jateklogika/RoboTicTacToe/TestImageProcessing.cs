using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboTicTacToe
{
    /// <summary>
    /// Test class to check the intermodule communication
    /// </summary>
    class TestImageProcessing : IImageProcessing
    {
        public event EventHandler<CameraStatusChangedEventArgs> CameraStatusChanged;

        public event EventHandler<TableStateChangedEventArgs> TableStateChanged;

        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

        /// <summary>
        /// Must be called when the image processing wants to show a post messsage
        /// </summary>
        public void OnPostMessageShowRequest(string message)
        {
            if(PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message));
        }

        /// <summary>
        /// Must be called when the status of the camera has changed
        /// </summary>
        public void OnCameraStatusChanged(CameraStatus newStatus)
        {
            if(CameraStatusChanged != null)
                CameraStatusChanged(this, new CameraStatusChangedEventArgs(newStatus));
        }

        /// <summary>
        /// Must be called when the table state has changed
        /// </summary>
        public void OnTableStateChanged(Piece[,] newTable)
        {
            if(TableStateChanged != null)
                TableStateChanged(this, new TableStateChangedEventArgs(newTable));
        }
    }
}
