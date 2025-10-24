/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;
using aviyal.Classes.Win32;

namespace aviyal.Classes.Structs;

/// <summary>
/// Win32 basic window message type used by SendMessage(), GetMessage(), TranslateMessage()
/// DispatchMessage() etc
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct MSG
{
	public nint hwnd;
	public WINDOWMESSAGE message;
	public nint wParam;
	public nint lParam;
	public uint time;
	public POINT pt;
	public uint lPrivate;
}

