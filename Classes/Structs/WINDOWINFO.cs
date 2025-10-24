/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct WINDOWINFO
{
	public uint cbSize;
	public RECT rcWindow;
	public RECT rcClient;
	public uint dwStyle;
	public uint dwExStyle;
	public uint dwWindowStatus;
	public int cxWindowBorders;
	public int cyWindowBorders;
	public ushort atomWindowType;
	public ushort wCreatorVersion;
}

