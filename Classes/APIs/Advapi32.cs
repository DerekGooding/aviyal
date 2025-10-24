/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Structs;
using aviyal.Classes.Win32;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

public class Advapi32
{
	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern int OpenProcessToken(nint handle, uint processAccess, out nint tokenHandle);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern int GetTokenInformation(nint handle, TOKEN_INFORMATION_CLASS informationClass, ref TOKEN_ELEVATION info, uint infoSize, out uint returnLength);
}
