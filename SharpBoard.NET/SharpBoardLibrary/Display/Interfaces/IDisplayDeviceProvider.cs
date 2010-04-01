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
using System.Windows.Forms;

namespace SharpBoardLibrary.Display
{
    public interface IDisplayDeviceProvider
    {
        void FakeMove(int x, int y);
        void FakeButton(MouseButton button, MouseAction action);
        //void FakeKey(byte key, KeyAction action);
        //void FakeKeys(string keys);

        WorkingArea GetScreenSize();
        void RefreshIRMonitor(object picMonitor);

        SharpBoard SB { get; set; }
        //int SmoothingAmount { get; }
        //bool EnableSmoothing { get; }
        void GetSmoothingCoords(out int x, out int y, float warpedX, float warpedY, int screenWidth, int screenHeight, SmoothingData smoothing);
        void GetSmoothingCoords(out int x, out int y, float warpedX, float warpedY, SmoothingData smoothing);

        // ICalibrationForm CalibrationForm { get; set; }
        void SetCursor(int x, int y);
        void SetMouse(MouseButton mouse, MouseAction action);
        void SetCursorAndMouse(int x, int y, MouseButton mouse, MouseAction action);

        void SendKeys(string keys);
        //void SendKey(byte key, KeyAction action);
        //void SendKey(KeyCodeCollection keys, KeyAction action);

    }
}
