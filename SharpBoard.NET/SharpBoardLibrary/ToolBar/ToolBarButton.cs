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
using System.IO;

namespace SharpBoardLibrary
{
    public class ToolBarButton
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string IconFileName { get; set; }
        public bool IsAbsolutePath { get; set; }

        public string IconPath
        {
            get
            {
                string path = IconFileName;
                if (!IsAbsolutePath)
                {
                    path = Path.Combine(Global.AppFolderPath, IconFileName);
                }
                return path;
            }
        }

        public XElement XElement
        {
            get
            {
                XElement element = 
                    new XElement("Button",
                        new XAttribute("name", Name),
                        new XAttribute("text", Text),
                        new XAttribute("isAbsolutePath", IsAbsolutePath),
                        IconFileName);
                return element;
            }
        }

        public ToolBarButton(XElement element)
        {
            Name = element.GetAttribute("name");
            Text = element.GetAttribute("text");
            IsAbsolutePath = element.GetBoolAttribute("isAbsolutePath", true, true);
            IconFileName = element.Value;
        }
    }
}
