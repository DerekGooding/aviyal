/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

/// <summary>
/// Very delicate struct, you might also notice that hWnd is an uint instead of the usual nint
/// (IntPtr)
/// </summary>

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct NOTIFYICONDATA
{
	public uint cbSize;
	/// <summary>
	/// Window handle of the message processing window for the tray icon. This is NOT
	/// the handle to the actual icon's window, the actual icon might not even have a window
	/// to begin with (which is the case with XAML elements)
	/// </summary>
	public uint hWnd;
	public uint uID;
	public uint uFlags;
	/// <summary>
	/// SendMessage(hWnd, uCallbackMessage, ..., ...)
	/// Wait what ? ......^...
	/// isnt it supposed to be a window message defined in WINDOWMESSAGE such as WM_CONTEXTMENU
	/// or WM_RIGHTBUTTONDOWN ? well the actual window the gets the WM_RIGHTBUTTONDOWN when
	/// the icon is rightclicked is the window hoisting the icon TopLevelXamlOverflowWindow
	/// or even Shell_TrayWnd. It then requests the message processing window of the icon (window with handle hWnd)
	/// for a context menu.
	/// </summary>
	public uint uCallbackMessage;
	public uint hIcon;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
	public string szTip;
	public uint dwState;
	public uint dwStateMask;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
	public string szInfo;
	public TIMEOUTVERSIONUNION uTimeoutOrVersion;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
	public string szInfoTitle;
	public uint dwInfoFlags;
	public Guid guidItem;
	public uint hBalloonIcon;
}

