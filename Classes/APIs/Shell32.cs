/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Structs;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

public class Shell32
{
	[DllImport("shell32.dll", SetLastError = true)]
	public static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

	[DllImport("shell32.dll", SetLastError = true)]
	public static extern long Shell_NotifyIconGetRect(ref _NOTIFYICONIDENTIFIER identifier, out RECT iconLocation);

	[DllImport("shell32.dll", SetLastError = true)]
	public static extern uint ExtractIconEx(string exePath, int nIconIndex, out nint iconLarge, out nint iconSmall, uint nIcons);
}
