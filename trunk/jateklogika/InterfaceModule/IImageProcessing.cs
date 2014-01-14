using System;
using System.Text;

namespace InterfaceModule
{
    /// <summary>
    /// This is the enumeration of hte possible camera statuses.
    /// </summary>
    public enum CameraStatus { Offline, Online, SearchBoard, BoardDetected_3x3, BoardDetected_4x4 };
    
    /// <summary>
    /// This interface represents the communication between the image processing and the game logic modules.
    /// </summary>
    public interface IImageProcessing : IGame
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
    public class CameraStatusChangedEventArgs : EventArgs
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
    /// This class contains the information about the next piece.
    /// </summary>
    public class NextPieceChangedEventArgs : EventArgs
    {
        public Piece NextPieceStatus { get; set; }

        public NextPieceChangedEventArgs(Piece NextPieceStatus)
            : base()
        {
            this.NextPieceStatus = NextPieceStatus;
        }

        public override string ToString()
        {
            return "Next Piece: " + NextPieceStatus;
        }
    }
    
    /// <summary>
    /// This class represents the new set-up of the changed table.
    /// </summary>
    public class TableStateChangedEventArgs : EventArgs
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
            if (table == null) 
            {
                return result.Append("- table is missing -\n").ToString();
            }
            int cols = table.GetLength(0);
            int rows = table.GetLength(1);
            for (int x = 0; x < cols; ++x)
            {
                for (int y = 0; y < rows; ++y)
                {
                    result.Append(String.Format("{0,-12}",table[x, y]));    // FIXIT: x and y are swapped?
                }
                result.Append("\n");
            }
            return result.ToString();
        }
    }
}
