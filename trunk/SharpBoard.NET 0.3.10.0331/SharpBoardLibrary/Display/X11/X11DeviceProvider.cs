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
using Gtk;
using Gdk;
using System.Diagnostics;

namespace SharpBoardLibrary.Display.X11
{
    public class X11DeviceProvider : DisplayDeviceProvider, IDisplayDeviceProvider
    {
        //static Screen s = Gdk.Display.Default.GetScreen(0);
        //static Gdk.Display display = Gdk.Display.Default;

        #region X11 Device Specific methods

        public override void FakeMove(int x, int y)
        {
            IntPtr display = X11.XOpenDisplay(0);
            //int screenNo = X11.XDefaultScreen(display);
            IntPtr root = X11.XDefaultRootWindow(display);
            X11.XTestFakeMotionEvent(display, -1, x, y, 0);
            X11.XWarpPointer(display, IntPtr.Zero, root, 0, 0, 0, 0, x, y);
            X11.XFlush(display);
            X11.XCloseDisplay(display);
        }

        public override void FakeButton(MouseButton button, MouseAction action)
        {
            bool pressed = (action == MouseAction.Down);
            IntPtr display = X11.XOpenDisplay(0);
            X11.XTestFakeButtonEvent(display, (uint)button, pressed, 1);
            X11.XFlush(display);
            X11.XCloseDisplay(display);
        }

        // Non buono
        //public override void FakeKey(byte key, KeyAction action)
        //{
        //    bool pressed = (action == KeyAction.Down);
        //    IntPtr display = X11.XOpenDisplay(0);
        //    X11.XTestFakeKeyEvent(display, (uint)key, pressed, 1);
        //    X11.XFlush(display);
        //    X11.XTestFakeKeyEvent(display, (uint)key, !pressed, 1);
        //    X11.XFlush(display);
        //    X11.XCloseDisplay(display);
        //}

        //public override void FakeKey(byte key, KeyAction action)
        //{
        //    bool pressed = (action == KeyAction.Down);
        //    IntPtr display = X11.XOpenDisplay(0);
        //    Console.WriteLine("prima {0}",key);
        //    key = (byte)X11.XKeysymToKeycode(display, key);
        //    X11.XTestFakeKeyEvent(display, (uint)key, pressed, 1);
        //    X11.XFlush(display);
        //    X11.XTestFakeKeyEvent(display, (uint)key, !pressed, 1);
        //    X11.XFlush(display);
        //    X11.XCloseDisplay(display);
        //    Console.WriteLine(key);
        //}

        public override WorkingArea GetScreenSize()
        {
            Screen screenCtl = Gdk.Display.Default.DefaultScreen;
            WorkingArea screenSize =
                new WorkingArea(screenCtl.Width,
                                screenCtl.Height);
            return screenSize;
        }

        public override void RefreshIRMonitor(object picMonitor)
        {
            // TODO: for test now use windows forms
            System.Windows.Forms.PictureBox pic = (System.Windows.Forms.PictureBox)picMonitor;
            pic.Invalidate();
        }

        #endregion
    }
}
