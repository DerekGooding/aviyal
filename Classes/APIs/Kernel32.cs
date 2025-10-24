/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;
using System.Text;

namespace aviyal.Classes.APIs;

public class Kernel32
{
	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool AttachConsole(int processId);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool FreeConsole();

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern nint GetConsoleWindow();

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern nint GetModuleHandle(string moduleName);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern nint OpenProcess(uint processAccess, bool bInheritHandle, int processId);

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

	[DllImport("kernel32.dll")]
	public static extern uint GetCurrentThreadId();

}
