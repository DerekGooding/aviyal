/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Delegates;
using aviyal.Classes.Enums;
using aviyal.Classes.Structs;
using System.Runtime.InteropServices;
using System.Text;

namespace aviyal.Classes.APIs;

public static partial class User32
{
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int MessageBox(nint hWnd, string message, string title, uint type);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial uint GetWindowLong(nint hWnd, GETWINDOWLONG nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

    [LibraryImport("user32.dll")]
    public static partial int GetWindowInfo(nint hWnd, out WINDOWINFO info);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, SETWINDOWPOS uFlags);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int GetSystemMetrics(int nIndex);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint FindWindow(string className, string windowName);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int ShowWindow(nint hWnd, SHOWWINDOW nCmdShow);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int EnableWindow(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool enable);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int RedrawWindow(nint hWnd, nint redrawRect, nint hrgnUpdate, REDRAWWINDOW flags);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int UpdateWindow(nint hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int InvalidateRect(nint hWnd, nint lpRect, [MarshalAs(UnmanagedType.Bool)] bool bgErase);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint FindWindowEx(nint hWndParent, nint hWndChildAfter, string className, string windowName);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int ShowWindowAsync(nint hWnd, SHOWWINDOW nCmdShow);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowPlacement(nint hWnd, ref WINDOWPLACEMENT lpwndpl);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out POINT cursorPos);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(nint hWnd, out RECT windowRect);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetClientRect(nint hWnd, out RECT clientRect);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint GetForegroundWindow();

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SetForegroundWindow(nint hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int BringWindowToTop(nint hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int AllowSetForegroundWindow(uint dwProcessId);

    [LibraryImport("user32.dll")]
    public static partial nint WindowFromPoint(POINT Point);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetClassName(nint hWnd, StringBuilder lpClassName, int nMaxCount);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumWindows(EnumWindowProc enumWindowProc, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumChildWindows(nint hWndParent, EnumWindowProc enumWindowProc, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowThreadProcessId(nint hWnd, out uint processId);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint CreateWindowEx(
        WINDOWSTYLE dwExStyle,
        string lpClassName,
        string lpWindowName,
        WINDOWSTYLE dwStyle,
        int X,
        int Y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam
    );

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial ushort RegisterClassEx(ref WNDCLASSEX wc);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnregisterClass(string className, nint hInstance);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint DefWindowProc(nint hWnd, WINDOWMESSAGE uMsg, nint wParam, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int DestroyWindow(nint hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int GetMessage(out MSG msg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TranslateMessage(ref MSG msg);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DispatchMessage(ref MSG msg);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial void PostQuitMessage(int nExitCode);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial uint RegisterWindowMessage(string message);

    /// <summary>
    /// Use when msg is not in WINDOWMESSAGE enum, like in situations where a new boradcast
    /// message has to be sent
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SendNotifyMessage(
        nint hWnd,
        uint msg,
        nint wParam,
        nint lParam
    );

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SendMessage(
        nint hWnd,
        uint msg,
        nint wParam,
        nint lParam
    );

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial void keybd_event(
        byte vk, // virtual key code
        byte scan, // scan code that identifies the key irrespective of 
                    // active layout, An application typically ignores scan codes. Instead, it uses the virtual-key codes to interpret keystroke messages.
        uint flags,
        nint extra
    );

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial void SetTimer(nint hWnd, nint nIdEvent, uint uElapse, TIMERPROC timerProc);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint GetAncestor(
        nint hwnd,
        uint gaFlags
    );

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint GetLastActivePopup(
        nint hWnd
    );

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsWindowVisible(nint hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int AnimateWindow(nint hWnd, uint dwTime, ANIMATEWINDOW dwFlags);

    // return hMonitor or the monitor handle

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint MonitorFromPoint(POINT pt, uint dwFlags);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint MonitorFromWindow(nint hWnd, uint dwFlags);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint SetParent(nint hWndChild, nint hWndNewParent);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AttachThreadInput(uint idAttach, uint idAttachTo, [MarshalAs(UnmanagedType.Bool)] bool fAttach);

}
