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
    public class SmoothingConfig
    {
        public static readonly int DEFAULT_SMOOTHING_AMMOUNT = 8;
        public static readonly int DEFAULT_SMOOTHING_BUFFER_SIZE = 50;

        public bool IsEnabled { get; set; }
        public int Amount { get; set; }
        public int BufferSize { get; set; }

        public SmoothingConfig()
        {
            IsEnabled = true;
            Amount = SmoothingConfig.DEFAULT_SMOOTHING_AMMOUNT;
            BufferSize = SmoothingConfig.DEFAULT_SMOOTHING_BUFFER_SIZE;
        }


        public XElement XElement
        {
            get
            {
                return new XElement("Smoothing",
                    new XAttribute("isEnabled", IsEnabled),
                    new XAttribute("amount", Amount),
                    new XAttribute("bufferSize", BufferSize));
            }
        }

        public SmoothingConfig(XElement element)
            : this()
        {
            if (element != null)
            {
                IsEnabled = element.GetBoolAttribute("isEnabled", true, true);
                Amount = element.GetIntAttribute("amount", true, SmoothingConfig.DEFAULT_SMOOTHING_AMMOUNT);
                BufferSize = element.GetIntAttribute("bufferSize", true, SmoothingConfig.DEFAULT_SMOOTHING_BUFFER_SIZE);
            }
        }
    }
}
