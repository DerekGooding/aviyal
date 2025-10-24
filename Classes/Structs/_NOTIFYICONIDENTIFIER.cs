/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// struct that applications use to query its tray icon information using
/// Shell_NotifyIcon() and Shell_NotifyIconGetRect(), these functions would then send
/// another internal struct [_NOTIFYICONIDENTIFIERINTERNAL] containing additional items 
/// to Shell_TrayWnd
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct _NOTIFYICONIDENTIFIER
{
	public uint cbSize;
	public nint hWnd;
	public uint UID;
	public Guid guidItem;
}

