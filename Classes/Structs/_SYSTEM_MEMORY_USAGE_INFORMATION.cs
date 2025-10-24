/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// used by SYSTEM_INFORMATION_CLASS.SystemMemoryUsageInformation
/// https://ntdoc.m417z.com/system_memory_usage_information
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct _SYSTEM_MEMORY_USAGE_INFORMATION
{
	public ulong TotalPhysicalBytes;
	public ulong AvailableBytes;
	public long ResidentAvailableBytes;
	public ulong CommittedBytes;
	public long SharedCommittedBytes;
	public long CommitLimitBytes;
	public long PeakCommitmentBytes;
}

