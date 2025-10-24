/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// Used by NtQuerySystemInformation in ntdll to query process module paths without
/// elevated priveleges. Part of the undocumented windows api
/// SYSTEM_INFORMATION_CLASS.SystemProcessIdInformation
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct SYSTEM_PROCESS_ID_INFORMATION
{
	public nint ProcessId;
	public UNICODE_STRING ImageName;
}

