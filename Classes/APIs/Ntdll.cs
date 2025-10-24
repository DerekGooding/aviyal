/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using aviyal.Classes.Structs;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

/// <summary>
/// Query kernel objects
/// </summary>
public partial class Ntdll
{
	[LibraryImport("ntdll.dll", SetLastError = true)]
	public static partial int NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS infoType, ref SYSTEM_PROCESS_ID_INFORMATION info, uint infoLength, out uint returnLength);

	[LibraryImport("ntdll.dll", SetLastError = true)]
	public static partial int NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS infoType, ref SYSTEM_BASIC_INFORMATION info, uint infoLength, out uint returnLength);

	[LibraryImport("ntdll.dll", SetLastError = true)]
	public static partial int NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS infoType, nint info, uint infoLength, out uint returnLength);

}
