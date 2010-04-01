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
using System.Xml.Linq;

namespace SharpBoardLibrary
{
    public class FiredBy
    {
        public FiredByType Type { get; set; }
        public WiimoteButtons Button { get; set; }
        public ToolBarButtons ToolBarButton { get; set; }
        public ActionBase Action { get; set; }
        public bool IsActive { get; set; }

        public XElement XElement
        {
            get
            {
                XElement element =
                    new XElement("FiredBy",
                        new XAttribute("type", Type.ToString()),
                        new XAttribute("isActive", IsActive.ToString()));
                switch (Type)
                {
                    case FiredByType.WiimoteButtons:
                        element.Add(new XAttribute("button", Button.ToString()));
                        break;
                    case FiredByType.ToolBarButtons:
                        element.Add(new XAttribute("toolBarButton", ToolBarButton.ToString()));
                        break;
                    default:
                        break;
                }
                return element;
            }
        }

        public FiredBy()
        { }

        public FiredBy(XElement element)
        {
            Type = (FiredByType)Enum.Parse(typeof(FiredByType), element.Attribute("type").Value);
            IsActive = element.GetBoolAttribute("isActive", true, false);
            switch (Type)
            {
                case FiredByType.WiimoteButtons:
                    Button = (WiimoteButtons)Enum.Parse(typeof(WiimoteButtons), element.Attribute("button").Value);
                    break;
                case FiredByType.ToolBarButtons:
                    ToolBarButton = (ToolBarButtons)Enum.Parse(typeof(ToolBarButtons), element.Attribute("toolBarButton").Value);
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            string s = Type.ToString() + " ";
            switch (Type)
            {
                case FiredByType.WiimoteButtons:
                    s = Button.ToString();
                    break;
                case FiredByType.ToolBarButtons:
                    s = ToolBarButton.ToString();
                    break;
            }
            return s;
        }
    }
}
