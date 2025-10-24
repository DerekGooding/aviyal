/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct WINDOWPLACEMENT
{
	public uint length;
	public uint flags;
	public uint showCmd;
	public POINT ptMinPosition;
	public POINT ptMaxPosition;
	public RECT rcNormalPosition;
	public RECT rcDevice;
}

