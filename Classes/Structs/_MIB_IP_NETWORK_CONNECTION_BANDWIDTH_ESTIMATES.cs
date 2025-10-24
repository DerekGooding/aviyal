/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// https://learn.microsoft.com/en-us/windows/win32/api/netioapi/ns-netioapi-mib_ip_network_connection_bandwidth_estimates
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct _MIB_IP_NETWORK_CONNECTION_BANDWIDTH_ESTIMATES
{
	public _NL_BANDWIDTH_INFORMATION InboundBandwidthInformation;
	public _NL_BANDWIDTH_INFORMATION OutboundBandwidthInformation;
}

