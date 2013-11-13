using System;

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
        /// Raise:  CameraStatusChanged(this, new CameraStatusChangedEventArgs(CameraStatus.Ok))
        /// Handle: EventHandler<CameraStatusChangedEventAgrgs> cameraStatusHandler = CameraStatusChanged
        /// </summary>
        event EventHandler<CameraStatusChangedEventArgs> CameraStatusChanged;
        /// <summary>
        /// This event should occur whenever something changed on the table.
        /// Raise:  TableSetupChanged(this, new TableSetupChangedEventArgs(newTable))
        /// Handle: EventHandler<CameraStatusChangedEventAgrgs> tableSetupHandler = TableSetupChanged
        /// </summary>
        event EventHandler<TableSetupChangedEventArgs> TableSetupChanged;
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
    }
    
    /// <summary>
    /// This class represents the new set-up of the changed table.
    /// </summary>
    class TableSetupChangedEventArgs : EventArgs
    {
        public Piece[,] table;
        
        public TableSetupChangedEventArgs(Piece[,] newTable) : base()
        {
            this.table = newTable;
        }
    }
}
