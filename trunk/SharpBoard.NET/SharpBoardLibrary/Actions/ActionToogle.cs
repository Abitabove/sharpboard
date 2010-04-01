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
using WiiDeviceLibrary;

namespace SharpBoardLibrary
{
    public class ActionToogle : ActionBase
    {
        public ToogleFeature Feature { get; set; }
        public ToogleAction Action { get; set; }

        public ActionToogle()
        { }

        public ActionToogle(XElement element)
            : base(element)
        {
            Feature = (ToogleFeature)Enum.Parse(typeof(ToogleFeature), element.Attribute("feature").Value);
            Action = (ToogleAction)Enum.Parse(typeof(ToogleAction), element.Attribute("action").Value);
        }

        public new XElement XElement
        {
            get
            {
                XElement action = base.XElement;
                action.Add(new XAttribute("feature", Feature.ToString()));
                action.Add(new XAttribute("action", Action.ToString()));
                return action;
            }
        }

        public override void SetParameters(string param)
        {
            string[] parts = param.Split(',');
            Feature = (ToogleFeature)Enum.Parse(typeof(ToogleFeature),parts[0]);
            Action = (ToogleAction)Enum.Parse(typeof(ToogleAction), parts[1]);
        }
    }
}
