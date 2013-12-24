using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AmobaProject_Vision_Processor;
using Interface;


namespace AmobaProject_Vision
{
    public partial class TestForm : Form
    {

        Camera cam1;
        CameraStatus status;


        public TestForm()
        {
            InitializeComponent();
            cam1 = new Camera();
            cam1.TableStateChanged += new EventHandler<Interface.TableStateChangedEventArgs>(cam1_TableStateChanged);
            cam1.CameraStatusChanged += new EventHandler<Interface.CameraStatusChangedEventArgs>(cam1_CameraStatusChanged);
            cam1.NextPieceChanged += new EventHandler<Interface.NextPieceChangedEventArgs>(cam1_NextPieceChanged);

            cam1.Init(imageBox1, imageBox2/*, new EventHandler<Interface.CameraStatusChangedEventArgs>(cam1_CameraStatusChanged)*/);

            //cam1.Start();
            
        }

        void cam1_TableStateChanged(object sender, Interface.TableStateChangedEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        void cam1_CameraStatusChanged(object sender, Interface.CameraStatusChangedEventArgs e)
        {
            if (e.CurrentStatus == Interface.CameraStatus.Online)
            {
                status = e.CurrentStatus;
                cam1.Start();
            }
            Console.WriteLine(e.ToString());
        }

        void cam1_NextPieceChanged(object sender, Interface.NextPieceChangedEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(status == CameraStatus.Online)
                cam1.Stop();
           
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            cam1.NewGame();
        }


    }
}
