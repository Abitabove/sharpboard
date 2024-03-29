﻿//    Copyright 2009 Wii Device Library authors
//
//    This file is part of Wii Device Library.
//
//    Wii Device Library is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Wii Device Library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Wii Device Library.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace WiiDeviceLibrary.Bluetooth.Bluez
{
    public class BluezDeviceProvider : IDeviceProvider
    {
        #region Fields
        private IDictionary<BluetoothAddress, BluezDeviceInfo> _FoundDevices = new Dictionary<BluetoothAddress, BluezDeviceInfo>();
		private IDictionary<BluetoothAddress, BluezDeviceInfo> _LostDevices = new Dictionary<BluetoothAddress, BluezDeviceInfo>();
		private List<BluetoothAddress> _NameRequested = new List<WiiDeviceLibrary.Bluetooth.BluetoothAddress>();
        private ICollection<IDevice> _ConnectedDevices = new List<IDevice>();
		private Thread _DiscoverThread = null;
		private int _DiscoverSocket = 0;
        #endregion

        #region Properties
        public ICollection<IDeviceInfo> FoundDevices
        {
            get { return (ICollection<IDeviceInfo>)_FoundDevices.Values; }
        }

        public ICollection<IDevice> ConnectedDevices
        {
            get { return _ConnectedDevices; }
        }

        public bool IsDiscovering
        {
            get { return _DiscoverThread != null; }
        }
        #endregion

        #region Constructors
        public BluezDeviceProvider()
        {
        }
        #endregion

        public void StartDiscovering()
        {
        	if(_DiscoverThread != null)
				throw new InvalidOperationException("The " + GetType().Name + " is already discovering.");
        	_DiscoverThread = new Thread(new ThreadStart(DiscoverFunction));
        	_DiscoverThread.Start();
        }

        public void StopDiscovering()
        {
            if (_DiscoverThread != null)
            {
				NativeMethods.hci_send_cmd(_DiscoverSocket, NativeMethods.OGF_LINK_CTL, NativeMethods.OCF_INQUIRY_CANCEL, 0, IntPtr.Zero);
                Thread stoppingThread = _DiscoverThread;
                _DiscoverThread = null;
                stoppingThread.Join();
            }
        }

		private int CreateDiscoverSocket()
		{
			int adapterId = -1;
			int socket = -1;
			
			// Find a bluetooth adapter, repeat until found
			while(_DiscoverThread != null && adapterId == -1)
			{
				adapterId = NativeMethods.hci_get_route(IntPtr.Zero);
				Thread.Sleep(1000);
			}
			
			// did the loop exit because of _DiscoverThread or adapterId
			if(adapterId == -1)
				return -1;
			
			// create the socket
            if ((socket = NativeMethods.hci_open_dev(adapterId)) == -1)
                return -1;

			// configure the socket
            NativeMethods.hci_filter filter = default(NativeMethods.hci_filter);
            filter.opcode = 0x0;
            filter.type_mask = 16;
            filter.event_mask_a = 0xffffffff;
            filter.event_mask_b = 0xffffffff;
            NativeMethods.setsockopt(socket, NativeMethods.SOL_HCI, NativeMethods.HCI_FILTER, ref filter, 14);				
			
			return socket;
		}
		
        private void DiscoverFunction()
        {
            byte[] buffer = new byte[255];
            _DiscoverSocket = CreateDiscoverSocket();
			if(_DiscoverSocket == -1)
				return;
			InitiateInquiry();
			

            while (_DiscoverThread != null)
            {
                int bytes = NativeMethods.recv(_DiscoverSocket, buffer, 255, 0);
				if(bytes == -1)
				{
					NativeMethods.hci_close_dev(_DiscoverSocket);
					_DiscoverSocket = CreateDiscoverSocket();
					if(_DiscoverSocket == -1)
						return;
					InitiateInquiry();
					continue;
				}				
				ParseEvent(buffer, bytes);
            }           
			NativeMethods.hci_close_dev(_DiscoverSocket);
        }
        
		private void FlushSocket()
		{
			byte[] buffer = new byte[255];
			while(NativeMethods.recv(_DiscoverSocket, buffer, 255, NativeMethods.MSG_DONTWAIT) != -1);
		}
		
		private void RequestRemoteName(BluetoothAddress address)
		{
			// create a command packet
			NativeMethods.remote_name_req_cp cmd_pkt = default(NativeMethods.remote_name_req_cp);
			NativeMethods.str2ba(address.ToString(), out cmd_pkt.bdaddr);
		
			// send the command packet
			GCHandle pinnedPacket = GCHandle.Alloc(cmd_pkt, GCHandleType.Pinned);
			NativeMethods.hci_send_cmd(_DiscoverSocket, NativeMethods.OGF_LINK_CTL, NativeMethods.OCF_REMOTE_NAME_REQ, 10, pinnedPacket.AddrOfPinnedObject());
			pinnedPacket.Free();
		}
		
		private void InitiateInquiry()
		{
			// create the command packet to initiate an inquiry
            NativeMethods.inquiry_cp cmd_pkt;
            cmd_pkt.lap_a = 0x33;
            cmd_pkt.lap_b = 0x8b;
            cmd_pkt.lap_c = 0x9e;
            cmd_pkt.length = 1;
            cmd_pkt.num_rsp = 255;
			GCHandle pinnedPacket = GCHandle.Alloc(cmd_pkt, GCHandleType.Pinned);
			NativeMethods.hci_send_cmd(_DiscoverSocket, NativeMethods.OGF_LINK_CTL, NativeMethods.OCF_INQUIRY, 5, pinnedPacket.AddrOfPinnedObject());
			pinnedPacket.Free();
		}
		
		private NativeMethods.HciEvent ParseEvent(byte[] buffer, int count)
		{
			if(buffer[0] != NativeMethods.HCI_EVENT_PKT)
				throw new ArgumentException("The provided buffer does not contain an event", "buffer");

            NativeMethods.HciEvent eventType = (NativeMethods.HciEvent)buffer[1];
            switch (eventType)
            {
                case NativeMethods.HciEvent.EVT_INQUIRY_RESULT:
				    ParseInquiryResult(buffer, 2);
					InitiateInquiry();
				    break;
                case NativeMethods.HciEvent.EVT_INQUIRY_RESULT_WITH_RSSI:
				    ParseInquiryResult(buffer, 4);
					InitiateInquiry();
				    break;
                case NativeMethods.HciEvent.EVT_REMOTE_NAME_REQ_COMPLETE:
				    ParseRemoteNameReqComplete(buffer, 3, count - 3);
				    InitiateInquiry();
				    break;
                case NativeMethods.HciEvent.EVT_INQUIRY_COMPLETE:
                case NativeMethods.HciEvent.EVT_DISCONN_COMPLETE:
				    RemoveLostDevices();				
				    InitiateInquiry();
				    break;
			}
			return eventType;
		}
		
		private void ParseInquiryResult(byte[] buffer, int offset)
		{
		    BluetoothAddress address = CreateAddress(buffer, offset);
		    HandleInquiry(address);
		}
		
		private void RemoveLostDevices()
		{
			foreach(BluezDeviceInfo info in new List<BluezDeviceInfo>(_FoundDevices.Values))
			{
				if((DateTime.Now - info.LastSeen).TotalSeconds > 3)
				{
					_LostDevices[info.Address] = info;
					_FoundDevices.Remove(info.Address);
					if(DeviceLost != null)
						DeviceLost(this, new DeviceInfoEventArgs(info));
				}
			}
		}
		
		private void HandleInquiry(BluetoothAddress address)
		{
			if(_FoundDevices.ContainsKey(address))
		    {
			    _FoundDevices[address].LastSeen = DateTime.Now;
		    }
		    else
		    {
				if(_NameRequested.Contains(address))
				{
					if((!_FoundDevices.ContainsKey(address)) && _LostDevices.ContainsKey(address))
					{
						BluezDeviceInfo info = _LostDevices[address];
						info.LastSeen = DateTime.Now;
						_LostDevices.Remove(address);
						_FoundDevices.Add(address, info);
						if(DeviceFound != null)
							DeviceFound(this, new DeviceInfoEventArgs(_FoundDevices[address]));
						FlushSocket();
					}
				}
				else
				{
					_NameRequested.Add(address);
					RequestRemoteName(address);
				}
		    }
		}
		
		private void ParseRemoteNameReqComplete(byte[] buffer, int offset, int count)
		{
			BluetoothAddress address = CreateAddress(buffer, offset + 1);

            // parse the bytes
            // delfo
            byte status = buffer[offset];
            if (status != 0)
            {
                _NameRequested.Remove(address);
                FlushSocket();
                return;
            }

			int nameCount = count - 7;
			int endOfName = Array.IndexOf<byte>(buffer, 0, offset + 7, nameCount) - (offset + 7);
			if (endOfName < 0)
				endOfName = nameCount;
			string name = System.Text.Encoding.ASCII.GetString(buffer, offset + 7, endOfName);

            //Console.WriteLine("-{0}-{1}-{2}-",name,offset + 7, endOfName);
            //int i = 0;
            //foreach (int ch in buffer)
            //{
            //    if (ch >32 && ch <121)
            //    {
            //        Console.Write("{0}:{1} ",(char)ch,i);
            //    }
            //    i++;
            //}
			
			switch(name)
			{
			    case "Nintendo RVL-WBC-01":
			    case "Nintendo RVL-CNT-01":
				    break;
			    default:
				    return;
			}
			
			if (_FoundDevices.ContainsKey(address) || IsConnected(address))
				return;
			
			// create a new deviceinfo and throw a DeviceFound event
			BluezDeviceInfo info = new BluezDeviceInfo(address, name);
			_FoundDevices.Add(address, info);
			if(DeviceFound != null)
				DeviceFound(this, new DeviceInfoEventArgs(info));	
			FlushSocket();
		}
		
		private bool IsConnected(BluetoothAddress address)
		{
			foreach(IDevice device in _ConnectedDevices)
			{
				if ((device.DeviceInfo as BluezDeviceInfo).Address.Equals(address))
					return true;
			}
			return false;
		}
		
		private static BluetoothAddress CreateAddress(byte[] bytes, int offset)
		{
			byte[] addressBytes = new byte[6];
			Array.Copy(bytes, offset, addressBytes, 0,  6);
			Array.Reverse(addressBytes);
			return new BluetoothAddress(addressBytes);
		}
		
        public IDevice Connect(IDeviceInfo deviceInfo)
        {
            BluezDeviceInfo info = deviceInfo as BluezDeviceInfo;
            if (info == null)
                throw new ArgumentException("The specified IDeviceInfo does not belong to this DeviceProvider.", "deviceInfo");
            
            Stream bluezStream = new BluezStream(info.Address);
            IWiiDevice device = null;
			
			// determine the device type
			if (info.Name == "Nintendo RVL-WBC-01")
                device = new ReportBalanceBoard(deviceInfo, bluezStream);
            else if (info.Name == "Nintendo RVL-CNT-01")
                device = new ReportWiimote(deviceInfo, bluezStream);
			else
				throw new ArgumentException("The specified deviceInfo with name '" + info.Name + "' is not supported.", "deviceInfo");

			try
			{
            	device.Initialize();
			}
			catch(Exception e)
			{
				throw new DeviceConnectException("Failed to connect to device", e);
			}	

			_FoundDevices.Remove(info.Address);
			_LostDevices.Add(info.Address, info);			
			if(DeviceLost != null)
				DeviceLost(this, new DeviceInfoEventArgs(info));
			
            _ConnectedDevices.Add(device);
            device.Disconnected += DeviceDisconnectedHandler;
			
			if(DeviceConnected != null)
				DeviceConnected(this, new DeviceEventArgs(device));
            return device;
        }

        #region Event Handlers
        private void DeviceDisconnectedHandler(object sender, EventArgs args)
        {
            IDevice device = (IDevice)sender;
            _ConnectedDevices.Remove(device);
            if (DeviceDisconnected != null)
                DeviceDisconnected(this, new DeviceEventArgs(device));
        }
        #endregion

        #region Events
        public event EventHandler<DeviceEventArgs> DeviceConnected;
        public event EventHandler<DeviceEventArgs> DeviceDisconnected;
        public event EventHandler<DeviceInfoEventArgs> DeviceFound;
        public event EventHandler<DeviceInfoEventArgs> DeviceLost;
        #endregion
    }
}
