/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

public class Dwmapi
{
	[DllImport("dwmapi.dll", SetLastError = true)]
	public static extern int DwmSetWindowAttribute(nint hWnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

	[DllImport("dwmapi.dll", SetLastError = true)]
	public static extern int DwmGetWindowAttribute(
		nint hWnd,
		uint dwAttribute,
		nint pvAttribute,
		uint cbAttribute
	);

	[DllImport("dwmapi.dll", SetLastError = true)]
	public static extern int DwmGetWindowAttribute(
		nint hWnd,
		uint dwAttribute,
		out uint pvAttribute,
		uint cbAttribute
	);
}
