/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Structs;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// Pseudocode / Plan:
// 1. Disable runtime marshalling for the assembly so LibraryImport generated source
//    can handle complex struct marshalling (fixes SYSLIB1051 for ref APPBARDATA).
//    - Add assembly-level attribute: [assembly: DisableRuntimeMarshalling]
// 2. Provide explicit string marshalling for the ExtractIconEx P/Invoke to avoid
//    default runtime string marshalling (fixes SYSLIB1051 for string exePath).
//    - Use LibraryImportAttribute.StringMarshalling = StringMarshalling.Utf16
// 3. Keep other imports as-is. Ensure all using directives are present.
// 4. Result: SHAppBarMessage accepts ref APPBARDATA; ExtractIconEx uses UTF-16 string marshalling.
//
// Implementation notes:
// - Assembly attribute must be declared at top-level before the namespace/class.
// - String marshalling set to Utf16 to match Windows wide-char APIs (LPWSTR).
// - No changes to method signatures aside from explicit marshalling on the attribute.

[assembly: DisableRuntimeMarshalling]

namespace aviyal.Classes.APIs;

public partial class Shell32
{
	[LibraryImport("shell32.dll", SetLastError = true)]
	public static partial uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

	[LibraryImport("shell32.dll", SetLastError = true)]
	public static partial long Shell_NotifyIconGetRect(ref NOTIFYICONIDENTIFIER identifier, out RECT iconLocation);

	[LibraryImport("shell32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	public static partial uint ExtractIconEx(string exePath, int nIconIndex, out nint iconLarge, out nint iconSmall, uint nIcons);
}
