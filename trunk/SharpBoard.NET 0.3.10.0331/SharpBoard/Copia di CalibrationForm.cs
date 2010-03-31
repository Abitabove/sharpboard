using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

using WiiDeviceLibrary;
using SharpBoardLibrary;

namespace SharpBoardWin
{
    public partial class CalibrationForm : Form
    {
        Bitmap bCalibration;
        Graphics gCalibration;
        Pen p = new Pen(Color.Red);

        int screenWidth = 1024;
        int screenHeight = 768;

        float calibrationMargin = .1f;

        float[] srcX = new float[4];
        float[] srcY = new float[4];
        float[] dstX = new float[4];
        float[] dstY = new float[4];

        public CalibrationForm()
        {
            InitializeComponent();
        }

        private void CalibrationForm_Load(object sender, EventArgs e)
        {
            screenWidth = Screen.GetBounds(this).Width;
            screenHeight = Screen.GetBounds(this).Height;

            this.FormBorderStyle = FormBorderStyle.None;
            //this.TopMost = true;

            this.Left = 0;
            this.Top = 0;
            this.Size = new Size(screenWidth, screenHeight);

            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyPress);

            bCalibration = new Bitmap(screenWidth, screenHeight, PixelFormat.Format24bppRgb);
            gCalibration = Graphics.FromImage(bCalibration);
            pbCalibrate.Left = 0;
            pbCalibrate.Top = 0;
            pbCalibrate.Size = new Size(screenWidth, screenHeight);

            gCalibration.Clear(Color.White);
            BeginInvoke((MethodInvoker)delegate() { pbCalibrate.Image = bCalibration; });
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((int)(byte)e.KeyCode == (int)Keys.Escape)
            {
                Close(); // Esc was pressed
                return;
            }
        }

        private void DrawCrosshair(int x, int y, int size)
        {
            gCalibration.DrawEllipse(p, x - size / 2, y - size / 2, size, size);
            gCalibration.DrawLine(p, x - size, y, x + size, y);
            gCalibration.DrawLine(p, x, y - size, x, y + size);
        }

        void ShowCalibration(int x, int y, int size)
        {
            gCalibration.Clear(Color.White);
            DrawCrosshair(x, y, size);
            BeginInvoke((MethodInvoker)delegate() { pbCalibrate.Image = bCalibration; });
        }

        public int Calibrate(int calibrationState, float x, float y)
        {
            switch (calibrationState)
            {
                case 1:
                    srcX[0] = x;
                    srcY[0] = y;
                    calibrationState = 2;
                    DoCalibration(calibrationState);
                    break;
                case 2:
                    srcX[1] = x;
                    srcY[1] = y;
                    calibrationState = 3;
                    DoCalibration(calibrationState);
                    break;
                case 3:
                    srcX[2] = x;
                    srcY[2] = y;
                    calibrationState = 4;
                    DoCalibration(calibrationState);
                    break;
                case 4:
                    srcX[3] = x;
                    srcY[3] = y;
                    calibrationState = 5;
                    DoCalibration(calibrationState);
                    break;
                default:
                    break;
            }
            return calibrationState;
        }

        void DoCalibration(int calibrationState)
        {
            int x = 0;
            int y = 0;
            int size = 25;
            switch (calibrationState)
            {
                case 1:
                    x = (int)(screenWidth * calibrationMargin);
                    y = (int)(screenHeight * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dstX[0] = x;
                    dstY[0] = y;
                    break;
                case 2:
                    x = screenWidth - (int)(screenWidth * calibrationMargin);
                    y = (int)(screenHeight * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dstX[1] = x;
                    dstY[1] = y;
                    break;
                case 3:
                    x = screenWidth - (int)(screenWidth * calibrationMargin);
                    y = screenHeight - (int)(screenHeight * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dstX[2] = x;
                    dstY[2] = y;
                    break;
                case 4:
                    x = (int)(screenWidth * calibrationMargin);
                    y = screenHeight - (int)(screenHeight * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dstX[3] = x;
                    dstY[3] = y;
                    break;
                case 5:
                    //compute warp
                    Global.Warper.SetDestination(dstX[0], dstY[0], dstX[1], dstY[1], dstX[2], dstY[2], dstX[3], dstY[3]);
                    Global.Warper.SetSource(srcX[0], srcY[0], srcX[1], srcY[1], srcX[2], srcY[2], srcX[3], srcY[3]);
                    Global.Warper.ComputeWarp();
                    BeginInvoke((MethodInvoker)delegate() { Close(); });

                    break;
                default:
                    break;
            }

        }

        private void CalibrationForm_Shown(object sender, EventArgs e)
        {
            DoCalibration(1);
        }
    }
}