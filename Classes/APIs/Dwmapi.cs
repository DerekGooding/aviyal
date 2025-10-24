/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

public partial class Dwmapi
{
	[LibraryImport("dwmapi.dll", SetLastError = true)]
	public static partial int DwmSetWindowAttribute(nint hWnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

	[LibraryImport("dwmapi.dll", SetLastError = true)]
	public static partial int DwmGetWindowAttribute(
		nint hWnd,
		uint dwAttribute,
		nint pvAttribute,
		uint cbAttribute
	);

	[LibraryImport("dwmapi.dll", SetLastError = true)]
	public static partial int DwmGetWindowAttribute(
		nint hWnd,
		uint dwAttribute,
		out uint pvAttribute,
		uint cbAttribute
	);
}
