﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MainModule
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void openTestWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestWindow tw = new TestWindow();
            tw.Show();
        }
    }
}
