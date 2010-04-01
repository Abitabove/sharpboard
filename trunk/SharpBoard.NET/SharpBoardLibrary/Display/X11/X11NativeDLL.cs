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
using System.Runtime.InteropServices;

namespace SharpBoardLibrary.Display.X11
{
    public class X11
    {
        [DllImport("libX11")]
        internal static extern IntPtr XDefaultVisual(IntPtr display,
                                                      int screen_number);

        [DllImport("libX11")]
        internal static extern IntPtr XDefaultRootWindow(IntPtr display);

        [DllImport("libX11")]
        internal static extern int XDefaultScreen(IntPtr display);

        [DllImport("libX11")]
        internal static extern IntPtr XCreateSimpleWindow(IntPtr display,
                                                    IntPtr window,
                                                    int x,
                                                    int y,
                                                    uint width,
                                                    uint height,
                                                    ulong border_width,
                                                    ulong border,
                                                    ulong background);
        [DllImport("libX11")]
        internal static extern int XMapWindow(IntPtr display,
                                               IntPtr window);

        [DllImport("libX11")]
        internal static extern int XClearWindow(IntPtr display,
                                                 IntPtr window);

        [DllImport("libX11")]
        internal static extern IntPtr XOpenDisplay(int display_name);

        [DllImport("libX11")]
        internal static extern IntPtr XScreenOfDisplay(IntPtr display,
                                                        int screen_number);

        [DllImport("libX11")]
        internal static extern int XCloseDisplay(IntPtr display);

        [DllImport("libX11")]
        internal static extern ulong XBlackPixel(IntPtr display,
                                                  int screen);

        [DllImport("libX11")]
        internal static extern ulong XWhitePixel(IntPtr display,
                                                  int screen);

        [DllImport("libX11")]
        internal static extern int XNextEvent(IntPtr display,
                                               IntPtr event_return);

        [DllImport("libX11")]
        internal static extern void XSync(IntPtr display,
                                                  bool discarder);

        [DllImport("libX11")]
        internal static extern void XFlush(IntPtr display);

	
        [DllImport("libX11")]
        internal static extern void XWarpPointer(IntPtr display,
                                                 IntPtr windowSrc,
		                                         IntPtr windowDst,
		                                         int srcX,
		                                         int srcY,
		                                         uint srcWidth,
		                                         uint srcHeight,
		                                         int destX,
		                                         int destY);

        [DllImport("libX11")]
        internal static extern int XKeysymToKeycode(IntPtr display, IntPtr keysym);
        internal static int XKeysymToKeycode(IntPtr display, int keysym)
        {
            return XKeysymToKeycode(display, (IntPtr)keysym);
        }
     

        [DllImport("libXtst")]
        internal static extern void XTestFakeKeyEvent(IntPtr display,
                                                    uint keycode,
                                                    bool is_press,
                                                    ulong delay);

        [DllImport("libXtst")]
        internal static extern void XTestFakeButtonEvent(IntPtr display,
                                                    uint button,
                                                    bool is_press,
                                                    ulong delay);

        [DllImport("libXtst")]
        internal static extern int XTestFakeMotionEvent(IntPtr display,
                                                    int screen,
                                                    int x,
                                                    int y,
                                                    ulong delay);

        [DllImport("libXtst")]
        internal static extern int XTestFakeRelativeMotionEvent(IntPtr display,
                                                    int screen,
                                                    int x,
                                                    int y,
                                                    ulong delay);
        [DllImport("libXtst")]
        internal static extern bool XTestQueryExtension(IntPtr display,
                                                        ref int event_base, 
                                                        ref int error_base, 
                                                        ref int major_version, 
                                                        ref int minor_version);
    }

}
