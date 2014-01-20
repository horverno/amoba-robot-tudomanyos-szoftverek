using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using InterfaceModule;
using GameLogicsModule;
using KinematicsModule;
using AmobaProject_Vision_Processor;

namespace MainModule
{
    public partial class RoboTicTacToe : Form
    {
        GameStatus status;
        GameLogics gameLogics;
        Kinematics kinematics;
        Camera imageProcessor;

        /// <summary>
        /// The main form of the program.
        /// </summary>
        public RoboTicTacToe()
        {
            InitializeComponent();

            // creating the components:
            gameLogics = new GameLogics(Piece.O);
            kinematics = new Kinematics();
            imageProcessor = new Camera();
            
            // establishing communication:
            gameLogics.PostMessageShowRequest += PostMessageHandler;
            kinematics.PostMessageShowRequest += PostMessageHandler;
            imageProcessor.PostMessageShowRequest += PostMessageHandler;
            kinematics.RobotStatusChanged += gameLogics.RobotStatusChangedHandler; // game logics handles the changes of the robot arm status (source: kinematics)
            gameLogics.RobotMovementReqest += kinematics.RobotMovementRequestHandler;  // kinematics handles the robot movement requests (source: game logics)
            imageProcessor.TableStateChanged += gameLogics.TableSetupChangedHandler;
            imageProcessor.NextPieceChanged += gameLogics.NextPieceChangedHandler;
            imageProcessor.GameStatusChanged += new EventHandler<InterfaceModule.GameStatusChangedEventArgs>(gameStatusChanged);
            
            // initialisation:
            imageProcessor.Init(pictureBox1, pictureBox2, pictureBox3, 17, 452, 200, 500, 359, 10, "Consolas");
        }
        
        /// <summary>
        /// Nedded for starting the image processor before calling the handler of the game logics.
        /// </summary>
        void gameStatusChanged(object sender, InterfaceModule.GameStatusChangedEventArgs e)
        {
            if (e.CurrentStatus == InterfaceModule.GameStatus.Online)
            {
                status = e.CurrentStatus;
                imageProcessor.Start();
                gameLogics.CameraStatusChangedHandler(sender, e);
            }
            Console.WriteLine(e.ToString());
        }

        /// <summary>Displays the incoming messages on the form.</summary>
        public void PostMessageHandler(object sender, PostMessageEventArgs e)
        {
            if (e.important){
                lblNotifier.Text = e.message;
            }else{
                textBox1.AppendText(e.message + "\n");
            }
        }

        private void újJátékToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageProcessor.NewGame();
        }

        private void RoboTicTacToe_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (status == GameStatus.Online)
                imageProcessor.Stop(); 
        }        
    }
}
