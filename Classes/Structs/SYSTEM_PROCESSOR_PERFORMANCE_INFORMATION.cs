/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// Used by SYSTEM_INFORMATION_CLASS.SystemProcessorPerformanceInformation
/// </summary>

[StructLayout(LayoutKind.Sequential)]
public struct SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION
{
	public long IdleTime;
	public long KernelTime;
	public long UserTime;
	public long DpcTime;
	public long InterruptTime;
	public uint Reserved2;
}

