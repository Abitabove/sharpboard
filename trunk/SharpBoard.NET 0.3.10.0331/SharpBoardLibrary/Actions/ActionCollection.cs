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
    public class ActionCollection : Dictionary<string, ActionBase>
    {
        public string FullName { get; set; }

        private FiredByCollection _events = null;
        public FiredByCollection Events
        {
            get
            {
                if (_events != null)
                    return _events;
                _events = new FiredByCollection();
                foreach (var act in this.Values)
                {
                    if (act.Fired != null)
                        foreach (var evt in act.Fired)
                        {
                            if (!_events.ContainsKey(evt.Key))
                            {
                                evt.Value.Action = act;
                                _events.Add(evt.Key, evt.Value);
                            }
                        }
                }

                return _events;
            }
        }

        public XElement XElement
        {
            get
            {
                XElement element = new XElement("Actions");
                foreach (var action in this.Values)
                {
                    element.Add(ActionTypeFactory.CreateXElement(action));
                }
                return element;
            }
        }

        public ActionCollection()
        {
            // Events = new FiredByCollection();
        }

        public void Add(string key, ActionBase value)
        {
            base.Add(key, value);
            //if (value.Fired != null)
            //{
            //    foreach (var item in value.Fired)
            //    {
            //        if (!Events.ContainsKey(item.Key))
            //        {
            //            item.Value.Action = value;
            //            Events.Add(item.Key, item.Value);
            //        }
            //    }
            //}
        }

        public ActionCollection(XElement element)
        {
            var actions = from action in element.Elements("Action")
                          select action;
            this.Clear();
            // Events = new FiredByCollection();
            foreach (var action in actions)
            {
                ActionBase a = ActionTypeFactory.CreateAction(action);
                this.Add(a.Description, a);
            }
        }

        public static ActionCollection LoadFromXML(string path)
        {
            ActionBase none = new ActionBase() { Type = ActionType.None, Description = "None" };
            ActionCollection acts = new ActionCollection();
            acts.FullName = path;
            acts.Add(none.Description, none);
            XElement doc = XElement.Load(path);
            ActionCollection actions = new ActionCollection(doc);
            if (actions.Count == 0)
            {
                return null;
            }
            foreach (ActionBase a in actions.Values)
            {
                if (!acts.ContainsKey(a.Description))
                {
                    acts.Add(a.Description, a);
                }
            }
            return acts;
        }

        public static ActionCollection LoadFromXMLString(string xml)
        {
            XElement doc = XElement.Parse(xml);
            return ActionCollection.ParseXML(doc);
        }

        static ActionCollection ParseXML(XElement doc)
        {
            ActionCollection acts = new ActionCollection();
            ActionCollection actions = new ActionCollection(doc);
            foreach (ActionBase a in actions.Values)
            {
                acts.Add(a.Description, a);
            }
            return acts;
        }

        public void SaveToXML(string path)
        {
            XElement.Save(path);
        }

    }
}
