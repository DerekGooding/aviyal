/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// Message Type recieved or send to Taskbar [Shell_TrayWnd]
/// during the WM_COPYDATA event
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct SHELLTRAYICONUPDATEDATA
{
	public int dwHz;
	public uint dwMessage;
	public NOTIFYICONDATA nid;
}

