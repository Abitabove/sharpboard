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

    public enum LogLevel
    {
        None,
        Info,
        Warning,
        Error,
        FatalError
    }

    public class LogWriter
    {
        string[] textLogLevel = { "[N]", "[I]", "[W]", "[E]", "[F]" };

        string fileName;
        StreamWriter sw;
        bool logOpen;
        int dayLog;

        public LogWriter(string fileName)
        {
            this.fileName = fileName;
            string path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public bool UpdateLogFile()
        {
            int today = DateTime.Now.Day;
            if (today != dayLog)
            {
                Close();
                dayLog = today;
                OpenLog();
                return true;
            }
            OpenLog();
            return false;
        }

        void OpenLog()
        {
            if (logOpen)
                return;

            try
            {
                string nameLog = Global.MakeDateFileName(fileName + "-");
                sw = new StreamWriter(nameLog, true);
                sw.AutoFlush = true;
                Global.WriteFirm(sw, "#LOG FILE");
                logOpen = true;
            }
            catch
            {
                logOpen = false;
            }
        }


        public void Add(string objectName, LogLevel level, string text)
        {
            Add(objectName, level, false, "{0}", text);
        }

        string FormatStringFormat(string format)
        {
            format = format.Replace("{", "[{").Replace("/[", "");
            return format.Replace("}", "}]").Replace("]/", "");
        }

        public void Add(string objectName, LogLevel level, string format, params object[] args)
        {
            Add(objectName, level, true, format, args);
        }

        public void Add(string objectName, LogLevel level, bool squares, string format, params object[] args)
        {
            //string logFormat1 = "<{0,8}> [{1, -8}]{2,-3} ";
            string logFormat1 = "<{0,8}>{1, -10}{2,-4}";
            string logFormat2 = " {0,8} {1, -10}{2,-4}";

            if (!logOpen)
                return;

            string s;
            if (objectName != null)
            {
                string strLevel = textLogLevel[(int)level];
                objectName = "[" + objectName + "]";
                s = string.Format(logFormat1, DateTime.Now.ToLongTimeString(), objectName,
                    strLevel);
            }
            else
                s = string.Format(logFormat2, "", "", "");

            if (squares)
                format = FormatStringFormat(format);
            sw.WriteLine(s + string.Format(format, args));
        }

        public void AddNewLine()
        {
            sw.WriteLine();
        }

        public void Close()
        {
            if (logOpen)
            {
                try
                {
                    sw.WriteLine("\r\n");
                    sw.Close();
                }
                catch
                {
                }
            }
            logOpen = false;
        }
    }
}