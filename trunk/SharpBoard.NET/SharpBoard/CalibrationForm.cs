using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

using WiiDeviceLibrary;
using SharpBoardLibrary;
using SharpBoardLibrary.Common;
using SharpBoardLibrary.Display;
using System.Diagnostics;

namespace SharpBoardWinForm
{
    public partial class CalibrationForm : Form, ICalibrationForm
    {
        Bitmap bCalibration;
        Graphics gCalibration;

        WorkingArea screenData;

        CalibrationPoints src = new CalibrationPoints();
        CalibrationPoints dst = new CalibrationPoints();

        CoordinateF oldCoord = new CoordinateF(0, 0);
        CoordinateF currentCoord = new CoordinateF(0, 0);
        BasicIRBeacon[] beacons = null;
        object syncWiiUpdate = new object();

        public CalibrationPhase CalibrationState { get; set; }

        WiiDevice _deviceCalibrating;
        public WiiDevice DeviceCalibrating
        {
            get { return _deviceCalibrating; }
            set { _deviceCalibrating = value; }
        }

        public CalibrationForm()
        {
            InitializeComponent();
        }

        private void CalibrationForm_Load(object sender, EventArgs e)
        {
            this.Left = 0;
            this.Top = 0;
            // this.Size = new Size(screenWidth, screenHeight);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            // this.TopMost = true;

            //IDisplayDeviceProvider videoDevice = DisplayDeviceFactory.CreateVideoDevice();
            screenData = new WorkingArea();

            //screenData.Width = Screen.PrimaryScreen.Bounds.Width;
            //screenData.Height = Screen.PrimaryScreen.Bounds.Height;
            //screenData.Top = Screen.PrimaryScreen.Bounds.Top;
            //screenData.Left = Screen.PrimaryScreen.Bounds.Left;

            screenData.Width = Screen.PrimaryScreen.WorkingArea.Width;
            screenData.Height = Screen.PrimaryScreen.WorkingArea.Height;
            screenData.Top = Screen.PrimaryScreen.WorkingArea.Top;
            screenData.Left = Screen.PrimaryScreen.WorkingArea.Left;

            lblScreenSize.Text = string.Format("Screen Size: {0} x {1}", screenData.Width, screenData.Height);

            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyPress);

            bCalibration = new Bitmap(screenData.Width, screenData.Height, PixelFormat.Format24bppRgb);
            gCalibration = Graphics.FromImage(bCalibration);
            pbCalibrate.Left = 0;
            pbCalibrate.Top = 0;
            pbCalibrate.Size = new Size(screenData.Width, screenData.Height);

            gCalibration.Clear(Color.White);
            BeginInvoke((MethodInvoker)delegate() { pbCalibrate.Image = bCalibration; });

            DoCalibration(CalibrationPhase.First);
        }

        public void ShowForm()
        {
            if (DeviceCalibrating != null)
            {
                DeviceCalibrating.Device.Updated += Wiimote_Updated;
            }
            this.ShowDialog();
        }

        public void CloseForm()
        {
            if (DeviceCalibrating != null)
            {
                DeviceCalibrating.Device.Updated -= Wiimote_Updated;
            }
            BeginInvoke((MethodInvoker)delegate() { Close(); });
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((int)(byte)e.KeyCode == (int)Keys.Escape)
            {
                CalibrationState = CalibrationPhase.None;
                CloseForm(); // ESC key close the form
                return;
            }
        }

        private void DrawCrosshair(int x, int y, int size)
        {
            Pen p = new Pen(Color.Red);
            gCalibration.DrawEllipse(p, x - size / 2, y - size / 2, size, size);
            gCalibration.DrawLine(p, x - size, y, x + size, y);
            gCalibration.DrawLine(p, x, y - size, x, y + size);
        }

        private void ShowCalibration(int x, int y, int size)
        {
            gCalibration.Clear(Color.White);
            DrawCrosshair(x, y, size);
            BeginInvoke((MethodInvoker)delegate()
            {
                txtCross.Text = string.Format("Crosshair coordinate: x:{0} y:{1}", x, y);
                pbCalibrate.Image = bCalibration;
            });
        }

        public CalibrationPhase Calibrate(WiiDevice device, CalibrationPhase calibrationState, float x, float y)
        {
            _deviceCalibrating = device;
            if (_deviceCalibrating == null)
            {
                throw new MissingFieldException("Device cannot be null");
            }
            BeginInvoke((MethodInvoker)delegate()
            {
                txtCurrent.Text = string.Format("Current coordinate: {0} - {1}", x, y);
            });
            switch (calibrationState)
            {
                case CalibrationPhase.First:
                    src.X[0] = x;
                    src.Y[0] = y;
                    calibrationState = CalibrationPhase.Second;
                    DoCalibration(calibrationState);
                    break;
                case CalibrationPhase.Second:
                    src.X[1] = x;
                    src.Y[1] = y;
                    calibrationState = CalibrationPhase.Third;
                    DoCalibration(calibrationState);
                    break;
                case CalibrationPhase.Third:
                    src.X[2] = x;
                    src.Y[2] = y;
                    calibrationState = CalibrationPhase.Forth;
                    DoCalibration(calibrationState);
                    break;
                case CalibrationPhase.Forth:
                    src.X[3] = x;
                    src.Y[3] = y;
                    calibrationState = CalibrationPhase.Fifth;
                    DoCalibration(calibrationState);
                    break;
                default:
                    break;
            }
            return calibrationState;
        }

        private void DoCalibration(CalibrationPhase calibrationState)
        {
            // Console.WriteLine(calibrationState.ToString());
            float calibrationMargin = .1f;
            int x = 0;
            int y = 0;
            int size = 25;
            switch (calibrationState)
            {
                case CalibrationPhase.First:
                    x = (int)(screenData.Width * calibrationMargin);
                    y = (int)(screenData.Height * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dst.X[0] = x + screenData.Left;
                    dst.Y[0] = y + screenData.Top;
                    break;
                case CalibrationPhase.Second:
                    x = screenData.Width - (int)(screenData.Width * calibrationMargin);
                    y = (int)(screenData.Height * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dst.X[1] = x + screenData.Left;
                    dst.Y[1] = y + screenData.Top;
                    break;
                case CalibrationPhase.Third:
                    x = screenData.Width - (int)(screenData.Width * calibrationMargin);
                    y = screenData.Height - (int)(screenData.Height * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dst.X[2] = x + screenData.Left;
                    dst.Y[2] = y + screenData.Top;
                    break;
                case CalibrationPhase.Forth:
                    x = (int)(screenData.Width * calibrationMargin);
                    y = screenData.Height - (int)(screenData.Height * calibrationMargin);
                    ShowCalibration(x, y, size);
                    dst.X[3] = x + screenData.Left;
                    dst.Y[3] = y + screenData.Top;
                    break;
                case CalibrationPhase.Fifth:
                    //compute warp
                    _deviceCalibrating.Warper.SetDestination(dst);
                    _deviceCalibrating.Warper.SetSource(src);
                    _deviceCalibrating.Warper.ComputeWarp();
                    CloseForm();
                    break;
                default:
                    break;
            }

        }

        void Wiimote_Updated(object sender, EventArgs e)
        {
            lock (syncWiiUpdate)
            {
                IWiimote wiiDevice = (IWiimote)sender;
                if (wiiDevice.DeviceInfo.GetID() != _deviceCalibrating.DeviceID)
                {
                    throw new Exception("Error Calibrating device");
                }
                WiiDevice wiimote = _deviceCalibrating;
                // Beacon recognized
                beacons = wiimote.Device.GetOrderedIRBeacon();
                if (beacons.Length != 0)
                {
                    int x = beacons[0].X;
                    int y = beacons[0].Y;
                    #region Wiimote Calibration
                    if (CalibrationState > 0)
                    {
                        CoordinateF c = new CoordinateF(x, y);
                        currentCoord = c;
                        if (DeviceCalibrating.Behaviour == MouseBehaviour.CursorOnly)
                        {
                            return;
                        }
                        if (!c.IsApproxCoord(oldCoord))
                        {
                            CalibrationState = Calibrate(wiimote, CalibrationState, x, y);
                            if (CalibrationState == CalibrationPhase.Fifth)
                            {
                                CalibrationState = CalibrationPhase.None;
                                Thread.Sleep(500);
                            }
                            oldCoord = c;
                        }
                    }
                    #endregion
                }
            }
        }

        private void pbCalibrate_Click(object sender, EventArgs e)
        {
            if (CalibrationState > 0)
            {
                CalibrationState = Calibrate(_deviceCalibrating, CalibrationState, currentCoord.X, currentCoord.Y);
                if (CalibrationState == CalibrationPhase.Fifth)
                {
                    CalibrationState = CalibrationPhase.None;
                    Thread.Sleep(100);
                }
            }

        }

        private void CalibrationForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (CalibrationState > 0)
            {
                CalibrationState = Calibrate(_deviceCalibrating, CalibrationState, currentCoord.X, currentCoord.Y);
                if (CalibrationState == CalibrationPhase.Fifth)
                {
                    CalibrationState = CalibrationPhase.None;
                    Thread.Sleep(100);
                }
            }
        }
    }

}