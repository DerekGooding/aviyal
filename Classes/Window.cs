using aviyal.Classes.APIs;
using aviyal.Classes.Enums;
using aviyal.Classes.Structs;
using aviyal.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace aviyal.Classes;

public class Window(nint hWnd) : IWindow
{
	public int workspace;
    public nint hWnd { get; } = hWnd;
    public string title => Utils.GetWindowTitleFromHWND(hWnd);
    public string className => Utils.GetClassNameFromHWND(hWnd);
    public string? exe
	{
		get
		{
			// EnumWindowProcess() is hella expensive, and anyway exe wont change
			// for a process once its found
			field ??= Utils.GetExePathFromHWND(hWnd) ??
						Utils.EnumWindowProcesses()
						.FirstOrDefault(wndProcess => wndProcess
										.windows.Select(wndp => wndp.hWnd)
						.Contains(hWnd))?
						.process.MainModule?.FileName;
			return field;
		}
	}
    public string ExeName => @$"{exe}"?.Split(@"\").Last().Replace(".exe", "")!;

    public RECT rect // absolute position
	{
		get
		{
			User32.GetWindowRect(hWnd, out var _rect);
			return _rect;
		}
	}

	public RECT relRect { get; set; } // position of window relative to workspace (without margins)

	public SHOWWINDOW state
	{
		get
		{
			WINDOWPLACEMENT wndPlmnt = new();
			User32.GetWindowPlacement(hWnd, ref wndPlmnt);
			var state = (SHOWWINDOW)wndPlmnt.showCmd;
			//Console.WriteLine($"state: {state}");
			return state;
		}
	}

    public bool resizeable => styles.HasFlag(WINDOWSTYLE.WS_THICKFRAME)
                && !className.Contains("OperationStatusWindow") // copy, paste status windows
                && !className.Contains("DS_MODALFRAME");

    public bool floating { get; set; }

    public int Pid
	{
		get
		{
			var _p = Process.GetProcessesByName(ExeName).FirstOrDefault();
			return (_p?.Id) ?? 0;
		}
	}
    //Console.WriteLine($"checking elevation of {title}: {Utils.IsProcessElevated(pid)}");
    public bool Elevated => Utils.IsProcessElevated(Pid);

    public WINDOWSTYLE styles => (WINDOWSTYLE)User32.GetWindowLong(hWnd, GETWINDOWLONG.GWL_STYLE);

    public WINDOWSTYLEEX exStyles => (WINDOWSTYLEEX)User32.GetWindowLong(hWnd, GETWINDOWLONG.GWL_EXSTYLE);

    public int BorderThickness
	{
		get
		{
			User32.GetWindowInfo(hWnd, out var info);
			return info.cxWindowBorders;
		}
	}
    //if (base.Equals(obj)) return true;
    public override bool Equals(object? obj) => obj is not null && ((Window)obj).hWnd == hWnd;

    public static bool operator ==(Window? left, Window? right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(Window? left, Window? right) => left is null ? right is not null : !left.Equals(right);

    public void Hide()
	{
		ToggleAnimation(false);
		User32.ShowWindow(hWnd, SHOWWINDOW.SW_HIDE);
		ToggleAnimation(true);
	}
	public void Show()
	{
		ToggleAnimation(false);
		User32.ShowWindow(hWnd, SHOWWINDOW.SW_SHOWNA);
		ToggleAnimation(true);
	}

	public void Focus()
	{
		User32.keybd_event(0, 0, 0, Globals.FOREGROUND_FAKE_KEY);
		User32.SetForegroundWindow(hWnd);
	}

	const SETWINDOWPOS _defaultMoveFlags =
		SETWINDOWPOS.SWP_NOSENDCHANGING |
		SETWINDOWPOS.SWP_NOCOPYBITS |
		SETWINDOWPOS.SWP_ASYNCWINDOWPOS |
		SETWINDOWPOS.SWP_NOACTIVATE |
		SETWINDOWPOS.SWP_NOZORDER;

	public void Move(RECT pos, bool redraw = true)
	{
		// remove frame bounds
		var margin = GetFrameMargin();
		pos.Left -= margin.Left;
		pos.Top -= margin.Top;
		pos.Right -= margin.Right;
		pos.Bottom -= margin.Bottom;

		var moveFlags = redraw switch
		{
			true => _defaultMoveFlags,
			false => _defaultMoveFlags | SETWINDOWPOS.SWP_NOREDRAW
		};

		User32.SetWindowPos(hWnd, 0, pos.Left, pos.Top, pos.Right - pos.Left, pos.Bottom - pos.Top, moveFlags);
	}

	const SETWINDOWPOS slideFlag = _defaultMoveFlags | SETWINDOWPOS.SWP_NOSIZE;
	public void Move(int? x, int? y, bool redraw = true)
	{
		var moveFlag = redraw switch
		{
			true => slideFlag,
			false => slideFlag | SETWINDOWPOS.SWP_NOREDRAW
		};
		User32.SetWindowPos(hWnd, 0, x ?? rect.Left, y ?? rect.Top, 0, 0, moveFlag);
	}

    public void Close() => User32.SendMessage(hWnd, (uint)WINDOWMESSAGE.WM_CLOSE, 0, 0);

    // force the window to redraw itself
    public void Redraw() => User32.RedrawWindow
	(hWnd, 0, 0,
        REDRAWWINDOW.INVALIDATE |
        REDRAWWINDOW.ALLCHILDREN |
        REDRAWWINDOW.UPDATENOW
    );

    public void SetBottom() => User32.SetWindowPos(hWnd, (nint)SWPZORDER.HWND_BOTTOM, 0, 0, 0, 0, SETWINDOWPOS.SWP_NOMOVE | SETWINDOWPOS.SWP_NOSIZE | SETWINDOWPOS.SWP_NOACTIVATE);

    public void SetFront() => User32.SetWindowPos(hWnd, (nint)SWPZORDER.HWND_TOP, 0, 0, 0, 0, SETWINDOWPOS.SWP_NOMOVE | SETWINDOWPOS.SWP_NOSIZE | SETWINDOWPOS.SWP_NOACTIVATE);

    public void ToggleAnimation(bool flag)
	{
		var attr = 0;
		if (!flag) attr = 1;
		var res = Dwmapi.DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_TRANSITIONS_FORCEDISABLED, ref attr, sizeof(int));
		////Console.WriteLine($"ToggleAnimation(): {res}");
	}

	public unsafe RECT GetFrameMargin()
	{
		User32.GetWindowRect(hWnd, out var rect);
		//Console.WriteLine($"GWR [L: {rect.Left} R: {rect.Right} T: {rect.Top} B:{rect.Bottom}]");
		var size = sizeof(RECT);
		var rectPtr = Marshal.AllocHGlobal(size);
		Dwmapi.DwmGetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, rectPtr, (uint)size);
		var rect2 = *(RECT*)rectPtr;
		Marshal.FreeHGlobal(rectPtr);
		//Console.WriteLine($"DWM [L: {rect2.Left} R: {rect2.Right} T: {rect2.Top} B:{rect2.Bottom}]");
		// scale dwm rect2 to take into account display scaling
		var scale = Utils.GetDisplayScaling();

		return new RECT()
		{
			Left = rect2.Left - rect.Left,
			Top = rect2.Top - rect.Top,
			Right = rect2.Right - rect.Right,
			Bottom = rect2.Bottom - rect.Bottom,
		};
	}

	RECT ScaleRect(RECT rect, double scale)
	{
		rect.Left = (int)(rect.Left * scale);
		rect.Top = (int)(rect.Top * scale);
		rect.Right = (int)(rect.Right * scale);
		rect.Bottom = (int)(rect.Bottom * scale);
		return rect;
	}

    bool RectEqual(RECT a, RECT b)
		=> a.Left == b.Left &&
            a.Top == b.Top &&
            a.Right == b.Right &&
            a.Bottom == b.Bottom;
}
