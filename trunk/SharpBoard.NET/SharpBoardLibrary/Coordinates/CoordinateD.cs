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
using System.Xml.Linq;

namespace SharpBoardLibrary
{
    public class CoordinateD : CoordinateI
    {
        public int Width
        {
            get { return _x; }
            set { _x = value; }
        }
        public int Height
        {
            get { return _y; }
            set { _y = value; }
        }

        public CoordinateD(int x, int y)
            : base(x, y)
        { }

        public CoordinateD()
            : base()
        { }

        public CoordinateD(XElement element)
        {
            _x = int.Parse(element.Attribute("Width").Value);
            _y = int.Parse(element.Attribute("Height").Value);
        }

        public XElement XElement
        {
            get
            {
                return new XElement("ScreenSize",
                    new XAttribute("Width", _x),
                    new XAttribute("Height", _y));
            }
        }
    }
}
