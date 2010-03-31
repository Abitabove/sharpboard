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
using SharpBoardLibrary.Keyboard;

namespace SharpBoardLibrary.Display
{
    public abstract class DisplayDeviceProvider : IDisplayDeviceProvider
    {
        //public int SmoothingAmount
        //{
        //    get { return _sb.SelectedWiiDevice.Smoothing.SmoothingAmount; }
        //}

        //public bool EnableSmoothing
        //{
        //    get { return _sb.SelectedWiiDevice.Smoothing.EnableSmoothing; }
        //}

        SharpBoard _sb;
        public SharpBoard SB
        {
            get { return _sb; }
            set { _sb = value; }
        }

        public abstract void FakeMove(int x, int y);
        public abstract void FakeButton(MouseButton mouse, MouseAction action);
        //public abstract void FakeKeys(string keys);

        public virtual void SendKeys(string keys)
        {
            KeyBoardDevice.SendWait(keys);
            // ((IDisplayDeviceProvider)this).FakeKeys(keys);
        }

        //public virtual void SendKey(byte key, KeyAction action)
        //{
        //    ((IDisplayDeviceProvider)this).FakeKey(key, action);
        //}
        //public virtual void SendKey(KeyCodeCollection keys, KeyAction action)
        //{
        //    foreach (var key in keys)
        //    {
        //        SendKey(key, action);
        //    }
        //}

        public virtual void SetCursor(int x, int y)
        {
            //bool isWin = this is Win32.Win32DeviceProvider;
            ((IDisplayDeviceProvider)this).FakeMove(x, y);
        }

        public virtual void SetMouse(MouseButton mouse, MouseAction action)
        {
            ((IDisplayDeviceProvider)this).FakeButton(mouse, action);
        }

        public virtual void SetCursorAndMouse(int x, int y, MouseButton mouse, MouseAction action)
        {
            SetCursor(x, y);
            SetMouse(mouse, action);
        }

        public abstract WorkingArea GetScreenSize();

        public abstract void RefreshIRMonitor(object picMonitor);

        //public void GetSmoothingCoords(bool isWin, out int x, out int y, float warpedX, float warpedY, int screenWidth, int screenHeight)

        public void GetSmoothingCoords(out int x, out int y, float warpedX, float warpedY, SmoothingData smoothing)
        {
            GetSmoothingCoords(out x, out y, warpedX, warpedY, SB.ScreenSize.Width, SB.ScreenSize.Height, smoothing);
        }

        public void GetSmoothingCoords(out int x, out int y, float warpedX, float warpedY, int screenWidth, int screenHeight, SmoothingData smoothing)
        {
            float scaleFactor = 65535.0f;
            if (!(this is Win32.Win32DeviceProvider))
            {
                scaleFactor = 1.0f;
                screenHeight = 1;
                screenWidth = 1;
            }

            if (SharpBoard.Smoothing.IsEnabled)
            {
                CoordinateF s = smoothing.GetSmoothedCursor(SharpBoard.Smoothing.Amount);
                x = (int)(s.X * scaleFactor / screenWidth);
                y = (int)(s.Y * scaleFactor / screenHeight);
            }
            else
            {
                x = (int)(warpedX * scaleFactor / screenWidth);
                y = (int)(warpedY * scaleFactor / screenHeight);
            }
        }

    }
}
