/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// NtQuerySystemInformation() can use it for basic querrying
/// used with SYSTEM_INFORMATION_CLASS.SystemBasicInformation
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct SYSTEM_BASIC_INFORMATION
{
	public uint Reserved;
	public uint TimerResolution;
	public uint PageSize;
	public uint NumberOfPhysicalPages;
	public uint LowestPhysicalPageNumber;
	public uint HighestPhysicalPageNumber;
	public uint AllocationGranularity;
	public nuint MinimumUserModeAddress;
	public nuint MaximumUserModeAddress;
	public nuint ActiveProcessorsAffinityMask;
	public byte NumberOfProcessors;
}

