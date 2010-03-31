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
    public class ActionBase
    {
        public ActionType Type { get; set; }
        public string Description { get; set; }
        public FiredByCollection Fired { get; set; }

        public ActionBase()
        { }

        public ActionBase(XElement element)
        {
            Type = (ActionType)Enum.Parse(typeof(ActionType), element.Attribute("type").Value);
            Description = element.Attribute("description").Value;
            Fired = new FiredByCollection(element);
        }

        public XElement XElement
        {
            get
            {
                XElement action =
                    new XElement("Action",
                        new XAttribute("type", Type.ToString()),
                        new XAttribute("description", Description));
                if (Fired != null)
                {
                    foreach (var fire in Fired.OrderBy(x => x.Value.Type))
                    {
                        action.Add(fire.Value.XElement);
                    }
                }
                return action;
            }
        }

        public virtual void SetParameters(string param)
        {
            throw new NotImplementedException();
        }

    }
}
