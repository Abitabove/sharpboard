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
    public class CoordinateI
    {
        protected int _x;
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        protected int _y;
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public CoordinateI(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public CoordinateI()
            : this(-1, -1)
        {
        }

        public override bool Equals(object obj)
        {
            CoordinateI cor = obj as CoordinateI;
            return (_x == cor.X && _y == cor.Y);
        }

        public static bool operator ==(CoordinateI cor1, CoordinateI cor2)
        {
            return cor1.Equals(cor2);
        }

        public static bool operator !=(CoordinateI cor1, CoordinateI cor2)
        {
            return !cor1.Equals(cor2);
        }

        public override int GetHashCode()
        {
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        public static explicit operator CoordinateF(CoordinateI coord)
        {
            CoordinateF c = new CoordinateF((float)coord.X, (float)coord.Y);
            return c;
        }
    }
}
