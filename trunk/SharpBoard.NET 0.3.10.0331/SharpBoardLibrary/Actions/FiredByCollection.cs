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
using System.Collections;

namespace SharpBoardLibrary
{
    public class FiredByCollection : SortedList<string,FiredBy>
    {
        public List<string> DefaultEvents()
        {
            List<string> list = new List<string>();
            foreach (var item in this.Values)
            {
                list.Add(item.ToString());
            }
            return list;
        }

        //public IEnumerable XElement
        //{
        //    get
        //    {
        //        for (int i = 0; i < Count; i++)
        //        {
        //            yield return this.Values[i].XElement;
        //        }
        //    }
        //}

        public FiredByCollection()
        { }

        public FiredByCollection(XElement element)
        {
            var fired = element.Elements("FiredBy");
            this.Clear();
            foreach (XElement e in fired)
            {
                FiredBy f = new FiredBy(e);
                this.Add(f.ToString(),f);
            }
        }
    }
}
