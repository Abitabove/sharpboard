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
using SharpBoardLibrary.Display.Win32;
using SharpBoardLibrary.Display.X11;

namespace SharpBoardLibrary.Display
{
    public enum DisplayFactoryType
    {
        X11,
        Win32
    }

    public static class DisplayDeviceFactory
    {
        static IList<IDisplayDeviceProviderFactory> _Factories = new List<IDisplayDeviceProviderFactory>();
        public static IList<IDisplayDeviceProviderFactory> Factories
        {
            get { return _Factories; }
        }

        static DisplayDeviceFactory()
        {
            Factories.Add(new SharpBoardLibrary.Display.Win32.Win32DeviceProviderFactory());
            Factories.Add(new SharpBoardLibrary.Display.X11.X11DeviceProviderFactory());
        }

        public static IDisplayDeviceProvider CreateVideoDevice()
        {
            IDisplayDeviceProvider videoDevice = null;
            IDisplayDeviceProviderFactory suppDevice;

            suppDevice = new Win32DeviceProviderFactory();
            if (suppDevice.IsSupported) // Win32
            {
                videoDevice = (IDisplayDeviceProvider)new Win32DeviceProvider();
            }

            suppDevice = new X11DeviceProviderFactory();
            if (suppDevice.IsSupported) // X11
            {
                videoDevice = (IDisplayDeviceProvider)new X11DeviceProvider();
            }
            return videoDevice;
        }

        public static IDisplayDeviceProviderFactory GetSupportedFactory()
        {
            foreach (IDisplayDeviceProviderFactory factory in Factories)
            {
                if (factory.IsSupported)
                    return factory;
            }
            return null;
        }

        public static IDisplayDeviceProvider CreateSupportedDeviceProvider()
        {
            IDisplayDeviceProviderFactory factory = GetSupportedFactory();
            if (factory == null)
                return null;
            return factory.Create();
        }
    }
}
