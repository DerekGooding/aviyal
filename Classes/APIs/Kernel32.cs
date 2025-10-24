/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;
using System.Text;

namespace aviyal.Classes.APIs;

public partial class Kernel32
{
	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AttachConsole(int processId);

	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FreeConsole();

	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	public static partial nint GetConsoleWindow();

	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	public static partial nint GetModuleHandle(string moduleName);

	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	public static partial nint OpenProcess(uint processAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int processId);

	[DllImport("kernel32.dll")]
	public static extern uint GetLogicalDriveStringsW(
	  uint nBufferLength,
	  StringBuilder lpBuffer
	);

	[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern uint QueryDosDevice(
	  string lpDeviceName,
	  StringBuilder lpTargetPath,
	  uint ucchMax
	);

	[LibraryImport("kernel32.dll")]
	public static partial uint GetCurrentThreadId();

}
