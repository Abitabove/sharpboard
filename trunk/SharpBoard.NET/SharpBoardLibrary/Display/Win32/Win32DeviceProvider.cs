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
using System.Windows.Forms;
using System.Runtime.InteropServices;//for firing keyboard and mouse events (optional)


namespace SharpBoardLibrary.Display.Win32
{
    public class Win32DeviceProvider : DisplayDeviceProvider, IDisplayDeviceProvider
    {
        #region Win32 Device Specific methods
        
        public override void FakeMove(int x, int y)
        {
            Win32.INPUT[] buffer = new Win32.INPUT[1];
            buffer[0].type = Win32.INPUT_MOUSE;
            buffer[0].mi.dx = (int)x;
            buffer[0].mi.dy = (int)y;
            buffer[0].mi.mouseData = 0;
            buffer[0].mi.dwFlags = Win32.MOUSEEVENTF_ABSOLUTE | Win32.MOUSEEVENTF_MOVE;
            buffer[0].mi.time = 0;
            buffer[0].mi.dwExtraInfo = (IntPtr)0;
            Win32.SendInput(1, buffer, Marshal.SizeOf(buffer[0]));
        }

        public override void FakeButton(MouseButton button, MouseAction action)
        {
            // Console.WriteLine("{0}-{1}", button.ToString(), action.ToString());
            uint mouseButton = 0;
            if (button == MouseButton.Left)
            {
                if (action == MouseAction.Down)
                    mouseButton = Win32.MOUSEEVENTF_LEFTDOWN;
                else
                    mouseButton = Win32.MOUSEEVENTF_LEFTUP;
            }

            if (button == MouseButton.Right)
            {
                if (action == MouseAction.Down)
                    mouseButton = Win32.MOUSEEVENTF_RIGHTDOWN;
                else
                    mouseButton = Win32.MOUSEEVENTF_RIGHTUP;
            }

            Win32.INPUT[] buffer = new Win32.INPUT[2];
            buffer[0].type = Win32.INPUT_MOUSE;
            buffer[0].mi.dx = 0;
            buffer[0].mi.dy = 0;
            buffer[0].mi.mouseData = 0;
            buffer[0].mi.dwFlags = mouseButton;
            buffer[0].mi.time = 1;
            buffer[0].mi.dwExtraInfo = (IntPtr)0;

            buffer[1].type = Win32.INPUT_MOUSE;
            buffer[1].mi.dx = 0;
            buffer[1].mi.dy = 0;
            buffer[1].mi.mouseData = 0;
            buffer[1].mi.dwFlags = Win32.MOUSEEVENTF_MOVE;
            buffer[1].mi.time = 0;
            buffer[1].mi.dwExtraInfo = (IntPtr)0;

            Win32.SendInput(2, buffer, Marshal.SizeOf(buffer[0]));

        }

        //public override void FakeKeys(string keys)
        //{
        //    System.Windows.Forms.SendKeys.SendWait(keys);
        //}

        public override WorkingArea GetScreenSize()
        {
            WorkingArea screenSize =
                new WorkingArea(Screen.PrimaryScreen.Bounds.Width,
                                Screen.PrimaryScreen.Bounds.Height);
            screenSize.Top = Screen.PrimaryScreen.Bounds.Top;
            screenSize.Left = Screen.PrimaryScreen.Bounds.Left;
            //WorkingArea screenSize =
            //    new WorkingArea(Screen.PrimaryScreen.WorkingArea.Width,
            //                    Screen.PrimaryScreen.WorkingArea.Height);
            //screenSize.Top = Screen.PrimaryScreen.WorkingArea.Top;
            //screenSize.Left = Screen.PrimaryScreen.WorkingArea.Left;

            return screenSize;
        }

        public override void RefreshIRMonitor(object picMonitor)
        {
            PictureBox pic = (PictureBox)picMonitor;
            pic.Invalidate();
        }

        #endregion
    }
}
