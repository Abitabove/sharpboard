namespace SharpBoardWinForm
{
    partial class ToolBarForm
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
            this.components = new System.ComponentModel.Container();
            this.btnToolBar1 = new System.Windows.Forms.Button();
            this.btnChangeSettings = new System.Windows.Forms.Button();
            this.ctxMenuChangeSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tipToolbarButton = new System.Windows.Forms.ToolTip(this.components);
            this.btnOrientation = new System.Windows.Forms.Button();
            this.lblMoveToolbar = new System.Windows.Forms.Label();
            this.lblCollExp = new System.Windows.Forms.Label();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnToolBar10 = new System.Windows.Forms.Button();
            this.btnToolBar9 = new System.Windows.Forms.Button();
            this.btnToolBar8 = new System.Windows.Forms.Button();
            this.btnToolBar7 = new System.Windows.Forms.Button();
            this.btnToolBar6 = new System.Windows.Forms.Button();
            this.btnToolBar5 = new System.Windows.Forms.Button();
            this.btnToolBar4 = new System.Windows.Forms.Button();
            this.btnToolBar3 = new System.Windows.Forms.Button();
            this.btnToolBar2 = new System.Windows.Forms.Button();
            this.ctxMenuChangeSettings.SuspendLayout();
            this.pnlButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnToolBar1
            // 
            this.btnToolBar1.Location = new System.Drawing.Point(1, 94);
            this.btnToolBar1.Name = "btnToolBar1";
            this.btnToolBar1.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar1.TabIndex = 3;
            this.btnToolBar1.Text = "Act 1";
            this.btnToolBar1.UseVisualStyleBackColor = true;
            this.btnToolBar1.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnChangeSettings
            // 
            this.btnChangeSettings.ContextMenuStrip = this.ctxMenuChangeSettings;
            this.btnChangeSettings.Location = new System.Drawing.Point(1, 54);
            this.btnChangeSettings.Name = "btnChangeSettings";
            this.btnChangeSettings.Size = new System.Drawing.Size(50, 34);
            this.btnChangeSettings.TabIndex = 2;
            this.btnChangeSettings.Text = "Settings";
            this.tipToolbarButton.SetToolTip(this.btnChangeSettings, "LeftClick: Change orientation - RightClick: Choose settings");
            this.btnChangeSettings.UseVisualStyleBackColor = true;
            this.btnChangeSettings.Click += new System.EventHandler(this.btnChangeSettings_Click);
            // 
            // ctxMenuChangeSettings
            // 
            this.ctxMenuChangeSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.ctxMenuChangeSettings.Name = "ctxMenuChangeSettings";
            this.ctxMenuChangeSettings.Size = new System.Drawing.Size(81, 26);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(80, 22);
            this.toolStripMenuItem2.Text = "1";
            // 
            // btnOrientation
            // 
            this.btnOrientation.ContextMenuStrip = this.ctxMenuChangeSettings;
            this.btnOrientation.Location = new System.Drawing.Point(1, 18);
            this.btnOrientation.Name = "btnOrientation";
            this.btnOrientation.Size = new System.Drawing.Size(50, 34);
            this.btnOrientation.TabIndex = 1;
            this.btnOrientation.Text = "Orientation";
            this.tipToolbarButton.SetToolTip(this.btnOrientation, "Change Orientation");
            this.btnOrientation.UseVisualStyleBackColor = true;
            this.btnOrientation.Click += new System.EventHandler(this.btnOrientation_Click);
            // 
            // lblMoveToolbar
            // 
            this.lblMoveToolbar.AutoSize = true;
            this.lblMoveToolbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoveToolbar.Location = new System.Drawing.Point(0, 0);
            this.lblMoveToolbar.Name = "lblMoveToolbar";
            this.lblMoveToolbar.Size = new System.Drawing.Size(41, 15);
            this.lblMoveToolbar.TabIndex = 2;
            this.lblMoveToolbar.Text = "Move";
            this.lblMoveToolbar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMoveToolbar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblMoveToolbar_MouseMove);
            this.lblMoveToolbar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblMoveToolbar_MouseDown);
            // 
            // lblCollExp
            // 
            this.lblCollExp.AutoSize = true;
            this.lblCollExp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCollExp.Location = new System.Drawing.Point(38, 0);
            this.lblCollExp.Name = "lblCollExp";
            this.lblCollExp.Size = new System.Drawing.Size(20, 15);
            this.lblCollExp.TabIndex = 3;
            this.lblCollExp.Text = "↑";
            this.lblCollExp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCollExp.Click += new System.EventHandler(this.lblCollExp_Click);
            // 
            // pnlButton
            // 
            this.pnlButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlButton.Controls.Add(this.btnOrientation);
            this.pnlButton.Controls.Add(this.btnToolBar10);
            this.pnlButton.Controls.Add(this.btnToolBar9);
            this.pnlButton.Controls.Add(this.btnToolBar8);
            this.pnlButton.Controls.Add(this.btnToolBar7);
            this.pnlButton.Controls.Add(this.btnToolBar6);
            this.pnlButton.Controls.Add(this.btnToolBar5);
            this.pnlButton.Controls.Add(this.btnToolBar4);
            this.pnlButton.Controls.Add(this.btnToolBar3);
            this.pnlButton.Controls.Add(this.btnToolBar2);
            this.pnlButton.Controls.Add(this.lblMoveToolbar);
            this.pnlButton.Controls.Add(this.btnChangeSettings);
            this.pnlButton.Controls.Add(this.lblCollExp);
            this.pnlButton.Controls.Add(this.btnToolBar1);
            this.pnlButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButton.Location = new System.Drawing.Point(0, 0);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(177, 453);
            this.pnlButton.TabIndex = 4;
            // 
            // btnToolBar10
            // 
            this.btnToolBar10.Location = new System.Drawing.Point(1, 391);
            this.btnToolBar10.Name = "btnToolBar10";
            this.btnToolBar10.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar10.TabIndex = 12;
            this.btnToolBar10.Text = "Act 10";
            this.btnToolBar10.UseVisualStyleBackColor = true;
            this.btnToolBar10.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar9
            // 
            this.btnToolBar9.Location = new System.Drawing.Point(1, 358);
            this.btnToolBar9.Name = "btnToolBar9";
            this.btnToolBar9.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar9.TabIndex = 11;
            this.btnToolBar9.Text = "Act 9";
            this.btnToolBar9.UseVisualStyleBackColor = true;
            this.btnToolBar9.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar8
            // 
            this.btnToolBar8.Location = new System.Drawing.Point(1, 325);
            this.btnToolBar8.Name = "btnToolBar8";
            this.btnToolBar8.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar8.TabIndex = 10;
            this.btnToolBar8.Text = "Act 8";
            this.btnToolBar8.UseVisualStyleBackColor = true;
            this.btnToolBar8.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar7
            // 
            this.btnToolBar7.Location = new System.Drawing.Point(1, 292);
            this.btnToolBar7.Name = "btnToolBar7";
            this.btnToolBar7.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar7.TabIndex = 9;
            this.btnToolBar7.Text = "Act 7";
            this.btnToolBar7.UseVisualStyleBackColor = true;
            this.btnToolBar7.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar6
            // 
            this.btnToolBar6.Location = new System.Drawing.Point(1, 259);
            this.btnToolBar6.Name = "btnToolBar6";
            this.btnToolBar6.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar6.TabIndex = 8;
            this.btnToolBar6.Text = "Act 6";
            this.btnToolBar6.UseVisualStyleBackColor = true;
            this.btnToolBar6.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar5
            // 
            this.btnToolBar5.Location = new System.Drawing.Point(1, 226);
            this.btnToolBar5.Name = "btnToolBar5";
            this.btnToolBar5.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar5.TabIndex = 7;
            this.btnToolBar5.Text = "Act 5";
            this.btnToolBar5.UseVisualStyleBackColor = true;
            this.btnToolBar5.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar4
            // 
            this.btnToolBar4.Location = new System.Drawing.Point(1, 193);
            this.btnToolBar4.Name = "btnToolBar4";
            this.btnToolBar4.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar4.TabIndex = 6;
            this.btnToolBar4.Text = "Act 4";
            this.btnToolBar4.UseVisualStyleBackColor = true;
            this.btnToolBar4.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar3
            // 
            this.btnToolBar3.Location = new System.Drawing.Point(1, 160);
            this.btnToolBar3.Name = "btnToolBar3";
            this.btnToolBar3.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar3.TabIndex = 5;
            this.btnToolBar3.Text = "Act 3";
            this.btnToolBar3.UseVisualStyleBackColor = true;
            this.btnToolBar3.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnToolBar2
            // 
            this.btnToolBar2.Location = new System.Drawing.Point(1, 127);
            this.btnToolBar2.Name = "btnToolBar2";
            this.btnToolBar2.Size = new System.Drawing.Size(50, 34);
            this.btnToolBar2.TabIndex = 4;
            this.btnToolBar2.Text = "Act 2";
            this.btnToolBar2.UseVisualStyleBackColor = true;
            this.btnToolBar2.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // ToolBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 453);
            this.Controls.Add(this.pnlButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ToolBarForm";
            this.Text = "ToolBar";
            this.Load += new System.EventHandler(this.ToolBarForm_Load);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ToolBarForm_Layout);
            this.ctxMenuChangeSettings.ResumeLayout(false);
            this.pnlButton.ResumeLayout(false);
            this.pnlButton.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnToolBar1;
        private System.Windows.Forms.Button btnChangeSettings;
        private System.Windows.Forms.ContextMenuStrip ctxMenuChangeSettings;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolTip tipToolbarButton;
        private System.Windows.Forms.Label lblMoveToolbar;
        private System.Windows.Forms.Label lblCollExp;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Button btnToolBar10;
        private System.Windows.Forms.Button btnToolBar9;
        private System.Windows.Forms.Button btnToolBar8;
        private System.Windows.Forms.Button btnToolBar7;
        private System.Windows.Forms.Button btnToolBar6;
        private System.Windows.Forms.Button btnToolBar5;
        private System.Windows.Forms.Button btnToolBar4;
        private System.Windows.Forms.Button btnToolBar3;
        private System.Windows.Forms.Button btnToolBar2;
        private System.Windows.Forms.Button btnOrientation;
    }
}