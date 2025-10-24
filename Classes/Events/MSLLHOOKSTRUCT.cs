/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Structs;
using System.Runtime.InteropServices;

namespace aviyal.Classes.Events;

[StructLayout(LayoutKind.Sequential)]
public struct MSLLHOOKSTRUCT
{
	POINT pt;
	uint mouseData;
	uint flags;
	uint time;
	nint dwExtraInfo;
}



