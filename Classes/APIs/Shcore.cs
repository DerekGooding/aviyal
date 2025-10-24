/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

public partial class Shcore
{
	// retrieves monitor scaling info
	// MONITOR_DEFAULTTONULL
	// 0x00000000
	// Returns NULL.
	// MONITOR_DEFAULTTOPRIMARY
	// 0x00000001
	// Returns a handle to the primary display monitor.
	// MONITOR_DEFAULTTONEAREST
	// 0x00000002
	// Returns a handle to the display monitor that is nearest to the point.
	[LibraryImport("shcore.dll", SetLastError = true)]
	public static partial int GetScaleFactorForMonitor(nint hMon, out DEVICE_SCALE_FACTOR scaleFactor);

	// will only return the correct dpi if calling process is dpi aware
	// specifically PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE
	// https://stackoverflow.com/a/70020835/14588925
	[LibraryImport("shcore.dll", SetLastError = true)]
	public static partial int GetDpiForMonitor(nint hMon, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

	// call this with PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE
	// so that all functions like GetSystemMetric() and GetDpiForMonitor()
	// returns the actual/correct values
	[LibraryImport("shcore.dll", SetLastError = true)]
	public static partial int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);
}
