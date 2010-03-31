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
using System.IO;
using System.Xml.Linq;

namespace SharpBoardLibrary
{
    public class ConfigInfo : Loggable
    {
        string _configFile;
        //LogWriter _log;

        // public ConfigInfo(string configFile, LogWriter log)
        public ConfigInfo(string configFile)
        {
            RegisterLogName("CONFIG");
            _configFile = configFile;
            //_log = log;
        }

        public bool SaveSBConfig(SharpBoard _sb)
        {
            bool ok = false;

            if (_sb != null)
            {
                try
                {
                    XElement configDoc = _sb.XElement;
                    configDoc.Save(_configFile);
                    // TODO: to be deleted when GUI completed
                    //_sb.Actions.SaveToXML(Global.AppPresenterDefaultFileName);
                    ok = true;
                }
                catch (Exception e)
                {
                    AddLog(LogLevel.FatalError, e.Message);
                    return false;
                }
            }
            else
            {
                AddLog(LogLevel.FatalError, "SharpBoard instance is null");
            }
            return ok;
        }

        public bool LoadSBConfig(SharpBoard _sb)
        {
            bool ok = false;
            if (_sb != null)
            {

                try
                {
                    XElement configDoc = XElement.Load(_configFile);
                    SharpBoard loadedSB = new SharpBoard(configDoc);

                    _sb.CopyObjectValues<SharpBoard>(loadedSB);

                    //_sb.ScreenSize = loadedSB.ScreenSize;
                    //_sb.Smoothing = loadedSB.Smoothing;

                    //_sb.BTProviderDiscovery = loadedSB.BTProviderDiscovery;
                    //_sb.StartGui = loadedSB.StartGui;
                    //_sb.MinimizeOnTray = loadedSB.MinimizeOnTray;
                    //_sb.ToolBarSetting = loadedSB.ToolBarSetting;
                    //_sb.AutoConnectDevices = loadedSB.AutoConnectDevices;
                    //_sb.ActionsSettingsFiles = loadedSB.ActionsSettingsFiles;
                    //_sb.ActionsSettingsFileName = loadedSB.ActionsSettingsFileName;
                    //_sb.WiiDevicesStoredInfo = loadedSB.WiiDevicesStoredInfo;
                    //_sb.Actions = loadedSB.Actions;

                    // _sb.Events = _sb.Actions.Events;
                    ok = true;
                }
                catch (Exception e)
                {
                    AddLog(LogLevel.FatalError, e.Message);
                    return false;
                }
            }
            else
            {
                AddLog(LogLevel.FatalError, "SharpBoard instance is null");
            }
            return ok;
        }
    }
}
