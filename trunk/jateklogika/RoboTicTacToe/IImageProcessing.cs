using System;

namespace RoboTicTacToe
{
    public enum CameraStatus { Ok, Offline, TooMuchLight, NotEnoughLight, ViewBlocked };
    
    /// <summary>
    /// This interface represents the communication between the image processing and the game logic modules.
    /// </summary>
    interface IImageProcessing : IGame
    {
        event EventHandler CameraStatusChanged;
        event EventHandler NewGameStarted;
        event EventHandler NewMove;
    }

    class CameraStatusChangedEventArgs : EventArgs
    {
        
        private CameraStatus CurrentStatus { get; set;}

        public CameraStatusChangedEventArgs(CameraStatus newCameraStatus) : base()
        {
            this.CurrentStatus = newCameraStatus;
        }
    }

    class NewGameStartedEventArgs : EventArgs
    {
        private byte ColCount { get; set; }
        private byte RowCount { get; set; }

        public NewGameStartedEventArgs(byte colCount, byte rowCount) : base()
        {
            this.ColCount = colCount;
            this.RowCount = rowCount;
        }
    }

    class NewMoveEventArgs : EventArgs
    {
        private byte Col { get; set; }
        private byte Row { get; set; }
        private Piece Piece { get; set; }

        public NewMoveEventArgs(byte col, byte row, Piece type) : base()
        {
            this.Col = col;
            this.Row = row;
            this.Piece = type;
        }
    }
}
