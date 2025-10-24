/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using aviyal.Classes.Structs;
using System.Runtime.InteropServices;

namespace aviyal.Classes.APIs;

public partial class Advapi32
{
	[LibraryImport("advapi32.dll", SetLastError = true)]
	public static partial int OpenProcessToken(nint handle, uint processAccess, out nint tokenHandle);

	[LibraryImport("advapi32.dll", SetLastError = true)]
	public static partial int GetTokenInformation(nint handle, TOKEN_INFORMATION_CLASS informationClass, ref TOKEN_ELEVATION info, uint infoSize, out uint returnLength);
}
