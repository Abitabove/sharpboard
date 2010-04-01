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
using System.Diagnostics;


namespace SharpBoardLibrary
{
    public static class CoordinateHelper
    {
        public static CoordinateF ScaleCoords(float width, float height, float x, float y)
        {
            CoordinateF coord = new CoordinateF();
            float scaleFactorX = (float)width / 1024f;
            float scaleFactorY = (float)height / 768f;

            coord.X = scaleFactorX * x - 2;
            coord.Y = height - (scaleFactorY * y - 2);

            return coord;
        }

        public static CoordinateF[] ScaleCoords(float width, float height, float[] srcX, float[] srcY)
        {
            CoordinateF[] areaCord = new CoordinateF[4];
            for (int j = 0; j < 4; j++)
            {
                areaCord[j] = CoordinateHelper.ScaleCoords(width, height, srcX[j], srcY[j]);
            }
            return areaCord;
        }

    }
}
