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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiiDeviceLibrary;
using System.Xml.Linq;
using SharpBoardLibrary.Common;

namespace SharpBoardLibrary
{
    public class WiiDevice
    {
        string _deviceID;
        public string DeviceID
        {
            get
            {
                string tmp = _deviceID;
                if (Device != null)
                {
                    _deviceID = Device.DeviceInfo.GetID();
                    tmp = _deviceID;
                }
                return tmp;
            }
        }
        public string TagName { get; set; }
        public IWiimote Device { get; set; }
        public WiimoteMode Mode { get; set; }
        public MouseBehaviour Behaviour { get; set; }
        public Warper Warper { get; set; }
        // public int RightClickTime { get; set; }

        public CameraSensitivity IRSensitivity { get; set; }

        public SmoothingData Smoothing {get; set;}
        
        //public SharpBoard SB { get; set; }

        public WiiDevice(IWiimote dev, WiimoteMode use, MouseBehaviour be)
        {
            Device = dev;
            Mode = use;
            Behaviour = be;
            Warper = new Warper();
            IRSensitivity = CameraSensitivity.Max;
            Smoothing = new SmoothingData(SharpBoard.Smoothing.BufferSize);

        }

        public WiiDevice(IWiimote dev, WiimoteMode use)
            : this(dev, use, MouseBehaviour.Normal)
        {
            if ((Mode & WiimoteMode.Presenter) == WiimoteMode.Presenter)
                Behaviour = MouseBehaviour.CursorOnly;
        }

        public WiiDevice()
            : this(null, WiimoteMode.WhiteBoard, MouseBehaviour.Normal)
        { }

        public XElement XElement
        {
            get
            {
                return new XElement("WiiDevice",
                    new XAttribute("deviceID", DeviceID),
                    new XAttribute("tagName",TagName),
                    new XAttribute("usedAs", Mode.ToString()),
                    new XAttribute("behaviour", Behaviour),
                    //new XAttribute("rightClickTime", RightClickTime),
                    new XAttribute("irSensitivity", IRSensitivity),
                    new XElement("CalibrationData", Warper.ToString()));
            }
        }

        public WiiDevice(XElement element)
        {
            _deviceID = element.Attribute("deviceID").Value;
            TagName = element.GetAttribute("tagName", true, "");
            Mode = (WiimoteMode)Enum.Parse(typeof(WiimoteMode), element.Attribute("usedAs").Value);
            Behaviour = (MouseBehaviour)Enum.Parse(typeof(MouseBehaviour), element.Attribute("behaviour").Value);
            //RightClickTime = int.Parse(element.Attribute("rightClickTime").Value);
            IRSensitivity = (CameraSensitivity)Enum.Parse(typeof(CameraSensitivity), element.Attribute("irSensitivity").Value);

            Warper w;
            bool okWarper = Warper.TryParse(element.Element("CalibrationData").Value, out w);
            if (okWarper)
            {
                Warper = w;
                Warper.ComputeWarp();
            }

        }

        public void LoadStoredData(WiiDeviceCollection wiiDevicesStoredInfo)
        {
            if (wiiDevicesStoredInfo.ContainsKey(DeviceID))
            {
                TagName = wiiDevicesStoredInfo[DeviceID].TagName;
                Mode = wiiDevicesStoredInfo[DeviceID].Mode;
                Behaviour = wiiDevicesStoredInfo[DeviceID].Behaviour;
                Warper = wiiDevicesStoredInfo[DeviceID].Warper;
                //RightClickTime = wiiDevicesStoredInfo[DeviceID].RightClickTime;
                IRSensitivity = wiiDevicesStoredInfo[DeviceID].IRSensitivity;
            }

        }

        public void InitializedSmoothingData(int smoothing)
        {
            Smoothing.SmoothingAmount = smoothing;
            for (int i = 0; i < Smoothing.SmoothingBuffer.Length; i++)
                Smoothing.SmoothingBuffer[i] = new CoordinateF();
        }
    }
}
