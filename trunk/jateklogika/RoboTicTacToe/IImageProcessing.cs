using System;
using System.Text;

namespace RoboTicTacToe
{
    /// <summary>
    /// This is the enumeration of hte possible camera statuses.
    /// </summary>
    public enum CameraStatus { Ok, Offline, TooMuchLight, NotEnoughLight, ViewBlocked, WrongTablePosition };
    
    /// <summary>
    /// This interface represents the communication between the image processing and the game logic modules.
    /// </summary>
    interface IImageProcessing : IGame
    {
        /// <summary>
        /// The event indicating tha camera state changes.
        /// </summary>
        event EventHandler<CameraStatusChangedEventArgs> CameraStatusChanged;
        /// <summary>
        /// This event should occur whenever something changed on the table.
        /// </summary>
        event EventHandler<TableStateChangedEventArgs> TableStateChanged;
    }
    
    /// <summary>
    /// This class contains the information about the new status of the camera.
    /// </summary>
    class CameraStatusChangedEventArgs : EventArgs
    {       
       public CameraStatus CurrentStatus { get; set;}

        public CameraStatusChangedEventArgs(CameraStatus newCameraStatus) : base()
        {
            this.CurrentStatus = newCameraStatus;
        }

        public override string ToString()
        {
            return "New status: " + CurrentStatus;
        }
    }
    
    /// <summary>
    /// This class represents the new set-up of the changed table.
    /// </summary>
    class TableStateChangedEventArgs : EventArgs
    {
        public Piece[,] table;
        
        public TableStateChangedEventArgs(Piece[,] newTable) : base()
        {
            this.table = newTable;
        }

        /// <returns>Returns the string representation of the table state</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder("New table set-up:\n");
            int cols = table.GetLength(0);
            int rows = table.GetLength(1);
            for (int x = 0; x < cols; ++x)
            {
                for (int y = 0; y < rows; ++y)
                {
                    result.Append(table[x, y]).Append("\t");
                }
                result.Append("\n");
            }
            return result.ToString();
        }
    }
}
