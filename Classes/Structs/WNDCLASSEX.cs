/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;
using aviyal.Classes.Delegates;

namespace aviyal.Classes.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct WNDCLASSEX
{
	public uint cbSize;
	public uint style;
	public WNDPROC lpfnWndProc;
	public int cbClsExtra;
	public int cbWndExtra;
	public nint hInstance;
	public nint hIcon;
	public nint hCurosr;
	public nint hbrBackground;
	public string lpszMenuName;
	public string lpszClassName;
	public nint hIconSm;
}

