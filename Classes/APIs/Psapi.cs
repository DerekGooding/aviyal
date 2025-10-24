/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;
using System.Text;

namespace aviyal.Classes.APIs;

public static class Psapi
{
	[DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetModuleFileNameEx(nint hProcess, nint hModule, out StringBuilder moduleFileName, uint nSize);
}
