//    Copyright 2010 SharpBoard Library authors
//
//    This file is part of SharpBoard Library.
//
//    SharpBoard Library is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    SharpBoard Library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with SharpBoard Library.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Threading;
using System.Reflection;

using WiiDeviceLibrary;
using SharpBoardLibrary.Display;
using SharpBoardLibrary.Display.Win32;
using SharpBoardLibrary.Display.X11;
using SharpBoardLibrary.Common;

namespace SharpBoardLibrary
{
    public class SharpBoard : Loggable
    {
        //LogWriter _log;
        bool _okToStart;

        IDisplayDeviceProvider _videoDevice;
        public IDisplayDeviceProvider VideoDevice
        {
            get { return _videoDevice; }
            set { _videoDevice = value; }
        }

        public object picIRBeacons { get; set; }

        public WorkingArea ScreenSize { get; set; }

        public bool AutoConnectTaggedDevices { get; set; }
        public string TagName { get; set; }
        public bool AllowNewDeviceConnection { get; set; }

        public static int RightClickTime { get; set; }

        public bool StartGui { get; set; }
        public bool MinimizeOnTray { get; set; }

        public static ToolBarSettings ToolBarSetting { get; set; }
        public static ToolBarButtonCollection ToolBarButtons { get; set; }
        public static SmoothingConfig Smoothing { get; set; }

        public IDeviceProvider BTProvider { get; set; }
        public BTProvidersCollection BTProviders;
        public FactoryType BTProviderDiscovery { get; set; }

        //int _selectedWiiNumber;
        //public int SelectedWiiNumber
        //{
        //    get { return _selectedWiiNumber; }
        //    set { _selectedWiiNumber = value; }
        //}

        string _selectedWiiDeviceID = "n/a";
        public string SelectedWiiDeviceID
        {
            get
            {
                return _selectedWiiDeviceID;
            }
            set
            {
                _selectedWiiDeviceID = null;
                _selectedWiiDevice = null;
                if (value != null && WiiDevices != null)
                {
                    if (WiiDevices.ContainsKey(value))
                    {
                        _selectedWiiDeviceID = value;
                        _selectedWiiDevice = WiiDevices[value];
                    }
                }
            }
        }

        WiiDevice _selectedWiiDevice;
        public WiiDevice SelectedWiiDevice
        {
            get
            {
                return _selectedWiiDevice;
            }
        }

        public WiiDeviceCollection WiiDevices { get; set; }
        public WiiDeviceCollection WiiDevicesStoredInfo { get; set; }

        public string ActionsSettingsFileName { get; set; }
        public ActionsSettings ActionsSettingsFiles { get; set; }
        public ActionCollection Actions { get; set; }

        ActionCollection ActionsDefaults { get; set; }

        //public FiredByCollection PresenterEventsDefaults
        //{
        //    get
        //    {
        //        return ActionsDefaults.PresenterEvents;
        //    }
        //}

        public bool OkToStart
        {
            get { return _okToStart; }
            set { _okToStart = value; }
        }

        public SharpBoard()
        {
            InitializeWhiteBoardData();
            LoadConfigData();
        }

        private WiiDeviceCollection AllDevices()
        {
            WiiDeviceCollection currentDevice = WiiDevices;
            foreach (var item in WiiDevicesStoredInfo)
            {
                if (!currentDevice.ContainsKey(item.Key))
                {
                    currentDevice.Add(item.Key, item.Value);
                }
            }
            return currentDevice;
        }

        public XElement XElement
        {
            get
            {
                XElement element =
                    new XElement("SharpBoard.NET",
                    //                    new XElement(ScreenSize.XElement),
                    //                    new XElement(Smoothing.XElement),
                    new XElement("AppSettings",
                        new XAttribute("btProviderDiscovery", BTProviderDiscovery),
                        new XAttribute("startGui", StartGui),
                        new XAttribute("minimizeOnTray", MinimizeOnTray),
                        new XAttribute("rightClickTime", RightClickTime),
                        new XAttribute("actionsSettings", ActionsSettingsFileName)),
                    new XElement(Smoothing.XElement),
                    new XElement(ToolBarSetting.XElement),
                    new XElement(AllDevices().XElement)
                    );

                element.Element("Devices").SetAttributeValue("autoConnectTaggedDevices", AutoConnectTaggedDevices);
                element.Element("Devices").SetAttributeValue("tagName", TagName);
                element.Element("Devices").SetAttributeValue("allowNewDeviceConnection", AllowNewDeviceConnection);
                return element;
            }
        }

        public SharpBoard(XElement element)
        {
            InitializeWhiteBoardData();
            string PresenterDefaultFileName = Global.AppPresenterDefaultFileName;

            //ScreenSize = new WorkingArea(element.Element("ScreenSize"));
            //Smoothing = new SmoothingData(element.Element("Smoothing"));
            XElement appSettingsXml = element.Element("AppSettings");
            XElement devicesXml = element.Element("Devices");

            BTProviderDiscovery = (FactoryType)Enum.Parse(typeof(FactoryType), appSettingsXml.Attribute("btProviderDiscovery").Value);
            StartGui = appSettingsXml.GetBoolAttribute("startGui", true, true);
            MinimizeOnTray = appSettingsXml.GetBoolAttribute("minimizeOnTray", true, false);
            RightClickTime = appSettingsXml.GetIntAttribute("rightClickTime", true, 1000);

            WiiDevicesStoredInfo = new WiiDeviceCollection(devicesXml);
            AutoConnectTaggedDevices = devicesXml.GetBoolAttribute("autoConnectTaggedDevices", true, false);
            TagName = devicesXml.GetAttribute("tagName", true, "");
            AllowNewDeviceConnection = devicesXml.GetBoolAttribute("allowNewDeviceConnection", true, true);

            //AutoConnectTaggedDevices = appSettings.GetBoolAttribute("autoConnectTaggedDevices", true, false);
            //TagName = appSettings.GetAttribute("tagName", true, "");

            Smoothing = new SmoothingConfig(element.Element("Smoothing"));

            ToolBarSetting = new ToolBarSettings(element.Element("ToolBarSettings"));
            
            ActionsSettingsFiles.LoadAllSettings();
            ActionsSettingsFileName = appSettingsXml.GetAttribute("actionsSettings", true, "");

            if (ActionsSettingsFileName != "")
            {
                if (ActionsSettingsFiles.ContainsKey(ActionsSettingsFileName))
                {
                    PresenterDefaultFileName = Path.Combine(Global.AppFolderPath, ActionsSettingsFileName);
                }
            }
            else
            {
                ActionsSettingsFileName = "PresenterSettings.xml";
            }
            Actions = ActionCollection.LoadFromXML(PresenterDefaultFileName);
        }

        public void SaveConfigData()
        {
            if (!Global.Config.SaveSBConfig(this))
            {
                AddLog(LogLevel.FatalError, "Unable to save Configuration file");
                Global.Log.Close();
            }
            else
            {
                AddLog(LogLevel.Info, "Saved configuration file");
            }

        }

        private void LoadConfigData()
        {
            if (!Global.Config.LoadSBConfig(this))
            {
                AddLog(LogLevel.FatalError, "Unable to load Configuration file");
                //AddLog("Unable to start SharpBoard.NET");
                // Global.Log.Close();
                //_okToStart = false;
            }
            else
            {
                //_okToStart = true;
                AddLog(LogLevel.Info, "Config file loaded succesfully");
            }
        }

        private void InitializeWhiteBoardData()
        {
            StartGui = true;
            ActionsSettingsFileName = "";
            ActionsSettingsFiles = new ActionsSettings();
            BTProviderDiscovery = FactoryType.All;
            BTProviders = new BTProvidersCollection();
            WiiDevices = new WiiDeviceCollection();
            WiiDevicesStoredInfo = new WiiDeviceCollection();
            AllowNewDeviceConnection = true;
            TagName = "";

            RightClickTime = 1000;
            Smoothing = new SmoothingConfig() { IsEnabled = true, Amount = 8, BufferSize = 50 };
            
            ToolBarSetting = new ToolBarSettings() { IsPortrait = true, X = 600, Y = 40 };

            if (!Directory.Exists(Global.AppFolderPath))
                Directory.CreateDirectory(Global.AppFolderPath);

            RegisterLogName("SharpBoard");

            Global.Log.UpdateLogFile();

            Assembly _assembly;
            StreamReader _textStreamReader;
            _assembly = Assembly.GetExecutingAssembly();
            _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("SharpBoardLibrary.DefaultPresenterSettings.xml"));
            string xml = _textStreamReader.ReadToEnd();
            _textStreamReader.Close();
            ActionsDefaults = ActionCollection.LoadFromXMLString(xml);
            if (!File.Exists(Global.AppPresenterDefaultFileName))
                ActionsDefaults.SaveToXML(Global.AppPresenterDefaultFileName);
            Actions = ActionsDefaults;
        }

        public void InitializedSmoothingData(WiiDevice wii, int smoothing)
        {
            wii.Smoothing.SmoothingAmount = smoothing;
            for (int i = 0; i < wii.Smoothing.SmoothingBuffer.Length; i++)
                wii.Smoothing.SmoothingBuffer[i] = new CoordinateF();
        }

        public void InitializeDisplayDevice(object screen)
        {
            _videoDevice = DisplayDeviceFactory.CreateVideoDevice();
            ScreenSize = (WorkingArea)_videoDevice.GetScreenSize();
            _videoDevice.SB = this;
        }

        public void InitializeBTDevices()
        {
            foreach (IDeviceProvider supportedProvider in DeviceProviderRegistry.CreateAllSupportedDeviceProviders())
            {
                string providerType = supportedProvider.GetBTProviderType();
                BTProviders.Add(providerType, supportedProvider);
            }
        }

        public void InitializeWiiDevice(string deviceID, int ledNumber)
        {
            WiimoteLeds led = (WiimoteLeds)(int)(Math.Pow(2, ledNumber) / 2);
            WiiDevices[deviceID].Device.Leds = led;
            WiiDevices[deviceID].Device.IsRumbling = true;
            Thread.Sleep(500);
            WiiDevices[deviceID].Device.IsRumbling = false;
            WiiDevices[deviceID].Device.SetReportingMode(ReportingMode.ButtonsAccelerometer36Ir);
            //WiiDevices[deviceID].LoadStoredData(WiiDevicesStoredInfo);
        }

        public void InitializeWiiDevice(string deviceID)
        {
            InitializeWiiDevice(deviceID, 1);
        }

        public MouseButton ExecuteAction(ActionBase act)
        {
            MouseButton presenterMouseButton = MouseButton.None;
            switch (act.Type)
            {
                case ActionType.Keys:
                    ActionKeys actk = act as ActionKeys;
                    string key = actk.Keys;
                    VideoDevice.SendKeys(key);
                    break;
                case ActionType.Mouse:
                    ActionMouse actm = act as ActionMouse;
                    presenterMouseButton = actm.Mouse;
                    break;
                case ActionType.Toogle:
                    ActionToogle actT = act as ActionToogle;
                    switch (actT.Feature)
                    {
                        case ToogleFeature.CursorOnly:
                            switch (actT.Action)
                            {
                                case ToogleAction.On:
                                    SelectedWiiDevice.Behaviour = MouseBehaviour.CursorOnly;
                                    break;
                                case ToogleAction.Off:
                                    SelectedWiiDevice.Behaviour = MouseBehaviour.Normal;
                                    break;
                                case ToogleAction.Toogle:
                                    switch (SelectedWiiDevice.Behaviour)
                                    {
                                        case MouseBehaviour.Normal:
                                            SelectedWiiDevice.Behaviour = MouseBehaviour.CursorOnly;
                                            break;
                                        case MouseBehaviour.CursorOnly:
                                            SelectedWiiDevice.Behaviour = MouseBehaviour.Normal;
                                            break;
                                        case MouseBehaviour.LeftButton:
                                        case MouseBehaviour.RightButton:
                                        case MouseBehaviour.DoubleClick:
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case ActionType.Process:
                    ActionProcess actp = act as ActionProcess;
                    Process.Start(actp.CmdLine);
                    break;
                default:
                    break;
            }
            return presenterMouseButton;
        }

        void Wiimote_Updated(object sender, EventArgs e)
        {
            throw new NotImplementedException("TODO !!!");
        }

    }
}
