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

namespace SharpBoardLibrary
{
    public class Loggable
    {
        string objectName;

        protected void RegisterLogName(string objectName)
        {
            this.objectName = objectName;
        }

        protected void AddLog(LogLevel level, string text)
        {
            Global.Log.Add(objectName, level, text);
        }

        protected void AddLog(LogLevel level, string format, params object[] args)
        {
            Global.Log.Add(objectName, level, format, args);
        }

        protected void AddLog(string format, params object[] args)
        {
            Global.Log.Add(null, LogLevel.None, format, args);
        }

        protected void AddLog(string text)
        {
            Global.Log.Add(null, LogLevel.None, text);
        }

        protected void AddNewLine()
        {
            Global.Log.AddNewLine();
        }

    }
}
