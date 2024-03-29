﻿using System;
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
using ImageProcessingModule;

namespace MainModule
{

    public partial class TestWindow : Form
   {
        Kinematics kinematics;
        ImageProcessing imageProcessing;
        GameLogics gameLogics;

        public TestWindow()
        {
            InitializeComponent();
            kinematics = new Kinematics();
            imageProcessing = new ImageProcessing(pictureBox1);
            gameLogics = new GameLogics(kinematics, imageProcessing);
        }

        public void WritePostMessage(string message)
        {
            textBox1.AppendText(message + "\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //gameLogics.OnPostMessageShowRequest("asdf1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //kinematics.OnPostMessageShowRequest("asdf2");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //imageProcessing.OnPostMessageShowRequest("asdf3");
        }

        private void button4_Click(object sender, EventArgs e)
        {/*
            try
            {
                gameLogics.OnRobotMovementRequiest(RobotMovement.PlacePiece, Piece.X, Convert.ToInt32(comboBox1.Text), Convert.ToInt32(comboBox2.Text));
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid destination value(s)!","Error");
            }*/
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //kinematics.OnRobotStatusChanged(RobotStatus.Ready);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //imageProcessing.OnCameraStatusChanged(CameraStatus.Ok);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Piece[,] table = new Piece[,]{{Piece.X,Piece._Empty},{Piece._Unknown, Piece.O}};
            //imageProcessing.OnTableStateChanged(table);
        }
   }
}
