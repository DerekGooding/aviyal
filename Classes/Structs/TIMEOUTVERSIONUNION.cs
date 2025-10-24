/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

[StructLayout(LayoutKind.Explicit)]
public struct TIMEOUTVERSIONUNION
{
	[FieldOffset(0)]
	public uint uTimeout;
	[FieldOffset(0)]
	public uint uVersion;
}

