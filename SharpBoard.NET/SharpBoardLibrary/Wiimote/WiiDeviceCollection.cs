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
    public class WiiDeviceCollection : SortedList<string, WiiDevice>
    {
        public XElement XElement
        {
            get
            {
                XElement device =
                    new XElement("Devices",
                            from dev in this.Values
                            select dev.XElement);
                return device;
            }
        }

        public WiiDeviceCollection()
            : base()
        {
        }

        public WiiDeviceCollection(XElement element)
            : base()
        {
            var devices = from dev in element.Elements("WiiDevice")
                          select new WiiDevice(dev);
            this.Clear();
            foreach (var item in devices)
            {
                if (!this.ContainsKey(item.DeviceID))
                {
                    this.Add(item.DeviceID, item);
                }
            }

        }

    }
}
