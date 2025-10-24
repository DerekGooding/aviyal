/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct UNICODE_STRING
{
	public ushort Length;
	public ushort MaximumLength;
	public nint Buffer;
}

