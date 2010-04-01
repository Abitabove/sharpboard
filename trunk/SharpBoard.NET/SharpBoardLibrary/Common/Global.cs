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
using WiiDeviceLibrary;

namespace SharpBoardLibrary
{
    public class Global : Loggable
    {
        public static readonly string AppName = "SharpBoard.NET";
        public static readonly string FIRM = AppName + " ver 1.0";

        public static readonly string AppFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), AppName);
        public static readonly string AppLogFolderPath = Path.Combine(AppFolderPath, "Logs");
        public static readonly string AppLogFileName = Path.Combine(AppLogFolderPath, AppName);
        public static readonly string AppConfigFileName = Path.Combine(AppFolderPath, "Config.xml");
        public static readonly string AppPresenterDefaultFileName = Path.Combine(AppFolderPath, "PresenterSettings.xml");

        static ConfigInfo _config;
        public static ConfigInfo Config
        {
            get
            {
                if (_config == null)
                {
                    _config = new ConfigInfo(Global.AppConfigFileName);
                }
                return _config;
            }
        }

        static LogWriter _log;
        public static LogWriter Log
        {
            get 
            {
                if (_log == null)
                {
                    _log = new LogWriter(Global.AppLogFileName);
                }
                return _log; 
            }
        }

        public static void WriteFirm(StreamWriter sw, string header)
        {
            sw.WriteLine("{0} [{1}] - {2}", header, DateTime.Now, Global.FIRM);
            sw.WriteLine();
        }

        public static string MakeDateFileName(DateTime data, string fileName)
        {
            return string.Format("{0}{1:0000}.{2:00}.{3:00}.log", fileName, data.Year, data.Month, data.Day);
        }

        public static string MakeDateFileName(string fileName)
        {
            DateTime data = DateTime.Now;
            return MakeDateFileName(data, fileName);
        }
    }
}
