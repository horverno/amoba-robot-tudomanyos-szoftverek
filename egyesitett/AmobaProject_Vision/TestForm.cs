using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AmobaProject_Vision_Processor;
using InterfaceModule;
using GameLogicsModule;
using KinematicsModule;



namespace AmobaProject_Vision
{
    public partial class TestForm : Form
    {
        GameStatus status;
        GameLogics gameLogics;
        Kinematics kinematics;
        Camera imageProcessor;


        public TestForm()
        {
            InitializeComponent();
            gameLogics = new GameLogics();
            kinematics = new Kinematics();
            imageProcessor = new Camera();
            // setting up the communication between the modules:
            gameLogics.PostMessageShowRequest += PostMessageHandler;  // game logics handles its own post messages
            kinematics.PostMessageShowRequest += PostMessageHandler;   // game logics handles the post messages of the kinematics
            imageProcessor.PostMessageShowRequest += PostMessageHandler;  // game logics handles the post messages of the image processing
            kinematics.RobotStatusChanged += gameLogics.RobotStatusChangedHandler; // game logics handles the changes of the robot arm status (source: kinematics)
            gameLogics.RobotMovementReqest += kinematics.RobotMovementRequestHandler;  // kinematics handles the robot movement requests (source: game logics)
            imageProcessor.TableStateChanged += gameLogics.TableSetupChangedHandler;
            
            imageProcessor.GameStatusChanged += new EventHandler<InterfaceModule.GameStatusChangedEventArgs>(cam1_CameraStatusChanged);
            imageProcessor.NextPieceChanged += gameLogics.NextPieceChangedHandler;

            imageProcessor.Init(pictureBox1, pictureBox2, pictureBox3, 17, 452, 200, 500, 359, 10, "Consolas");
        }

        public void PostMessageHandler(object sender, PostMessageEventArgs e)
        {
            Console.WriteLine(e.ToString(), sender.ToString());
        }

        void cam1_CameraStatusChanged(object sender, InterfaceModule.GameStatusChangedEventArgs e)
        {
            if (e.CurrentStatus == InterfaceModule.GameStatus.Online)
            {
                status = e.CurrentStatus;
                imageProcessor.Start();
                gameLogics.CameraStatusChangedHandler(sender, e);
            }
            Console.WriteLine(e.ToString());
        }

        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(status == GameStatus.Online)
                imageProcessor.Stop();     
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            imageProcessor.NewGame();
        }


    }
}
