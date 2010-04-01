namespace SharpBoardWinForm
{
    partial class CalibrationForm
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
            this.pbCalibrate = new System.Windows.Forms.PictureBox();
            this.lblScreenSize = new System.Windows.Forms.Label();
            this.txtCurrent = new System.Windows.Forms.Label();
            this.txtCross = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbCalibrate)).BeginInit();
            this.SuspendLayout();
            // 
            // pbCalibrate
            // 
            this.pbCalibrate.Location = new System.Drawing.Point(66, 63);
            this.pbCalibrate.Margin = new System.Windows.Forms.Padding(2);
            this.pbCalibrate.Name = "pbCalibrate";
            this.pbCalibrate.Size = new System.Drawing.Size(75, 41);
            this.pbCalibrate.TabIndex = 0;
            this.pbCalibrate.TabStop = false;
            this.pbCalibrate.Click += new System.EventHandler(this.pbCalibrate_Click);
            // 
            // lblScreenSize
            // 
            this.lblScreenSize.AutoSize = true;
            this.lblScreenSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScreenSize.Location = new System.Drawing.Point(205, 52);
            this.lblScreenSize.Name = "lblScreenSize";
            this.lblScreenSize.Size = new System.Drawing.Size(109, 24);
            this.lblScreenSize.TabIndex = 1;
            this.lblScreenSize.Text = "Screen size";
            // 
            // txtCurrent
            // 
            this.txtCurrent.AutoSize = true;
            this.txtCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrent.Location = new System.Drawing.Point(205, 121);
            this.txtCurrent.Name = "txtCurrent";
            this.txtCurrent.Size = new System.Drawing.Size(171, 24);
            this.txtCurrent.TabIndex = 2;
            this.txtCurrent.Text = "Current coordinate:";
            // 
            // txtCross
            // 
            this.txtCross.AutoSize = true;
            this.txtCross.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCross.Location = new System.Drawing.Point(205, 87);
            this.txtCross.Name = "txtCross";
            this.txtCross.Size = new System.Drawing.Size(188, 24);
            this.txtCross.TabIndex = 3;
            this.txtCross.Text = "Crosshair coordinate:";
            // 
            // CalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 211);
            this.Controls.Add(this.txtCross);
            this.Controls.Add(this.txtCurrent);
            this.Controls.Add(this.lblScreenSize);
            this.Controls.Add(this.pbCalibrate);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CalibrationForm";
            this.Text = "WinCalibrationForm";
            this.Load += new System.EventHandler(this.CalibrationForm_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CalibrationForm_MouseClick);
            ((System.ComponentModel.ISupportInitialize)(this.pbCalibrate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbCalibrate;
        private System.Windows.Forms.Label lblScreenSize;
        private System.Windows.Forms.Label txtCurrent;
        private System.Windows.Forms.Label txtCross;
    }
}