namespace AmobaProject_Vision
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imbOriginal = new Emgu.CV.UI.ImageBox();
            this.btnCapture = new System.Windows.Forms.Button();
            this.imbModified = new Emgu.CV.UI.ImageBox();
            this.lblOriginal = new System.Windows.Forms.Label();
            this.lblModified = new System.Windows.Forms.Label();
            this.chbGrid = new System.Windows.Forms.CheckBox();
            this.chbXO = new System.Windows.Forms.CheckBox();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imbOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imbModified)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // imbOriginal
            // 
            this.imbOriginal.Location = new System.Drawing.Point(15, 29);
            this.imbOriginal.Name = "imbOriginal";
            this.imbOriginal.Size = new System.Drawing.Size(427, 320);
            this.imbOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imbOriginal.TabIndex = 2;
            this.imbOriginal.TabStop = false;
            // 
            // btnCapture
            // 
            this.btnCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCapture.Location = new System.Drawing.Point(880, 29);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(75, 23);
            this.btnCapture.TabIndex = 3;
            this.btnCapture.Text = "Stop";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // imbModified
            // 
            this.imbModified.Location = new System.Drawing.Point(448, 29);
            this.imbModified.Name = "imbModified";
            this.imbModified.Size = new System.Drawing.Size(427, 320);
            this.imbModified.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imbModified.TabIndex = 4;
            this.imbModified.TabStop = false;
            // 
            // lblOriginal
            // 
            this.lblOriginal.AutoSize = true;
            this.lblOriginal.Location = new System.Drawing.Point(12, 13);
            this.lblOriginal.Name = "lblOriginal";
            this.lblOriginal.Size = new System.Drawing.Size(76, 13);
            this.lblOriginal.TabIndex = 5;
            this.lblOriginal.Text = "Original image:";
            // 
            // lblModified
            // 
            this.lblModified.AutoSize = true;
            this.lblModified.Location = new System.Drawing.Point(448, 13);
            this.lblModified.Name = "lblModified";
            this.lblModified.Size = new System.Drawing.Size(81, 13);
            this.lblModified.TabIndex = 6;
            this.lblModified.Text = "Modified image:";
            // 
            // chbGrid
            // 
            this.chbGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbGrid.AutoSize = true;
            this.chbGrid.Checked = true;
            this.chbGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbGrid.Location = new System.Drawing.Point(880, 60);
            this.chbGrid.Name = "chbGrid";
            this.chbGrid.Size = new System.Drawing.Size(78, 17);
            this.chbGrid.TabIndex = 7;
            this.chbGrid.Text = "Detect grid";
            this.chbGrid.UseVisualStyleBackColor = true;
            this.chbGrid.CheckedChanged += new System.EventHandler(this.chbGrid_CheckedChanged);
            // 
            // chbXO
            // 
            this.chbXO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbXO.AutoSize = true;
            this.chbXO.Checked = true;
            this.chbXO.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbXO.Location = new System.Drawing.Point(880, 83);
            this.chbXO.Name = "chbXO";
            this.chbXO.Size = new System.Drawing.Size(76, 17);
            this.chbXO.TabIndex = 8;
            this.chbXO.Text = "Detect XO";
            this.chbXO.UseVisualStyleBackColor = true;
            this.chbXO.CheckedChanged += new System.EventHandler(this.chbXO_CheckedChanged);
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(15, 356);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(300, 300);
            this.imageBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(880, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Rotate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 543);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.chbXO);
            this.Controls.Add(this.chbGrid);
            this.Controls.Add(this.lblModified);
            this.Controls.Add(this.lblOriginal);
            this.Controls.Add(this.imbModified);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.imbOriginal);
            this.Name = "MainForm";
            this.Text = "Vision";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.imbOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imbModified)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imbOriginal;
        private System.Windows.Forms.Button btnCapture;
        private Emgu.CV.UI.ImageBox imbModified;
        private System.Windows.Forms.Label lblOriginal;
        private System.Windows.Forms.Label lblModified;
        private System.Windows.Forms.CheckBox chbGrid;
        private System.Windows.Forms.CheckBox chbXO;
        private Emgu.CV.UI.ImageBox imageBox1;
        private System.Windows.Forms.Button button1;
    }
}

