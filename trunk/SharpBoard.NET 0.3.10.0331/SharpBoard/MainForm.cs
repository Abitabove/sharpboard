using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Timers;

using WiiDeviceLibrary;
using SharpBoardLibrary;
using SharpBoardLibrary.Common;
using SharpBoardLibrary.Display;
using SharpBoardLibrary.Display.Win32;
using SharpBoardLibrary.Display.X11;
using SharpBoardLibrary.Keyboard;

using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

namespace SharpBoardWinForm
{
    public partial class MainForm : Form
    {
        SharpBoard _sb = null;

        ICalibrationForm frmCalibration;
        ToolBarForm frmToolbar = new ToolBarForm();

        BasicIRBeacon[] beacons = null;
        WiimoteButtons oldWiimoteButtons;

        MouseButton presenterMouseButton = MouseButton.None;

        bool isNewAction = false;
        bool isLoadPresenterFiles = false;
        bool isToolbarActive = false;
        bool isEditSettings = false;

        CameraSensitivity irMode = CameraSensitivity.Max;

        object syncWiiUpdate = new object();

        CoordinateF oldCoord = new CoordinateF(0, 0);

        PointF[] calibrationPoints = new PointF[4];
        Bitmap calibrationArea = null;
        bool startCalibration = false;

        // needed to WiimoteUpdate
        MouseClickState mouseState = MouseClickState.ButtonNone;
        //DateTime dt1stClick;
        //DateTime dt2ndClick;

        DateTime dtStart;
        DateTime dtStop;
        const int WIDTH_NORMAL = 220;
        const int WIDTH_SETTING = 770;
        const int HEIGHT_SIZE = 525;

        Size formSizeNormal;
        Size formSizeSetting;

        System.Timers.Timer idleTimer;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Wiidevice specific methods

        void DeviceFound(object sender, DeviceInfoEventArgs args)
        {
            IDeviceInfo deviceInfo = args.DeviceInfo;
            IDevice device = Connect(deviceInfo);
            if (device != null)
                if (device.IsConnected)
                {
                    bool ok = OnWiimoteConnected((IWiimote)device);
                    if (!ok)
                        device.Disconnect();
                }
        }

        bool OnWiimoteConnected(IWiimote wiimote)
        {
            string msgConnect = "You are connecting a Wiimote device\n";
            msgConnect += "Enter a tag name and press the OK button ";
            msgConnect += "to confirm or press the Cancel button to abort.";

            string deviceID = wiimote.DeviceInfo.GetID();

            _sb.WiiDevices[deviceID].LoadStoredData(_sb.WiiDevicesStoredInfo);
            string desc = _sb.WiiDevices[deviceID].TagName;
            InputBoxResult res = new InputBoxResult() { ReturnCode = DialogResult.OK, Text = desc };

            if (!(_sb.AutoConnectTaggedDevices && desc == _sb.TagName))
            {
                if (_sb.AllowNewDeviceConnection)
                {
                    res = InputBox.Show(msgConnect, "Connecting a device", _sb.WiiDevices[deviceID].TagName);
                }
                else
                {
                    res.ReturnCode = DialogResult.Cancel;
                }
            }

            if (res.ReturnCode == DialogResult.OK)
            {
                int ledNum = _sb.WiiDevices.Values.Count;
                _sb.WiiDevices[deviceID].TagName = res.Text;
                _sb.InitializeWiiDevice(deviceID, ledNum);
                _sb.WiiDevices[deviceID].Device.Updated += Wiimote_Updated;
                _sb.WiiDevices[deviceID].InitializedSmoothingData(8);
                _sb.SelectedWiiDeviceID = deviceID;
                EnableWiiDeviceSpecificControls();
                UpdateWiiMotesStatus(_sb.SelectedWiiDevice);
                UpdateTrackingUtilization(_sb.SelectedWiiDevice);
                UpdateDeviceTab();
            }
            return (res.ReturnCode == DialogResult.OK);
        }

        void DeviceLost(object sender, DeviceInfoEventArgs args)
        {
            // what to do when lost a device currently nothing
        }

        void DeviceDisconnected(object sender, EventArgs e)
        {
            IWiimote wiimote = (IWiimote)sender;
            string deviceID = wiimote.DeviceInfo.GetID();
            //if (_sb.SelectedWiiDeviceID == deviceID)
            //{
            //    _sb.SelectedWiiDeviceID = null;
            //    EnableWiiDeviceSpecificControls();
            //}
            _sb.WiiDevices[deviceID].Device.Updated -= Wiimote_Updated;
            _sb.WiiDevices.Remove(deviceID);

            UpdateDeviceTab();
        }

        IDevice Connect(IDeviceInfo iDevice)
        {
            if (iDevice != null)
            {
                IDevice device;
                string providerType = iDevice.GetDeviceType();
                if (_sb.BTProviders.Keys.Contains(providerType))
                {
                    try
                    {
                        device = _sb.BTProviders[providerType].Connect(iDevice);
                        device.Disconnected += new EventHandler(DeviceDisconnected);
                        if (device is IWiimote)
                        {
                            WiiDevice wiimote = new WiiDevice((IWiimote)device, WiimoteMode.WhiteBoard, MouseBehaviour.Normal);
                            _sb.WiiDevices.Add(wiimote.DeviceID, wiimote);
                            return device;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
            return null;
        }

        void Wiimote_Updated(object sender, EventArgs e)
        {
            lock (syncWiiUpdate)
            {
                if (startCalibration)
                    return;

                IWiimote wiiDevice = (IWiimote)sender;
                WiiDevice wiimote = _sb.WiiDevices[wiiDevice.DeviceInfo.GetID()];

                WiimoteButtons changedButtons = oldWiimoteButtons ^ wiimote.Device.Buttons;
                WiimoteButtons pressedButtons = changedButtons & wiimote.Device.Buttons;
                WiimoteButtons releasedButtons = changedButtons & oldWiimoteButtons;
                oldWiimoteButtons = wiimote.Device.Buttons;

                switch (wiimote.Mode)
                {
                    case WiimoteMode.Disabled:
                        return;
                        break;
                    case WiimoteMode.WhiteBoard:
                        if (((releasedButtons & WiimoteButtons.A) == WiimoteButtons.A))
                        {
                            startCalibration = true;
                            SetCalibration(_sb.SelectedWiiDevice);
                            return;
                        }
                        break;
                    case WiimoteMode.Presenter:
                        #region PressedButtons
                        if (pressedButtons != WiimoteButtons.None)
                        {
                            string wiiButton = pressedButtons.ToString();
                            if (_sb.Actions.Events.ContainsKey(wiiButton))
                            {
                                ActionBase act = _sb.Actions.Events[wiiButton].Action;
                                MouseButton pres = _sb.ExecuteAction(act);
                                if (act.Type == ActionType.Mouse)
                                {
                                    presenterMouseButton = pres;
                                }
                                //switch (act.Type)
                                //{
                                //    case ActionType.Keys:
                                //        ActionKeys actk = act as ActionKeys;
                                //        string key = actk.Keys;
                                //        _sb.VideoDevice.SendKeys(key);
                                //        break;
                                //    case ActionType.Mouse:
                                //        ActionMouse actm = act as ActionMouse;
                                //        presenterMouseButton = actm.Mouse;
                                //        break;
                                //    case ActionType.Toogle:
                                //        ActionToogle actT = act as ActionToogle;
                                //        switch (actT.Feature)
                                //        {
                                //            case ToogleFeature.CursorOnly:
                                //                switch (actT.Action)
                                //                {
                                //                    case ToogleAction.On:
                                //                        _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.CursorOnly;
                                //                        break;
                                //                    case ToogleAction.Off:
                                //                        _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.Normal;
                                //                        break;
                                //                    case ToogleAction.Toogle:
                                //                        switch (_sb.SelectedWiiDevice.Behaviour)
                                //                        {
                                //                            case MouseBehaviour.Normal:
                                //                                _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.CursorOnly;
                                //                                break;
                                //                            case MouseBehaviour.CursorOnly:
                                //                                _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.Normal;
                                //                                break;
                                //                            case MouseBehaviour.LeftButton:
                                //                            case MouseBehaviour.RightButton:
                                //                            case MouseBehaviour.DoubleClick:
                                //                            default:
                                //                                break;
                                //                        }
                                //                        break;
                                //                    default:
                                //                        break;
                                //                }
                                //                break;
                                //            default:
                                //                break;
                                //        }
                                //        break;
                                //    case ActionType.Process:
                                //        ActionProcess actp = act as ActionProcess;
                                //        Process.Start(actp.CmdLine);
                                //        break;
                                //    default:
                                //        break;
                                //}

                            }
                        }
                        #endregion PressedButtons

                        #region ReleasedButtons
                        if (releasedButtons != WiimoteButtons.None)
                        {
                            string wiiButton = releasedButtons.ToString();
                            if (_sb.Actions.Events.ContainsKey(wiiButton))
                            {
                                ActionBase act = _sb.Actions.Events[wiiButton].Action;
                                switch (act.Type)
                                {
                                    case ActionType.Keys:
                                        break;
                                    case ActionType.Mouse:
                                        presenterMouseButton = MouseButton.None;
                                        break;
                                    case ActionType.Toogle:
                                        break;
                                    case ActionType.Process:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        #endregion ReleasedButtons
                        break;
                    default:
                        return;
                        break;
                }


                // Beacon recognized
                beacons = wiimote.Device.GetOrderedIRBeacon();
                if (beacons.Length != 0)
                {
                    int x = beacons[0].X;
                    int y = beacons[0].Y;
                    //Console.WriteLine("{0}-{1}",x,y);

                    CoordinateF c = new CoordinateF(x, y);

                    float warpedX = x;
                    float warpedY = y;
                    if (wiimote.Mode == WiimoteMode.Presenter)
                    {
                        wiimote.Warper.WarpPresenter(_sb.ScreenSize.Width, _sb.ScreenSize.Height, ref warpedX, ref warpedY);
                    }
                    else
                    {
                        wiimote.Warper.Warp(x, y, ref warpedX, ref warpedY);
                    }
                    //Console.WriteLine(mouseState);
                    if (mouseState == MouseClickState.ButtonNone)
                    {
                        wiimote.Smoothing.SmoothingBufferIndex = 0;
                    }
                    wiimote.Smoothing.SmoothingBuffer[wiimote.Smoothing.SmoothingBufferIndex % wiimote.Smoothing.SmoothingBuffer.Length].X = warpedX;
                    wiimote.Smoothing.SmoothingBuffer[wiimote.Smoothing.SmoothingBufferIndex % wiimote.Smoothing.SmoothingBuffer.Length].Y = warpedY;
                    wiimote.Smoothing.SmoothingBufferIndex++;

                    int smoothX = 0;
                    int smoothY = 0;
                    _sb.VideoDevice.GetSmoothingCoords(out smoothX, out smoothY, warpedX, warpedY, wiimote.Smoothing);
                    switch (mouseState)
                    {
                        case MouseClickState.ButtonNone:
                            if (presenterMouseButton == MouseButton.Right)
                            {
                                _sb.VideoDevice.SetCursorAndMouse(smoothX, smoothY, MouseButton.Right, MouseAction.Down);
                                mouseState = MouseClickState.RightDown;
                                return;
                            }
                            else if (presenterMouseButton == MouseButton.Left
                                    || wiimote.Behaviour == MouseBehaviour.Normal)
                            {
                                _sb.VideoDevice.SetCursorAndMouse(smoothX, smoothY, MouseButton.Left, MouseAction.Down);
                                mouseState = MouseClickState.LeftDown;
                            }
                            else
                            {
                                _sb.VideoDevice.SetCursor(smoothX, smoothY);
                            }

                            if (mouseState == MouseClickState.LeftDown)
                            {
                                dtStart = DateTime.Now;
                                oldCoord = c;
                            }
                            break;
                        case MouseClickState.LeftDown:
                            _sb.VideoDevice.SetCursor(smoothX, smoothY);
                            if (SharpBoard.RightClickTime > 0 && wiimote.Behaviour == MouseBehaviour.Normal)
                            {
                                if (c.IsApproxCoord(oldCoord))
                                {
                                    dtStop = DateTime.Now;
                                    TimeSpan diff = dtStop - dtStart;
                                    if (diff.TotalMilliseconds > SharpBoard.RightClickTime)
                                    {
                                        _sb.VideoDevice.SetMouse(MouseButton.Left, MouseAction.Up);
                                        _sb.VideoDevice.SetCursorAndMouse(smoothX, smoothY, MouseButton.Right, MouseAction.Down);
                                        _sb.VideoDevice.SetMouse(MouseButton.Right, MouseAction.Up);
                                        mouseState = MouseClickState.RightUp;
                                    }
                                }
                                else
                                {
                                    dtStart = DateTime.Now;
                                    oldCoord = c;
                                }
                            }
                            else
                            {
                                dtStart = DateTime.Now;
                                oldCoord = c;
                            }

                            if (wiimote.Mode == WiimoteMode.Presenter)
                            {
                                if (presenterMouseButton == MouseButton.None
                                    && wiimote.Behaviour != MouseBehaviour.Normal)
                                {
                                    _sb.VideoDevice.SetMouse(MouseButton.Left, MouseAction.Up);
                                    mouseState = MouseClickState.ButtonNone;
                                }
                            }
                            break;
                        case MouseClickState.LeftUp:
                            break;
                        case MouseClickState.RightDown:
                            _sb.VideoDevice.SetCursor(smoothX, smoothY);
                            if (presenterMouseButton == MouseButton.None)
                            {
                                _sb.VideoDevice.SetMouse(MouseButton.Right, MouseAction.Up);
                                mouseState = MouseClickState.ButtonNone;
                            }
                            break;
                        case MouseClickState.RightUp:
                            if (wiimote.Mode == WiimoteMode.Presenter)
                            {
                                mouseState = MouseClickState.ButtonNone;
                            }
                            break;
                        default:
                            break;
                    }
                    _sb.VideoDevice.RefreshIRMonitor(picIRBeacons);
                }
                else
                {
                    if (mouseState == MouseClickState.LeftDown)
                    {
                        _sb.VideoDevice.SetMouse(MouseButton.Left, MouseAction.Up);
                    }
                    mouseState = MouseClickState.ButtonNone;
                }
            }
        }

        #endregion

        #region Setting and updating methods

        void SetDisplaySettingsFromApplicationSettings()
        {
            // Wiidevice
            chkAutoConnectTaggedDevice.Checked = _sb.AutoConnectTaggedDevices;
            txtTagName.Text = _sb.TagName;
            chkAllowNewDeviceConnection.Checked = _sb.AllowNewDeviceConnection;

            // Interface
            chkStartGUI.Checked = !_sb.StartGui;
            chkMinimizeTray.Checked = _sb.MinimizeOnTray;
            udRightClick.Value = SharpBoard.RightClickTime;

            // Smoothing
            chkSmoothingEnable.Checked = SharpBoard.Smoothing.IsEnabled;
            udSmoothingAmount.Value = SharpBoard.Smoothing.Amount;
            udSmoothingBufferSize.Value = SharpBoard.Smoothing.BufferSize;

        }

        void SetPresenterFileDisplay()
        {
            isLoadPresenterFiles = true;
            cboPresenterFileNames.DataSource = _sb.ActionsSettingsFiles.Keys.ToList();
            int index = -1;
            foreach (var name in _sb.ActionsSettingsFiles.Keys)
            {
                index++;
                if (name == _sb.ActionsSettingsFileName)
                {
                    break;
                }
            }
            cboPresenterFileNames.SelectedIndex = index;
            isLoadPresenterFiles = false;
        }

        void SetDisplaySettingsFromWiimoteActions()
        {
            if (_sb.Actions != null)
            {
                var actionsList = _sb.Actions.Keys.ToList();
                lboActions.DataSource = actionsList;
                cboType.DataSource = Enum.GetNames(typeof(ActionType));
                SetDisplaySettingsByGroupType(grpButtons, FiredByType.WiimoteButtons);
                SetDisplaySettingsByGroupType(grpToolbar, FiredByType.ToolBarButtons);
                SetToolBarButtons(_sb.Actions.FullName);
            }
        }

        void SetToolBarButtons(string path)
        {
            if (path != null)
            {
                FileInfo fi = new FileInfo(path);
                string toolPath = fi.FullName.Replace(fi.Extension, ".toolbar.xml");
                SharpBoard.ToolBarButtons = null;
                if (File.Exists(toolPath))
                {
                    SharpBoard.ToolBarButtons = ToolBarButtonCollection.LoadFromXML(toolPath);
                }
            }
        }

        void SetDisplaySettingsByGroupType(GroupBox grp, FiredByType type)
        {
            foreach (var item in grp.Controls)
            {
                ComboBox cbo = item as ComboBox;
                if (cbo != null)
                {
                    string ctrlName = cbo.Name.Replace("cboBtn", "");
                    List<string> actions = new List<string>();
                    foreach (var key in _sb.Actions.Keys)
                    {
                        if (type == FiredByType.ToolBarButtons)
                        {
                            if (_sb.Actions[key].Type == ActionType.Keys ||
                                _sb.Actions[key].Type == ActionType.Mouse)
                            {
                                continue;
                            }
                        }
                        actions.Add(key);
                    }
                    cbo.DataSource = actions;
                    var eventi = from ev in _sb.Actions.Events
                                 where ev.Key == ctrlName
                                 select ev;
                    foreach (var evento in eventi)
                    {
                        CheckBox chk = (CheckBox)grp.Controls["chkBtn" + ctrlName];
                        chk.Checked = evento.Value.IsActive;
                        int i = 0;
                        if (evento.Value.Action != null)
                        {
                            string actName = evento.Value.Action.Description;
                            foreach (string s in actions)
                            {
                                if (s.Contains(actName))
                                    break;
                                i++;
                            }
                        }
                        if (i >= cbo.Items.Count)
                        {
                            i = 0;
                        }
                        cbo.SelectedIndex = i;
                    }
                }
            }
        }

        void SetWiimoteActionsFromDisplay()
        {
            ActionBase actNone = new ActionBase() { Type = ActionType.None, Description = "None" };
            ActionCollection acts = new ActionCollection();
            acts.Add(actNone.Description, actNone);
            if (_sb.Actions != null)
            {
                SetActionsForGroupType(acts, grpButtons, FiredByType.WiimoteButtons);
                SetActionsForGroupType(acts, grpToolbar, FiredByType.ToolBarButtons);
                string fileName = cboPresenterFileNames.SelectedItem.ToString();
                string path = Path.Combine(Global.AppFolderPath, fileName);
                acts.SaveToXML(path);
                _sb.Actions = acts;
                // _sb.Events = acts.Events;
                Console.WriteLine();
            }
        }

        void SetActionsForGroupType(ActionCollection acts, GroupBox grp, FiredByType type)
        {
            ActionBase act = null;
            FiredBy fired = null;
            foreach (var item in grp.Controls)
            {
                ComboBox cbo = item as ComboBox;
                if (cbo != null)
                {
                    var actDesc = cbo.SelectedItem.ToString();
                    try
                    {
                        ActionType actType = (ActionType)Enum.Parse(typeof(ActionType), cbo.Text);
                        if (actType == ActionType.None)
                        {
                            continue;
                        }

                    }
                    catch
                    {

                    }
                    string ctrlName = cbo.Name.Replace("cboBtn", "");
                    if (!acts.ContainsKey(actDesc))
                    {
                        act = _sb.Actions[actDesc];
                        act.Fired.Clear();
                        acts.Add(act.Description, act);
                    }
                    else
                    {
                        act = acts[actDesc];
                        if (act.Fired == null)
                        {
                            act.Fired = new FiredByCollection();
                        }
                    }
                    CheckBox chk = (CheckBox)grp.Controls["chkBtn" + ctrlName];
                    bool isActive = chk.Checked;
                    WiimoteButtons btn = WiimoteButtons.None;
                    ToolBarButtons tbBtn = ToolBarButtons.None;
                    switch (type)
                    {
                        case FiredByType.WiimoteButtons:
                            btn = (WiimoteButtons)Enum.Parse(typeof(WiimoteButtons), ctrlName);
                            break;
                        case FiredByType.ToolBarButtons:
                            tbBtn = (ToolBarButtons)Enum.Parse(typeof(ToolBarButtons), ctrlName);
                            break;
                        default:
                            break;
                    }
                    fired = new FiredBy() { Action = act, Button = btn, ToolBarButton = tbBtn, IsActive = isActive, Type = type };
                    act.Fired.Add(ctrlName, fired);

                }
            }
        }

        void SetDiscoveryForBTProvider(FactoryType type)
        {
            foreach (IDeviceProvider provider in _sb.BTProviders.Values)
            {
                if (type == FactoryType.All)
                {
                    if (!provider.IsDiscovering)
                    {
                        provider.StartDiscovering();
                    }
                }
                else
                {
                    if (type == FactoryType.None)
                    {
                        if (provider.IsDiscovering)
                        {
                            provider.StopDiscovering();
                        }
                    }
                    else
                    {
                        if (provider.GetBTProviderType() == type.ToString())
                        {
                            if (!provider.IsDiscovering)
                            {
                                provider.StartDiscovering();
                            }
                        }
                        else
                        {
                            if (provider.IsDiscovering)
                            {
                                provider.StopDiscovering();
                            }

                        }

                    }
                }
            }
            Thread.Sleep(100);
        }

        void SetDiscoveryOptions(FactoryType discovery)
        {
            if (discovery == FactoryType.All)
            {
                cboProvider.Enabled = false;
                chkButtonDiscovery.Enabled = false;
                // frmDeviceDiscovery = null;
                SetDiscoveryForBTProvider(FactoryType.All);
            }
            else
            {
                //frmDeviceDiscovery = new DeviceDiscovery();
                //frmDeviceDiscovery.Providers = _sb.BTProviders.Values;
                SetDiscoveryForBTProvider(FactoryType.None);
                chkButtonDiscovery.Enabled = true;
                cboProvider.Enabled = true;
            }
        }

        void SetCalibration(WiiDevice device)
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                foreach (var dev in _sb.WiiDevices.Values)
                {
                    _sb.WiiDevices[dev.DeviceID].Device.Updated -= Wiimote_Updated;
                }

                frmCalibration = new CalibrationForm();
                frmCalibration.CalibrationState = CalibrationPhase.First;
                frmCalibration.DeviceCalibrating = device;
                frmCalibration.ShowForm();

                foreach (var dev in _sb.WiiDevices.Values)
                {
                    _sb.WiiDevices[dev.DeviceID].Device.Updated += Wiimote_Updated;
                }

                UpdateTrackingUtilization(_sb.SelectedWiiDevice);

                startCalibration = false;
            });
        }

        void SetWiiMouseBehaviour(bool isCursorOnly)
        {
            if (isCursorOnly)
            {
                _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.CursorOnly;
            }
            else
            {
                _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.Normal;
            }
        }

        float UpdateTrackingUtilization(WiiDevice wiimote)
        {
            float util = -1;
            int width = picIRBeacons.Width;
            int height = picIRBeacons.Height;

            if (wiimote != null)
            {
                util = wiimote.Warper.UtilArea;
                BeginInvoke((MethodInvoker)delegate() { lblTrackingUtil.Text = util.ToString("f0") + "%"; });
                BeginInvoke((MethodInvoker)delegate() { pbTrackingUtil.Value = (int)util; });
                calibrationPoints = CoordinateHelper.ScaleCoords(width, height, wiimote.Warper.SrcX, wiimote.Warper.SrcY).ToPointF();
            }

            calibrationArea = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(calibrationArea);
            Invoke((MethodInvoker)delegate
            {
                Pen p = new Pen(Brushes.White);
                if (wiimote != null)
                {
                    SolidBrush b = new SolidBrush(Color.White);
                    g.FillPolygon(b, calibrationPoints);
                }
                p.Color = Color.Blue;
                g.DrawLine(p, 0, 0, width - 1, height - 1);
                g.DrawLine(p, 0, height - 1, width - 1, 0);
                picIRBeacons.Image = calibrationArea;
            });

            _sb.VideoDevice.RefreshIRMonitor(picIRBeacons);

            return util;
        }

        void UpdateWiiMotesStatus(WiiDevice wiimote)
        {
            byte batteryLevel = wiimote.Device.BatteryLevel;
            float f = (100.0f * (float)batteryLevel) / 192.0f;
            BeginInvoke((MethodInvoker)delegate()
                {
                    switch (wiimote.Mode)
                    {
                        case WiimoteMode.Disabled:
                            rbuDisableWiimote.Checked = true;
                            break;
                        case WiimoteMode.WhiteBoard:
                            rbtnWhiteBoard.Checked = true;

                            break;
                        case WiimoteMode.Presenter:
                            rbtnPresenter.Checked = true;
                            break;
                        default:
                            break;
                    }
                    chkCursorOnly.Checked = (wiimote.Behaviour == MouseBehaviour.CursorOnly);
                    pbBattery.Value = (batteryLevel > 0xc8 ? (byte)0xc8 : batteryLevel);
                    grpBattery.Text = f.ToString("f0") + "%";
                }
            );
        }

        void UpdateDeviceTab()
        {
            Invoke((MethodInvoker)delegate()
            {
                tabWiimote.SelectedIndex = _sb.WiiDevices.Count > 0 ? _sb.WiiDevices.Count - 1 : 0;
                if (_sb.WiiDevices.Count == 0)
                {
                    _sb.SelectedWiiDeviceID = null;
                    EnableWiiDeviceSpecificControls();
                }
            });
        }

        void EnableWiiDeviceSpecificControls()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                bool isEnabled = _sb.SelectedWiiDevice != null;
                if (isEnabled)
                {
                    // grpWhiteBoard.Enabled = _sb.SelectedWiiDevice != null;
                    grpBattery.Enabled = isEnabled;
                    pnlShow.Enabled = isEnabled;
                    grpWiimoteMode.Enabled = isEnabled;
                    btnCalibrate.Enabled = true;
                }
            });
        }

        void EditDeviceSettings()
        {
            int currentWiimoteSelected = tabWiimote.SelectedIndex;
            //int currentWiimoteSelected = _sb.SelectedWiiNumber;

            if (_sb.WiiDevices.Keys.Count > currentWiimoteSelected)
            {
                string deviceID = _sb.WiiDevices.Keys[currentWiimoteSelected];
                if (_sb.WiiDevices.ContainsKey(deviceID))
                {
                    WiiDevice currentWii = _sb.WiiDevices[deviceID];
                }
            }
            if (isEditSettings)
            {
                this.Size = formSizeNormal;
            }
            else
            {
                this.Size = formSizeSetting;
            }
            isEditSettings = !isEditSettings;
        }

        #endregion

        #region Form specific methods

        private void MainForm_Load(object sender, EventArgs e)
        {
            _sb = new SharpBoard();

            _sb.InitializeDisplayDevice(this);

            _sb.InitializeBTDevices(); // Not implemented yet

            UpdateTrackingUtilization(null);

            cboProvider.Items.Clear();
            cboProvider.Items.Add("Choose provider");
            int selBt = 0;
            foreach (IDeviceProvider provider in _sb.BTProviders.Values)
            {
                selBt++;
                provider.DeviceFound += DeviceFound;
                provider.DeviceLost += DeviceLost;
                cboProvider.Items.Add(provider.GetBTProviderType());
                string btProv = cboProvider.Items[selBt].ToString();
                if (btProv == _sb.BTProviderDiscovery.ToString())
                {
                    cboProvider.SelectedIndex = selBt;
                }
            }

            idleTimer = new System.Timers.Timer();
            idleTimer.Elapsed += new ElapsedEventHandler(idleTimer_Elapsed);
            idleTimer.Interval = 10;
            idleTimer.Enabled = true;
            idleTimer.Start();

            int maxHeight = grpDevice.Location.Y + grpDevice.Size.Height + 40;
            int widthNormal = tabWiimote.Location.X + tabWiimote.Size.Width + 15;
            int widthSetting = tabSettings.Location.X + tabSettings.Size.Width + 15;

            formSizeNormal = new Size(widthNormal, maxHeight);
            formSizeSetting = new Size(widthSetting, maxHeight);
            this.Size = formSizeNormal;

            // frmCalibration = new CalibrationForm();

            chkAutomaticDiscovery.Checked = (_sb.BTProviderDiscovery == FactoryType.All);

            if (!_sb.StartGui)
            {
                WindowState = FormWindowState.Minimized;
            }

            EnableWiiDeviceSpecificControls();
            SetDisplaySettingsFromWiimoteActions();
            SetPresenterFileDisplay();
            SetDisplaySettingsFromApplicationSettings();

            btnSavePresenterSettings.Enabled = false;
            // formSize = this.Size;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            foreach (IDeviceProvider provider in _sb.BTProviders.Values)
            {
                provider.DeviceFound -= DeviceFound;
                provider.DeviceLost -= DeviceLost;
                provider.StopDiscovering();
            }

            //_sb.StartGui = (WindowState != FormWindowState.Minimized);
            _sb.SaveConfigData();

            //DialogResult risp = MessageBox.Show("Are you sure ?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (risp == DialogResult.No)
            //{
            //    e.Cancel = true;
            //}
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (_sb != null)
            {
                if (_sb.MinimizeOnTray)
                {
                    switch (WindowState)
                    {
                        case FormWindowState.Minimized:
                            ShowInTaskbar = false;
                            notifyIcon1.Visible = true;
                            notifyIcon1.ShowBalloonTip(500);
                            Hide();
                            break;
                        //case FormWindowState.Normal:
                        //    ShowInTaskbar = true;
                        //    notifyIcon1.Visible = false;
                        //    Show();
                        //    break;
                        default:
                            break;
                    }
                }
            }
        }

        private void picIRBeacons_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (beacons != null)
            {
                if (beacons.Length != 0)
                {
                    int width = picIRBeacons.Width;
                    int height = picIRBeacons.Height;

                    int i = 1;
                    foreach (ExtendedIRBeacon irBeacon in beacons)
                    {
                        if (irBeacon == null)
                            continue;
                        CoordinateI coord = (CoordinateI)CoordinateHelper.ScaleCoords(width, height, irBeacon.X, irBeacon.Y);
                        int dim = 8 / i;
                        Invoke((MethodInvoker)delegate
                        {
                            Pen p = new Pen(Brushes.Red);
                            g.DrawEllipse(p, coord.X - (dim / 2), coord.Y - (dim / 2), dim, dim);
                        });
                        i++;
                    }
                    string irDots = "";
                    for (int j = 1; j < i; j++)
                    {
                        irDots += j.ToString() + ".";
                    }
                    Invoke((MethodInvoker)delegate { lblIRvisible.Text = irDots; });
                }
            }
        }

        private void idleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.RaiseIdle(EventArgs.Empty);
        }

        private void btnCalibrate_Click(object sender, EventArgs e)
        {
            SetCalibration(_sb.SelectedWiiDevice);
        }

        private void rbuWiimoteMode_CheckedChanged(object sender, EventArgs e)
        {
            //if (_sb.WiiDevices.Keys.Count > _sb.SelectedWiiDeviceWiimoteSelected)
            {
                RadioButton rb = (RadioButton)sender;
                bool isDisabled = rb.Name.IndexOf("Disable") != -1;
                bool isPresenter = rb.Name.IndexOf("Presenter") != -1;
                if (_sb.SelectedWiiDevice != null)
                {
                    if (isDisabled)
                    {
                        _sb.SelectedWiiDevice.Mode = WiimoteMode.Disabled;
                        _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.CursorOnly;
                        btnCalibrate.Enabled = false;
                        chkCursorOnly.Enabled = false;
                    }
                    else
                    {
                        if (isPresenter)
                        {
                            _sb.SelectedWiiDevice.Mode = WiimoteMode.Presenter;
                            _sb.SelectedWiiDevice.Behaviour = MouseBehaviour.CursorOnly;
                            btnCalibrate.Enabled = false;
                            chkCursorOnly.Enabled = false;
                        }
                        else
                        {
                            _sb.SelectedWiiDevice.Mode = WiimoteMode.WhiteBoard;
                            SetWiiMouseBehaviour(chkCursorOnly.Checked);
                            btnCalibrate.Enabled = true;
                            chkCursorOnly.Enabled = true;
                        }
                    }
                }
            }
        }

        private void chkCursorOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (_sb.SelectedWiiDevice != null)
            {
                SetWiiMouseBehaviour(chkCursorOnly.Checked);
            }
        }

        private void tabWiimote_Selecting(object sender, TabControlCancelEventArgs e)
        {
            int currentWiimoteSelected = e.TabPageIndex;

            if (_sb.WiiDevices.Keys.Count <= currentWiimoteSelected)
            {
                e.Cancel = true;
            }
        }

        private void tabWiimote_Selected(object sender, TabControlEventArgs e)
        {
            int currentWiimoteSelected = e.TabPageIndex;
            int prevPage = (currentWiimoteSelected + 1) % 2;

            string deviceID = _sb.WiiDevices.Keys[currentWiimoteSelected];
            if (_sb.WiiDevices.ContainsKey(deviceID))
            {
                _sb.SelectedWiiDeviceID = deviceID;
                UpdateWiiMotesStatus(_sb.SelectedWiiDevice);
                UpdateTrackingUtilization(_sb.SelectedWiiDevice);
                while (tabWiimote.TabPages[prevPage].Controls.Count > 0)
                {
                    tabWiimote.TabPages[prevPage].Controls[0].Parent = e.TabPage;
                }
            }
            //_sb.SelectedWiiNumber = currentWiimoteSelected;
        }

        private void grpWiimoteMode_Enter(object sender, EventArgs e)
        {
            if (_sb.SelectedWiiDevice != null)
            {
                if (_sb.SelectedWiiDevice.Mode == WiimoteMode.WhiteBoard &&
                    _sb.SelectedWiiDevice.Behaviour == MouseBehaviour.CursorOnly)
                {
                    chkCursorOnly.Checked = false;
                }
            }
        }

        private void btnKeyboard_Click(object sender, EventArgs e)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    Process.Start("onboard");
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    Process.Start("osk");
                    break;
                default:
                    break;
            }
        }

        private void chkButtonDiscovery_CheckedChanged(object sender, EventArgs e)
        {
            // SetDiscoveryForBTProvider(FactoryType.None);
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked == true)
            {
                cboProvider_SelectedIndexChanged(sender, e);
                SetDiscoveryForBTProvider(_sb.BTProviderDiscovery);

                chk.Text = "Stop";
            }
            else
            {
                chk.Text = "Start";
            }
        }

        private void cboProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkAutomaticDiscovery.Checked == false)
            {
                if (cboProvider.SelectedIndex > 0)
                {
                    _sb.BTProviderDiscovery = (FactoryType)Enum.Parse(typeof(FactoryType), cboProvider.SelectedItem.ToString());
                }
                else
                {
                    _sb.BTProviderDiscovery = FactoryType.None;
                }
            }
        }

        private void chkAutomaticDiscovery_CheckedChanged(object sender, EventArgs e)
        {
            // SetDiscoveryOptions(false);
            chkAutomaticDiscovery.Enabled = false;
            chkButtonDiscovery.Checked = false;
            if (chkAutomaticDiscovery.Checked == true)
            {
                _sb.BTProviderDiscovery = FactoryType.All;
            }
            else
            {
                cboProvider_SelectedIndexChanged(sender, e);
            }
            SetDiscoveryOptions(_sb.BTProviderDiscovery);
            chkAutomaticDiscovery.Enabled = true;
        }

        private void toolStripSettings_Click(object sender, EventArgs e)
        {
            EditDeviceSettings();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //WindowState = FormWindowState.Normal;
            //ShowInTaskbar = true;
            //notifyIcon1.Visible = false;
            //Show();
            //this.Size = formSizeNormal;
            //Activate();
        }

        private void chkEnableSmoothing_CheckedChanged(object sender, EventArgs e)
        {
            SharpBoard.Smoothing.IsEnabled = chkSmoothingEnable.Checked;
            udSmoothingAmount.Enabled = SharpBoard.Smoothing.IsEnabled;
            udSmoothingBufferSize.Enabled = SharpBoard.Smoothing.IsEnabled;
        }

        private void lboActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lboActions.SelectedIndex != -1)
            {
                btnParam.Enabled = false;
                string desc = lboActions.SelectedValue.ToString();
                ActionBase act = _sb.Actions[desc];
                int index = 0;
                string param = "";
                switch (act.Type)
                {
                    case ActionType.Keys:
                        ActionKeys actk = act as ActionKeys;
                        param = actk.Keys;
                        index = 1;
                        break;
                    case ActionType.Mouse:
                        ActionMouse actm = act as ActionMouse;
                        param = actm.Mouse.ToString();
                        index = 2;
                        break;
                    case ActionType.Toogle:
                        ActionToogle actToogle = act as ActionToogle;
                        param = string.Format("{0},{1}", actToogle.Feature, actToogle.Action);
                        index = 3;
                        break;
                    case ActionType.Process:
                        ActionProcess actp = act as ActionProcess;
                        param = actp.CmdLine;
                        index = 4;
                        btnParam.Enabled = true;
                        break;
                    default:
                        break;
                }

                txtDescription.Text = act.Description;
                txtParam.Text = param;
                cboType.SelectedIndex = index;
                grpEditAction.Enabled = (lboActions.SelectedIndex > 0);
                btnActionDelete.Enabled = (lboActions.SelectedIndex > 0);
                btnActionSave.Enabled = (lboActions.SelectedIndex > 0);
            }
        }

        private void btnActionSave_Click(object sender, EventArgs e)
        {
            try
            {
                ActionType type = (ActionType)Enum.Parse(typeof(ActionType), cboType.Text);
                if (txtDescription.Text == "" || type == ActionType.None)
                {
                    throw new Exception();
                }
                string actDesc = lboActions.SelectedValue.ToString();
                ActionBase oldAct = _sb.Actions[actDesc];

                ActionBase act = null;
                switch (type)
                {
                    case ActionType.Keys:
                        act = new ActionKeys();
                        break;
                    case ActionType.Mouse:
                        act = new ActionMouse();
                        break;
                    case ActionType.Toogle:
                        act = new ActionToogle();
                        break;
                    case ActionType.Process:
                        act = new ActionProcess();
                        break;
                    default:
                        break;
                }

                act.Description = txtDescription.Text;
                act.Type = type;
                act.Fired = oldAct.Fired;
                act.SetParameters(txtParam.Text);

                if (!isNewAction)
                {
                    _sb.Actions.Remove(actDesc);
                }

                _sb.Actions.Add(act.Description, act);
                if (act.Fired != null)
                {
                    foreach (var item in act.Fired.Values)
                    {
                        item.Action = act;
                    }
                }
                SetDisplaySettingsFromWiimoteActions();
                grpEditAction.Enabled = false;
                btnActionDelete.Enabled = false;
                btnActionSave.Enabled = false;
                btnSavePresenterSettings.Enabled = true;

                isNewAction = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Invald actions's values", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnActionDelete_Click(object sender, EventArgs e)
        {
            string actDesc = lboActions.SelectedValue.ToString();
            ActionBase oldAct = _sb.Actions[actDesc];

            if (oldAct.Fired != null)
            {
                foreach (var item in oldAct.Fired.Values)
                {
                    item.Action = null;
                }
            }
            _sb.Actions.Remove(actDesc);
            SetDisplaySettingsFromWiimoteActions();
            grpEditAction.Enabled = false;
            btnActionDelete.Enabled = false;
            btnActionSave.Enabled = false;
            btnSavePresenterSettings.Enabled = true;
        }

        private void btnActionNew_Click(object sender, EventArgs e)
        {
            isNewAction = true;
            txtDescription.Text = "Insert description here";
            txtParam.Text = "Insert parameter's value here";
            grpEditAction.Enabled = true;
            btnActionDelete.Enabled = true;
            btnActionSave.Enabled = true;
        }

        private void btnParam_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult res = fd.ShowDialog();
            if (res == DialogResult.OK)
            {
                txtParam.Text = fd.FileName;
            }
        }

        private void btnSavePresenterSettings_Click(object sender, EventArgs e)
        {
            SetWiimoteActionsFromDisplay();
            btnSavePresenterSettings.Enabled = false;
        }

        private void cboChangeActionsEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSavePresenterSettings.Enabled = true;
        }

        private void cboPresenterFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadPresenterFiles)
            {
                string presName = cboPresenterFileNames.SelectedItem.ToString();
                _sb.Actions = _sb.ActionsSettingsFiles[presName];
                _sb.ActionsSettingsFileName = presName;
                SetDisplaySettingsFromWiimoteActions();
                frmToolbar.UpdateFormContent();
            }
        }

        private void btnLoadPresenterSettings_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Presenter settings (*.xml)|*.xml";
            DialogResult risp = fd.ShowDialog();
            if (risp == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(fd.FileName);
                string fileName = fi.Name;
                string path = Path.Combine(Global.AppFolderPath, fileName);
                if (_sb.ActionsSettingsFiles.ContainsKey(fileName) ||
                    path == Global.AppConfigFileName)
                {
                    MessageBox.Show("You cannot load a file with an existing name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ActionCollection acts = ActionCollection.LoadFromXML(fd.FileName);
                    acts.SaveToXML(path);
                    _sb.ActionsSettingsFiles.LoadAllSettings();
                    SetPresenterFileDisplay();
                }
            }
        }

        private void btnToolbar_Click(object sender, EventArgs e)
        {
            if (isToolbarActive)
            {
                frmToolbar.Hide();
                btnToolbar.Text = "Show toolbar";
            }
            else
            {
                frmToolbar.SB = _sb;
                frmToolbar.Show();
                btnToolbar.Text = "Hide toolbar";
            }
            isToolbarActive = !isToolbarActive;
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            Show();
            this.Size = formSizeNormal;
            Activate();
        }

        private void chkStartGUI_CheckedChanged(object sender, EventArgs e)
        {
            _sb.StartGui = !chkStartGUI.Checked;
        }

        private void chkMinimizeTray_CheckedChanged(object sender, EventArgs e)
        {
            _sb.MinimizeOnTray = chkMinimizeTray.Checked;
        }

        private void chkAutoConnectTaggedDevice_CheckedChanged(object sender, EventArgs e)
        {
            _sb.AutoConnectTaggedDevices = chkAutoConnectTaggedDevice.Checked;
            txtTagName.Enabled = _sb.AutoConnectTaggedDevices;
        }

        private void txtTagName_TextChanged(object sender, EventArgs e)
        {
            _sb.TagName = txtTagName.Text;
        }

        private void udSmoothingAmount_ValueChanged(object sender, EventArgs e)
        {
            if (udSmoothingAmount.Value <= udSmoothingBufferSize.Value)
            {
                SharpBoard.Smoothing.Amount = (int)udSmoothingAmount.Value;
            }
            else
            {
                udSmoothingAmount.Value = udSmoothingBufferSize.Value;
            }
        }

        private void udSmoothingBufferSize_ValueChanged(object sender, EventArgs e)
        {
            if (udSmoothingBufferSize.Value > udSmoothingAmount.Value)
            {
                SharpBoard.Smoothing.BufferSize = (int)udSmoothingBufferSize.Value;
            }
            else
            {
                udSmoothingAmount.Value = udSmoothingBufferSize.Value;
            }
        }

        private void chkAllowNewDeviceConnection_CheckedChanged(object sender, EventArgs e)
        {
            _sb.AllowNewDeviceConnection = chkAllowNewDeviceConnection.Checked;
        }

        #endregion

        private void udRightClick_ValueChanged(object sender, EventArgs e)
        {
            SharpBoard.RightClickTime = (int)udRightClick.Value;
        }

        private void rbuLevel_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbu = (RadioButton)sender;
            string cntName = rbu.Name.Replace("rbuIR", "");
            irMode = (CameraSensitivity)Enum.Parse(typeof(CameraSensitivity), cntName);
        }

        private void btnIRApply_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name.Contains("All"))
            {
                foreach (var dev in _sb.WiiDevices.Values)
                {
                    dev.Device.SetReportingMode(ReportingMode.ButtonsAccelerometer36Ir, irMode);
                    dev.IRSensitivity = irMode;
                }
            }
            else
            {
                if (_sb.SelectedWiiDeviceID != null)
                {
                    _sb.WiiDevices[_sb.SelectedWiiDeviceID].Device.SetReportingMode(ReportingMode.ButtonsAccelerometer36Ir, irMode);
                    _sb.WiiDevices[_sb.SelectedWiiDeviceID].IRSensitivity = irMode;
                }
            }
        }

    }
}
