using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AmobaProject_Vision_Processor;

namespace AmobaProject_Vision
{
    public partial class TestForm : Form
    {

        Camera cam1;

        public TestForm()
        {
            InitializeComponent();
            cam1 = new Camera(imageBox1, imageBox2);
            cam1.Start();
        }



        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cam1.Stop();
        }
    }
}
