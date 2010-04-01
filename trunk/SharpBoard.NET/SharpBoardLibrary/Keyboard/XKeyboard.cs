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
using System.Collections;
using SharpBoardLibrary.Display.X11;

namespace SharpBoardLibrary.Keyboard
{
    public class XKeyboard
    {
        public static int SendInput(IntPtr handle, Queue keys)
        {
            IntPtr display = X11.XOpenDisplay(0);
            int count = keys.Count;
            while (keys.Count > 0)
            {
                MSG msg = (MSG)keys.Dequeue();
                bool pressed = (msg.message == Msg.WM_KEYDOWN ? true : false);
                int key = Keyboard.ToKeycode(display, msg.wParam.ToInt32());
                X11.XTestFakeKeyEvent(display, (uint)key, pressed, 0);
                X11.XFlush(display);
            }
            X11.XCloseDisplay(display);
            return count;
        }

    }

    static class Keyboard
    {
        internal enum XKeySym : uint
        {
            XK_BackSpace = 0xFF08,
            XK_Tab = 0xFF09,
            XK_Clear = 0xFF0B,
            XK_Return = 0xFF0D,
            XK_Home = 0xFF50,
            XK_Left = 0xFF51,
            XK_Up = 0xFF52,
            XK_Right = 0xFF53,
            XK_Down = 0xFF54,
            XK_Page_Up = 0xFF55,
            XK_Page_Down = 0xFF56,
            XK_End = 0xFF57,
            XK_Begin = 0xFF58,
            XK_Menu = 0xFF67,
            XK_Shift_L = 0xFFE1,
            XK_Shift_R = 0xFFE2,
            XK_Control_L = 0xFFE3,
            XK_Control_R = 0xFFE4,
            XK_Caps_Lock = 0xFFE5,
            XK_Shift_Lock = 0xFFE6,
            XK_Meta_L = 0xFFE7,
            XK_Meta_R = 0xFFE8,
            XK_Alt_L = 0xFFE9,
            XK_Alt_R = 0xFFEA,
            XK_Super_L = 0xFFEB,
            XK_Super_R = 0xFFEC,
            XK_Hyper_L = 0xFFED,
            XK_Hyper_R = 0xFFEE,

            // change by delfo
            XK_Delete = 0xFFFF,
            XK_Escape = 0xFF1B,
            XK_Help = 0xFF6A,
            XK_Insert = 0xFF63,
            XK_F1 = 0xFFBE,
            XK_F2 = 0xFFBF,
            XK_F3 = 0xFFC0,
            XK_F4 = 0xFFC1,
            XK_F5 = 0xFFC2,
            XK_F6 = 0xFFC3,
            XK_F7 = 0xFFC4,
            XK_F8 = 0xFFC5,
            XK_F9 = 0xFFC6,
            XK_F10 = 0xFFC7,
            XK_F11 = 0xFFC8,
            XK_F12 = 0xFFC9,
        }

        private readonly static int[] nonchar_vkey_key = new int[]
		{
			0, 0, 0, 0, 0,		/* 00-04 */ 
			0, 0, 0, (int)XKeySym.XK_BackSpace, (int)XKeySym.XK_Tab,		/* 05-09 */
			0, 0, (int)XKeySym.XK_Clear, (int)XKeySym.XK_Return,	0, 0,	/* 0A-0F */
			//(int)XKeySym.XK_Shift_L, (int)XKeySym.XK_Control_L, (int)XKeySym.XK_Menu, 0, (int)XKeySym.XK_Caps_Lock,		/* 10-14 */ 
			(int)XKeySym.XK_Shift_L, (int)XKeySym.XK_Control_L, (int)XKeySym.XK_Alt_L, 0, (int)XKeySym.XK_Caps_Lock,		/* 10-14 */ 
			0, 0, 0, 0, 0,		/* 15-19 */
			0, (int)XKeySym.XK_Escape, 0, 0,	0, 0,	/* 1A-1F */
			0, (int)XKeySym.XK_Page_Up, (int)XKeySym.XK_Page_Down, (int)XKeySym.XK_End, (int)XKeySym.XK_Home,		/* 20-24 */ 
			(int)XKeySym.XK_Left, (int)XKeySym.XK_Up, (int)XKeySym.XK_Right, (int)XKeySym.XK_Down, 0,		/* 25-29 */
			0, 0, 0, (int)XKeySym.XK_Insert,(int)XKeySym.XK_Delete, (int)XKeySym.XK_Help,	/* 2A-2F */
			0, 0, 0, 0, 0,		/* 30-34 */ 
			0, 0, 0, 0, 0,		/* 35-39 */
			0, 0, 0, 0,	0, 0,	/* 3A-3F */
			0, 0, 0, 0, 0,		/* 40-44 */ 
			0, 0, 0, 0, 0,		/* 45-49 */
			0, 0, 0, 0,	0, 0,	/* 4A-4F */
			0, 0, 0, 0, 0,		/* 50-54 */ 
			0, 0, 0, 0, 0,		/* 55-59 */
			0, (int)XKeySym.XK_Meta_L, (int)XKeySym.XK_Meta_R, 0,	0, 0,	/* 5A-5F */
			0, 0, 0, 0, 0,		/* 60-64 */ 
			0, 0, 0, 0, 0,		/* 65-69 */
			0, 0, 0, 0,	0, 0,	/* 6A-6F */
			(int)XKeySym.XK_F1, (int)XKeySym.XK_F2, (int)XKeySym.XK_F3, (int)XKeySym.XK_F4, (int)XKeySym.XK_F5,		/* 70-74 */ 
			(int)XKeySym.XK_F6, (int)XKeySym.XK_F7, (int)XKeySym.XK_F8, (int)XKeySym.XK_F9, (int)XKeySym.XK_F10,		/* 75-79 */
			(int)XKeySym.XK_F11, (int)XKeySym.XK_F12, 0, 0,	0, 0,	/* 7A-7F */
			0, 0, 0, 0, 0,		/* 80-84 */ 
			0, 0, 0, 0, 0,		/* 85-89 */
			0, 0, 0, 0,	0, 0,	/* 8A-8F */
			0, 0, 0, 0, 0,		/* 90-94 */ 
			0, 0, 0, 0, 0,		/* 95-99 */
			0, 0, 0, 0,	0, 0,	/* 9A-9F */
			(int)XKeySym.XK_Shift_L, (int)XKeySym.XK_Shift_R, (int)XKeySym.XK_Control_L, (int)XKeySym.XK_Control_R, (int)XKeySym.XK_Alt_L,		/* A0-A4 */ 
			(int)XKeySym.XK_Alt_R, 0, 0, 0, 0,		/* A5-A9 */
			0, 0, 0, 0,	0, 0,	/* AA-AF */
			0, 0, 0, 0, 0,		/* B0-B4 */ 
			0, 0, 0, 0, 0,		/* B5-B9 */
			0, 0, 0, 0,	0, 0,	/* BA-BF */
			0, 0, 0, 0, 0,		/* C0-C4 */ 
			0, 0, 0, 0, 0,		/* C5-C9 */
			0, 0, 0, 0,	0, 0,	/* CA-CF */
			0, 0, 0, 0, 0,		/* D0-D4 */ 
			0, 0, 0, 0, 0,		/* D5-D9 */
			0, 0, 0, 0,	0, 0,	/* DA-DF */
			0, 0, 0, 0, 0,		/* E0-E4 */ 
			0, 0, 0, 0, 0,		/* E5-E9 */
			0, 0, 0, 0,	0, 0,	/* EA-EF */
			0, 0, 0, 0, 0,		/* F0-F4 */ 
			0, 0, 0, 0, 0,		/* F5-F9 */
			0, 0, 0, 0,	0, 0	/* FA-FF */
		};

        public static int ToKeycode(IntPtr display, int key)
        {
            int keycode = 0;

            if (nonchar_vkey_key[key] > 0)
                keycode = X11.XKeysymToKeycode(display, nonchar_vkey_key[key]);

            if (keycode == 0)
                keycode = X11.XKeysymToKeycode(display, key);

            return keycode;
        }
    }
}
