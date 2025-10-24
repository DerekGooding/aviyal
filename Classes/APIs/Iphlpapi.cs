/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using aviyal.Classes.Structs;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

public class Iphlpapi
{
	/// <summary>
	/// https://learn.microsoft.com/en-us/windows/win32/api/netioapi/nf-netioapi-getipnetworkconnectionbandwidthestimates
	/// </summary>
	/// <param name="interfaceIndex"></param>
	/// <param name="adressFamily"></param>
	/// <param name="info"></param>
	/// <returns></returns>
	[DllImport("iphlpapi.dll", SetLastError = true)]
	public static extern int GetIpNetworkConnectionBandwidthEstimates(int interfaceIndex, ADRESS_FAMILY adressFamily, out _MIB_IP_NETWORK_CONNECTION_BANDWIDTH_ESTIMATES info);
}
