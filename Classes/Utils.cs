/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using aviyal.Classes.Utilities;
using aviyal.Classes.APIs;
using aviyal.Classes.Structs;
using aviyal.Classes.Enums;
using aviyal.Classes.Delegates;

namespace aviyal.Classes;

public static class Utils
{
    public static List<string> GetStylesFromHwnd(nint hWnd)
    {
        var dwStyles = User32.GetWindowLong(hWnd, GETWINDOWLONG.GWL_STYLE);
        List<string> styles = [];
        foreach (var name in Enum.GetNames<WINDOWSTYLE>())
        {
            var style = Enum.Parse<WINDOWSTYLE>(name);
            if (style == WINDOWSTYLE.WS_OVERLAPPED)
            {
                if ((dwStyles & ((uint)WINDOWSTYLE.WS_POPUP | (uint)WINDOWSTYLE.WS_CHILD)) == 0)
                {
                    styles.Add(name);
                }
                else
                {
                    if ((dwStyles & (uint)style) != 0) styles.Add(name);
                }
            }
        }
        return styles;
    }

    public static bool IsContextMenu(nint hWnd)
    {
        var styleList = GetStylesFromHwnd(hWnd);
        //Logger.Log($"IsContextMenu(): {Marshal.GetLastWin32Error()}");
        if (styleList.Contains("WS_POPUP")) return true;

        var className = GetClassNameFromHWND(hWnd);
        return className is "#32768" or "#32770" or "SysListView32"
            or "SysShadow" or "TrayiconMessageWindow" or "tray_icon_app";
    }

    public static bool IsWindowVisible(nint hWnd)
    {
        var styleList = GetStylesFromHwnd(hWnd);
        //Logger.Log($"IsWindowVisible(): {Marshal.GetLastWin32Error()}");
        return styleList.Contains("WS_VISIBLE");
    }

    public static nint GetWindowUnderCursor()
    {
        User32.GetCursorPos(out var pt);
        return User32.WindowFromPoint(pt);
    }

    public static void MoveWindowToCursor(nint hWnd, int offsetX = 0, int offsetY = 0)
    {
        User32.GetCursorPos(out var cursorPos);
        User32.SetWindowPos(hWnd, nint.Zero, cursorPos.X + offsetX, cursorPos.Y + offsetY, 0, 0, SETWINDOWPOS.SWP_NOSIZE);
    }

    public static void MoveWindow(nint hWnd, int x, int y)
    {
        User32.SetWindowPos(hWnd, nint.Zero, x, y, 0, 0, SETWINDOWPOS.SWP_NOSIZE);
    }

    public static string GetClassNameFromHWND(nint hWnd)
    {
        StringBuilder str = new(256);
        User32.GetClassName(hWnd, str, str.Capacity);
        return str.ToString();
    }

    public static (int, int) GetWindowDimensions(nint hWnd)
    {
        User32.GetWindowRect(hWnd, out var rect);
        var Width = rect.Right - rect.Left;
        var Height = rect.Bottom - rect.Top;
        return (Width, Height);
    }

    /// <summary>
    /// Get the top level window matching the given pid
    /// </summary>
    /// <param name="processId"></param>
    /// <returns></returns>
    public static nint GetHWNDFromPID(int processId)
    {
        nint found_hWnd = new();
        bool enumWindowProc(nint hWnd, nint lParam)
        {
            User32.GetWindowThreadProcessId(hWnd, out var _processId);
            if (_processId == processId)
            {
                found_hWnd = hWnd;
                return false;
            }
            return true;
        }
        User32.EnumWindows(enumWindowProc, processId);
        return found_hWnd;
    }

    /// <summary>
    /// Enumerate ALL windows (including children)
    /// </summary>
    /// <returns></returns>
    public static List<GUIProcess> EnumWindowProcesses()
    {
        List<GUIProcess> guiProcesses = [];
        bool enumWindowProc(nint hWnd, nint lParam)
        {
            User32.GetWindowThreadProcessId(hWnd, out var processId);
            var process = Process.GetProcessById((int)processId);
            GUIProcess guiProcess;
            if ((guiProcess = guiProcesses.Where(_p => _p.name == process.ProcessName).FirstOrDefault()) == null)
            {
                guiProcess = new() { name = process.ProcessName };
                guiProcesses.Add(guiProcess);
            }
            guiProcess.process = process;
            SubWindow window = new();
            window.hWnd = hWnd;
            window.className = GetClassNameFromHWND(hWnd);
            guiProcess.windows.Add(window);
            EnumWindowProc enumChildWindowProc = (c_hWnd, lParam) =>
            {
                SubWindow c_window = new();
                c_window.hWnd = c_hWnd;
                c_window.className = GetClassNameFromHWND(c_hWnd);
                guiProcess.windows.Add(c_window);
                return true;
            };
            User32.EnumChildWindows(hWnd, enumChildWindowProc, nint.Zero);
            return true;
        }
        User32.EnumWindows(enumWindowProc, nint.Zero);
        return guiProcesses;
    }

    public static string? GetExePathFromHWND(nint hWnd)
    {
        User32.GetWindowThreadProcessId(hWnd, out var processId);

        if (Environment.IsPrivilegedProcess)
        {
            var allWindows = EnumWindowProcesses();
            var process = allWindows.Where(guiProcess => guiProcess.process.Id == processId).FirstOrDefault()?.process;
            return process?.MainModule?.FileName;
        }

        /// <summary>
        /// Getting module filenames without elevated privileges
        /// NtQuerySystemInformation() := undocumented internal API
        /// https://stackoverflow.com/a/75084784/14588925
        /// </summary>
        SYSTEM_PROCESS_ID_INFORMATION info = new() { ProcessId = (nint)processId, ImageName = new() { Length = 0, MaximumLength = 256, Buffer = Marshal.AllocHGlobal(512) } };
        var result = Ntdll.NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemProcessIdInformation, ref info, (uint)Marshal.SizeOf<SYSTEM_PROCESS_ID_INFORMATION>(), out var returnLength);
        var exePath = Marshal.PtrToStringUni(info.ImageName.Buffer);
        Marshal.FreeHGlobal(info.ImageName.Buffer);
        if (exePath == null) return null;

        // List all device paths
        List<string> driveDevicePaths = [];
        List<string> driveNames = [];
        Dictionary<string, string> devicePathToDrivePath = [];
        driveNames = DriveInfo.GetDrives().Select(drive => drive.Name.Substring(0, 2)).ToList();
        driveDevicePaths = driveNames.ConvertAll(drive =>
        {
            StringBuilder str = new(256);
            Kernel32.QueryDosDevice(drive, str, (uint)str.Capacity);
            var devicePath = str.ToString();
            devicePathToDrivePath[devicePath] = drive;
            return devicePath;
        });

        //
        var exePathDeviceName = driveDevicePaths.Where(path => exePath.Contains(path)).FirstOrDefault();
        if (exePathDeviceName == null) return null;
        var exePathDriveName = devicePathToDrivePath[exePathDeviceName];

        var exeNtPath = Path.Join(exePathDriveName, exePath.Replace(exePathDeviceName, ""));
        return exeNtPath;
    }

    public static int MAKEWPARAM(short L, short H) => H << 16 | (int)L;

    public static int MAKELPARAM(short L, short H) => H << 16 | (int)L;

    /// <summary>
    /// Determines if a window is visible and running in the Taskbar/Alt-Tab
    /// not the pinned icons in the taskbar, source:
    /// https://stackoverflow.com/questions/210504/enumerate-windows-like-alt-tab-does
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    public static bool IsWindowInTaskBar(nint hWnd)
    {
        // filter out the obvious -------------------------
        if (!User32.IsWindowVisible(hWnd)) return false;

        var exStyle = User32.GetWindowLong(hWnd, GETWINDOWLONG.GWL_EXSTYLE);
        if (exStyle.ContainsFlag((uint)WINDOWSTYLEEX.WS_EX_TOOLWINDOW)) return false;
        //if (exStyle.ContainsFlag((uint)WINDOWSTYLE.WS_EX_APPWINDOW)) return false;

        var className = GetClassNameFromHWND(hWnd);
        var title = GetWindowTitleFromHWND(hWnd);
        //if (className == "Windows.UI.Core.CoreWindow")
        //{
        //	if (title == "Settings") return true;
        //	return false;
        //}
        //if (className == "ApplicationFrameWindow")
        //{
        //	return false;
        //}
        // ---------------------------------------------------------------
        Dwmapi.DwmGetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out var cloak, sizeof(uint));
        //Console.WriteLine($"[CLOAK], title: {title}, class: {className}, cloak: {cloak}");
        return cloak == 0;

        /*
        // https://devblogs.microsoft.com/oldnewthing/20071008-00/?p=24863 
        const int GA_ROOTOWNER = 3;
        // start at the owner window
        nint hWndWalk = User32.GetAncestor(hWnd, GA_ROOTOWNER);

        nint hWndTry;
        // a window in taskbar / alt-tab is its own last popup window, so loop until hWnd walk becomes a popup window
        while ((hWndTry = User32.GetLastActivePopup(hWndWalk)) != hWndWalk)
        {
            if (IsWindowVisible(hWndTry)) break;
            hWndWalk = hWndTry;
        }
        // once the walk is finished hWndWalk "is" the taskbarwindow in that owner chain, now check if the window you supplied is that window
        return hWnd == hWndWalk;
        */
    }

    /// <summary>
    /// All normal applications with a taskbar icon
    /// </summary>
    /// <returns></returns>
    public static List<nint>? GetAllTaskbarWindows()
    {
        List<nint>? topWindows = [];
        EnumWindowProc enumWnd = (hWnd, lParam) =>
        {
            topWindows.Add(hWnd);
            return true;
        };
        User32.EnumWindows(enumWnd, nint.Zero);
        var taskbarWindows = topWindows.Where(hWnd => IsWindowInTaskBar(hWnd)).ToList();
        //taskbarWindows.ForEach(hWnd => Logger.Log($"TASKBAR WINDOWS, hWnd: {hWnd}, class: {GetClassNameFromHWND(hWnd)}, exe: {GetExePathFromHWND(hWnd)}"));
        return taskbarWindows;
        //return topWindows;
    }

    /// <summary>
    /// Retreives the local lan ip assigned to your pc in your LAN network
    /// usually in the form 192.168.XX.XX
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetLANIP() => NetworkInterface.GetAllNetworkInterfaces()
            .ToList()
            .Select(iface => iface.GetIPProperties().UnicastAddresses
                .Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork && addr.PrefixOrigin == PrefixOrigin.Dhcp)
            )
            .Where(list => list.Any())
            .ToList()[0]
            .ToList()[0]
            .Address;

    /// <summary>
    /// Retrieves the primary network interface in your pc that you 
    /// use for internet, required for monitoring network bandwidths
    /// and speeds. The idea is that the interface that is used for internet
    /// has the local lan ip
    /// </summary>
    /// <returns></returns>
    public static NetworkInterface GetPrimaryNetworkInterface()
    {
        var addr = GetLANIP();
        var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();
        return interfaces.First(iface => iface.GetIPProperties().UnicastAddresses.Select(ucast => ucast.Address).Contains(addr));
    }

    public static int GetInterfaceIndex(NetworkInterface iface) => iface.GetIPProperties().GetIPv4Properties().Index;

    public static string GetWindowTitleFromHWND(nint hWnd)
    {
        StringBuilder str = new(256);
        User32.GetWindowText(hWnd, str, str.Capacity);
        return str.ToString();
    }

    public static double GetDisplayScaling()
    {
        var hMon = User32.MonitorFromPoint(new POINT() { X = 0, Y = 0 }, 0x01);
        Shcore.GetDpiForMonitor(hMon, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);
        //Console.WriteLine($"dpiX: {dpiX}, dpiY: {dpiY}");
        return dpiX / 96.0f;
    }

    //public static double GetDisplayScaling()
    //{
    //	nint hMon = User32.MonitorFromPoint(new POINT() { X = 0, Y = 0 }, 0x01);
    //	Shcore.GetScaleFactorForMonitor(hMon, out DEVICE_SCALE_FACTOR scale);
    //	//Console.WriteLine($"SCALE: {scale}, hMon: {hMon}");
    //	return ((double)scale) / 100;
    //}

    /// <summary>
    /// Hides window in the alt-tab window by (ADDING the WS_EX_TOOLWINDOW) and 
    /// (REMOVING the WS_EX_APPWINDOW) extended Styles
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    public static int HideWindowInAltTab(nint hWnd)
    {
        var exStyles = User32.GetWindowLong(hWnd, GETWINDOWLONG.GWL_EXSTYLE);
        return User32.SetWindowLong(
            hWnd,
            (int)GETWINDOWLONG.GWL_EXSTYLE,
            (int)((exStyles | (uint)WINDOWSTYLEEX.WS_EX_TOOLWINDOW) & ~(uint)WINDOWSTYLEEX.WS_EX_APPWINDOW)
        );
    }

    /// <summary>
    /// Make a window bottom most and stick to desktop by making it unfocusable
    /// This is required especially for creating widget windows that need to 
    /// always be on the background and never recieve focus. This is done by 
    /// adding the WS_EX_NOACTIVATE style.
    ///
    /// Still can flicker at activation and gain focus from alt tab selection,
    /// if the window is hidden in alt tab this shouldnt be a concern, however
    /// to avoid these problems sambar.WidgetWindow exists.
    /// </summary>
    public static void StickWindowToBottom(nint hWnd)
    {
        User32.SetWindowPos(hWnd, (nint)SWPZORDER.HWND_BOTTOM, 0, 0, 0, 0, SETWINDOWPOS.SWP_NOMOVE | SETWINDOWPOS.SWP_NOSIZE | SETWINDOWPOS.SWP_NOACTIVATE);
        var exStyles = User32.GetWindowLong(hWnd, GETWINDOWLONG.GWL_EXSTYLE);
        User32.SetWindowLong(hWnd, (int)GETWINDOWLONG.GWL_EXSTYLE, (int)(exStyles | (uint)WINDOWSTYLEEX.WS_EX_NOACTIVATE));
    }

    /// <summary>
    /// Get the scaled display resolution of the screen
    /// </summary>
    public static (int, int) GetScreenSize()
    {
        var scale = GetDisplayScaling();
        var screenWidth = User32.GetSystemMetrics(0);
        var screenHeight = User32.GetSystemMetrics(1);
        screenWidth = screenWidth;
        screenHeight = screenHeight;
        //Console.WriteLine($"[{screenWidth}x{screenHeight}], scale: {scale}");
        return (screenWidth, screenHeight);
    }

    /// <summary>
    /// Scale an image so that it fills the target rect (width*height) completely
    /// </summary>
    public static (int, int) ScaleImage(int imgWidth, int imgHeight, int targetWidth, int targetHeight)
    {
        var touchDimension = targetWidth / imgWidth > targetHeight / imgHeight ? "width" : "height";
        var scale = touchDimension == "width" ? (double)targetWidth / imgWidth : (double)targetHeight / imgHeight;

        imgWidth = (int)(scale * imgWidth);
        imgHeight = (int)(scale * imgHeight);

        if (imgWidth < targetWidth || imgHeight < targetHeight)
        {
            touchDimension = touchDimension == "width" ? "height" : "width";
            scale = touchDimension == "width" ? (double)targetWidth / imgWidth : (double)targetHeight / imgHeight;
            return ((int)(scale * imgWidth), (int)(scale * imgHeight));
        }
        else
        {
            return (imgWidth, imgHeight);
        }
    }

    public static bool ListContentEqual<T>(List<T> a, List<T> b)
    {
        if (a.Count != b.Count) return false;
        for (var i = 0; i < a.Count; i++)
        {
            if (!b.Contains(a[i]) || !a.Contains(b[i])) return false;
        }
        return true;
    }

    /// <summary>
    /// Check if another process is elevated without doing the open handle exception
    /// bullshit
    /// </summary>
    public static bool IsProcessElevated(int pid)
    {
        const int PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
        var handle = Kernel32.OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
        const int TOKEN_QUERY = 0x0008;
        Advapi32.OpenProcessToken(handle, TOKEN_QUERY, out var tokenHandle);
        TOKEN_ELEVATION info = new();
        Advapi32.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevation, ref info, sizeof(uint), out var returnLength);
        return info.TokenIsElevated != 0;
    }
}
