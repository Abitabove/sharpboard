using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpBoardLibrary;
using System.IO;

namespace SharpBoardWinForm
{
    public partial class ToolBarForm : Form
    {
        Form frmParent;
        public SharpBoard SB { get; set; }
        string collapseChar = "↑";
        string expandChar = "↓";
        bool isCollapsed = false;
        bool isPortrait = true;

        private Point distanceFormToolbar;

        public ToolBarForm(Form parent)
        {
            frmParent = parent;
            InitializeComponent();
        }

        private void ToolBarForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(64, 490);
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Location = new Point(SharpBoard.ToolBarSetting.X, SharpBoard.ToolBarSetting.Y);
        }

        private void ToolBarForm_Layout(object sender, LayoutEventArgs e)
        {
            UpdateFormContent();
        }

        private void lblMoveToolbar_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    distanceFormToolbar = new Point(MousePosition.X - this.Location.X, MousePosition.Y - this.Location.Y);
                    //mouseDown = true;
                    break;
            }
        }

        private void lblMoveToolbar_MouseMove(object sender, MouseEventArgs e)
        {
            int x = MousePosition.X - distanceFormToolbar.X;
            int y = MousePosition.Y - distanceFormToolbar.Y;
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(x, y);
            }
            SharpBoard.ToolBarSetting.X = x;
            SharpBoard.ToolBarSetting.Y = y;
        }

        private void toolStripMenuSelection_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnu = (ToolStripMenuItem)sender;
            string presName = mnu.Text;
            ((MainForm)frmParent).cboPresenterFileNames.SelectedItem = presName;

            ////string presName = cboPresenterFileNames.SelectedItem.ToString();
            //SB.Actions = SB.ActionsSettingsFiles[presName];
            //SB.ActionsSettingsFileName = presName;
            //// SetDisplaySettingsFromWiimoteActions();
            //UpdateFormContent();
        }

        private void btnToolbar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string ctrlName = btn.Name.Replace("btn", "");
            if (SB.Actions.Events.ContainsKey(ctrlName))
            {
                ActionBase act = SB.Actions.Events[ctrlName].Action;
                SB.ExecuteAction(act);
            }

        }

        private void lblCollExp_Click(object sender, EventArgs e)
        {
            int width = 0;
            if (isCollapsed)
            {
                lblCollExp.Text = collapseChar;
                width = btnToolBar10.Location.X + btnToolBar10.Size.Width + 10;
                int height = btnToolBar10.Location.Y + btnToolBar10.Size.Height + 10;
                this.Size = new Size(width, height);
            }
            else
            {
                if (!isPortrait)
                {
                    width = btnOrientation.Location.X + btnOrientation.Size.Width + 10;
                }
                else
                {
                    width = btnToolBar10.Location.X + btnToolBar10.Size.Width + 10;
                }
                lblCollExp.Text = expandChar;
                this.Size = new Size(width, 19);
            }
            isCollapsed = !isCollapsed;
        }

        private void btnOrientation_Click(object sender, EventArgs e)
        {
            Size btnDim = btnOrientation.Size;
            Point btnLoc = btnOrientation.Location;
            int num = 0;
            List<Button> buttons = new List<Button>();
            foreach (var btn in pnlButton.Controls)
            {
                Button button = btn as Button;
                if (button != null)
                {
                    buttons.Add(button);
                    num++;
                }
            }

            var btnOrd = buttons.OrderBy(b => b.TabIndex);
            int x = btnLoc.X;
            int y = btnLoc.Y;
            foreach (var btn in btnOrd)
            {
                btn.Location = new Point(x, y);
                if (isPortrait)
                {
                    x += btnDim.Width + 1;
                }
                else
                {
                    y += btnDim.Height + 1;
                }
            }
            if (isPortrait)
            {
                y += btnDim.Height + 2;
            }
            else
            {
                x += btnDim.Width + 2;

            }

            int width = btnToolBar10.Location.X + btnToolBar10.Size.Width + 10;
            int height = btnToolBar10.Location.Y + btnToolBar10.Size.Height + 10;
            this.Size = new Size(width, height);
            isPortrait = !isPortrait;
            SharpBoard.ToolBarSetting.IsPortrait = isPortrait;
        }

        public void UpdateFormContent()
        {
            if (SB != null)
            {
                ctxMenuChangeSettings.Items.Clear();
                foreach (var item in SB.ActionsSettingsFiles.Keys.ToList())
                {
                    ToolStripMenuItem ts = new ToolStripMenuItem();
                    ts.Text = item;
                    ts.Click += new EventHandler(toolStripMenuSelection_Click);
                    ctxMenuChangeSettings.Items.Add(ts);
                }


                var fired = from f in SB.Actions.Events
                            where f.Value.ToolBarButton != ToolBarButtons.None
                            select f.Key;
                foreach (var cnt in pnlButton.Controls)
                {
                    Button btn = cnt as Button;
                    if (btn != null)
                    {
                        btn.Click -= btnToolbar_Click;
                        string ctrlName = btn.Name.Replace("btn", "");
                        if (ctrlName.Contains("ToolBar"))
                        {
                            btn.Image = null;
                            btn.Enabled = false;
                            if (fired.Contains(ctrlName))
                            {
                                btn.Enabled = true;
                                btn.Click += btnToolbar_Click;
                                if (SharpBoard.ToolBarButtons != null)
                                {
                                    if (SharpBoard.ToolBarButtons.ContainsKey(ctrlName))
                                    {
                                        string iconPath = SharpBoard.ToolBarButtons[ctrlName].IconPath;
                                        if (File.Exists(iconPath))
                                        {
                                            btn.Image = new Bitmap(iconPath);
                                        }
                                        btn.Text = SharpBoard.ToolBarButtons[ctrlName].Text;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
