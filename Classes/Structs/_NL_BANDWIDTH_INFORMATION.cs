/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct _NL_BANDWIDTH_INFORMATION
{
	public ulong Bandwidth;
	public ulong Instability;
	public byte BandwidthPeaked;
}

