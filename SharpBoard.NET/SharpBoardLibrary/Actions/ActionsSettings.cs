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
using System.IO;

namespace SharpBoardLibrary
{
    public class ActionsSettings : Dictionary<string, ActionCollection>
    {
        public int LoadAllSettings()
        {
            return LoadAllSettings(Global.AppFolderPath);
        }

        public int LoadAllSettings(string path)
        {
            base.Clear();
            int num = 0;
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] listFile = di.GetFiles("*.xml");
            //var goodFile = from f in listFile
            //               where f.Name.ToLower().Contains("presenter")
            //               select f;
            foreach (FileInfo presFile in listFile)
            {
                try
                {
                    ActionCollection setting = ActionCollection.LoadFromXML(presFile.FullName);
                    if (setting != null)
                    {
                        base.Add(presFile.Name, setting);
                        num++;
                    }
                }
                catch { }
            }

            return num;
        }
    }
}
