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
    public static class ActionTypeFactory
    {
        public static XElement CreateXElement(ActionBase act)
        {
            XElement element = act.XElement;
            switch (act.Type)
            {
                case ActionType.Keys:
                    element = ((ActionKeys)act).XElement;
                    break;
                case ActionType.Mouse:
                    element = ((ActionMouse)act).XElement;
                    break;
                case ActionType.Toogle:
                    element = ((ActionToogle)act).XElement;
                    break;
                case ActionType.Process:
                    element = ((ActionProcess)act).XElement;
                    break;
                default:
                    break;
            }
            return element;
        }

        public static ActionBase CreateAction(XElement element)
        {
            ActionBase act = new ActionBase(element);
            switch (act.Type)
            {
                case ActionType.Keys:
                    act = new ActionKeys(element);
                    break;
                case ActionType.Mouse:
                    act = new ActionMouse(element);
                    break;
                case ActionType.Toogle:
                    act = new ActionToogle(element);
                    break;
                case ActionType.Process:
                    act = new ActionProcess(element);
                    break;
                default:
                    break;
            }
            return act;
        }
    }
}
