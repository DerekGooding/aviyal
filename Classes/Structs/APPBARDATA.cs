/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;
using System.Drawing;

namespace aviyal.Classes.Structs;
/// <summary>
/// DWORD := uint
/// HWND  := nint
/// PVOID := nint
/// </summary>
/// ------------------------

/// <summary>
/// For controlling the visibility and autohide behaviours of the taksbar
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct APPBARDATA
{
	public uint cbSize;
	public nint hWnd;
	public uint uCallbackMessage;
	public uint uEdge;
	public Rectangle rc;
	public uint lParam;
}

