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
using System.Drawing;
using WiiDeviceLibrary;
using System.Reflection;

namespace SharpBoardLibrary
{
    public static class ExtensionsMethods
    {
        public static PointF[] ToPointF(this CoordinateF[] coords)
        {
            PointF[] points = new PointF[coords.Length];
            for (int j = 0; j < 4; j++)
            {
                points[j] = new PointF(coords[j].X, coords[j].Y);
            }
            return points;
        }

        public static string GetBTProviderType(this IDeviceProvider provider)
        {
            string[] parts = provider.GetType().ToString().Split('.');
            string providerType = parts[parts.Length - 1].Replace("DeviceProvider", "");
            return providerType;
        }

        public static string GetDeviceType(this IDeviceInfo iDevice)
        {
            string[] parts = iDevice.GetType().ToString().Split('.');
            string providerType = parts[parts.Length - 1].Replace("DeviceInfo", "");
            return providerType;
        }

        public static BasicIRBeacon[] GetOrderedIRBeacon(this IWiimote wiimote)
        {
            BasicIRBeacon[] beacons = null;
            switch (wiimote.ReportingMode)
            {
                case ReportingMode.Buttons10Ir9Extension:
                case ReportingMode.ButtonsAccelerometer10Ir6Extension:
                    beacons =
                        (from bea in wiimote.IRBeacons
                         let b = bea as BasicIRBeacon
                         where b != null
                         select b).ToArray();
                    break;
                case ReportingMode.ButtonsAccelerometer12Ir:
                    beacons =
                        (from bea in wiimote.IRBeacons
                         let b = bea as ExtendedIRBeacon
                         where b != null
                         orderby b.Size descending
                         select b).ToArray();
                    break;
                case ReportingMode.ButtonsAccelerometer36Ir:
                    beacons =
                        (from bea in wiimote.IRBeacons
                         let b = bea as FullIRBeacon
                         where b != null
                         orderby b.Intensity descending
                         select b).ToArray();
                    break;
            }
            return beacons;
        }

        public static CoordinateF GetSmoothedCursor(this SmoothingData smooth, int amount)
        {
            int start = smooth.SmoothingBufferIndex - amount;
            if (start < 0)
                start = 0;
            CoordinateF smoothed = new CoordinateF(0, 0);
            int count = smooth.SmoothingBufferIndex - start;
            if (count == 0)
            {
                count = 1;
            }
            for (int i = start; i < smooth.SmoothingBufferIndex; i++)
            {
                smoothed.X += smooth.SmoothingBuffer[i % smooth.SmoothingBuffer.Length].X;
                smoothed.Y += smooth.SmoothingBuffer[i % smooth.SmoothingBuffer.Length].Y;
            }
            smoothed.X /= count;
            smoothed.Y /= count;
            //string tmp = string.Format("{0}-{1}-{2}", start, count, smoothed.ToString());
            //Debug.WriteLine(tmp);
            return smoothed;
        }

        public static string GetID(this IDeviceInfo dev, int length)
        {
            string s = "n/a";
            if (dev != null)
            {
                s = dev.ToString();
                if (length > 0)
                {
                    s = s.Substring(0, length);
                }
            }
            return s;
        }

        public static string GetID(this IDeviceInfo dev)
        {
            return GetID(dev, -1);
        }

        public static void CopyObjectValues<T>(this T destination, T source)
        {
            PropertyInfo[] propertyInfos = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                System.Reflection.MethodInfo setMethod = propertyInfo.GetSetMethod();
                if (setMethod != null)
                    propertyInfo.SetValue(destination, propertyInfo.GetValue(source, null), null);
            }
        }
    }
}
