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

namespace SharpBoardLibrary
{
    public class CoordinateF
    {
        public float X { get; set; }
        public float Y { get; set; }

        public CoordinateF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public CoordinateF()
            : this(-1f, -1f)
        {
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", X, Y);
        }

        public override bool Equals(object obj)
        {
            CoordinateF cor = obj as CoordinateF;
            return (X == cor.X && Y == cor.Y);
        }

        public static bool operator ==(CoordinateF cor1, CoordinateF cor2)
        {
            return cor1.Equals(cor2);
        }

        public static bool operator !=(CoordinateF cor1, CoordinateF cor2)
        {
            return !cor1.Equals(cor2);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static explicit operator CoordinateI(CoordinateF coord)
        {
            CoordinateI c = new CoordinateI((int)coord.X, (int)coord.Y);
            return c;
        }

        public double Distance(CoordinateF p1)
        {
            double xDist = X - p1.X;
            double yDist = Y - p1.Y;
            double dist = Math.Sqrt(xDist * xDist + yDist * yDist);
            return dist;
        }

        public bool IsApproxCoord(CoordinateF coord)
        {
            return IsApproxCoord(coord, 20f);
        }

        public bool IsApproxCoord(CoordinateF coord, float dist)
        {
            bool approx = false;
            if (this.Distance(coord) < dist)
            {
                approx = true;
            }
            return approx;
        }

    }
}
