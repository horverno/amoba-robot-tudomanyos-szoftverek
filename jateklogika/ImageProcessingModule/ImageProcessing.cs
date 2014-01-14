using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceModule;
using System.Windows.Forms;

namespace ImageProcessingModule
{
    /// <summary>
    /// This is a test class to simulate the image processing module.
    /// It will be replaced by the final module when it will be ready.
    /// </summary>
    public class ImageProcessing : IImageProcessing
    {

        public CameraStatus cameraStatus {
            get
            {
                return cameraStatus;
            }
            private set
            {
                cameraStatus = value;
                OnCameraStatusChanged(value);
            }
        }

        public event EventHandler<CameraStatusChangedEventArgs> CameraStatusChanged;

        public event EventHandler<TableStateChangedEventArgs> TableStateChanged;

        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

        /// <param name="camViewer">Given by the main form to be able to show the image processing live</param>
        public ImageProcessing(PictureBox camViewer)
        {
        }

        /// <summary>
        /// Must be called when the image processing wants to show a post messsage
        /// </summary>
        public void OnPostMessageShowRequest(string message)
        {
            if (PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message));
        }

        /// <summary>
        /// Must be called when the status of the camera has changed
        /// </summary>
        public void OnCameraStatusChanged(CameraStatus newStatus)
        {
            if (CameraStatusChanged != null)
                CameraStatusChanged(this, new CameraStatusChangedEventArgs(newStatus));
        }

        /// <summary>
        /// Must be called when the table state has changed
        /// </summary>
        public void OnTableStateChanged(Piece[,] newTable)
        {
            if (TableStateChanged != null)
                TableStateChanged(this, new TableStateChangedEventArgs(newTable));
        }

        public void testCameraStatusChangedEvent(CameraStatus newStatus)
        {
            cameraStatus = newStatus;
        }

        public void testTableSetupChangedEvent(Piece[,] newTable)
        {
            OnTableStateChanged(newTable);
        }
    }
}
