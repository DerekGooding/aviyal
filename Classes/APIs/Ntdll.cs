/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Structs;
using aviyal.Classes.Win32;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

/// <summary>
/// Query kernel objects
/// </summary>
public class Ntdll
{
	[DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern int NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS infoType, ref SYSTEM_PROCESS_ID_INFORMATION info, uint infoLength, out uint returnLength);

	[DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern int NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS infoType, ref SYSTEM_BASIC_INFORMATION info, uint infoLength, out uint returnLength);

	[DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern int NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS infoType, nint info, uint infoLength, out uint returnLength);

}
