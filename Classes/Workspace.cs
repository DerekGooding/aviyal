using aviyal.Classes.APIs;
using aviyal.Classes.Enums;
using aviyal.Classes.Structs;
using aviyal.Interfaces;

namespace aviyal.Classes;

public class Workspace : IWorkspace
{
	public Guid Id { get; } = Guid.NewGuid();
    public List<Window?> windows { get; } = [];
    public Window? focusedWindow => windows.FirstOrDefault(_wnd => _wnd == new Window(User32.GetForegroundWindow()));
    public int? focusedWindowIndex
	{
		get
		{
			int? index = null;
			if (focusedWindow == null) return null;
			for (var i = 0; i < windows.Count; i++)
			{
				if (windows[i] == focusedWindow)
				{
					index = i;
					break;
				}
			}
			return index;
		}
	}
	public ILayout? layout { get; set; }

    public override bool Equals(object? obj) => obj is null ? this is null : ((Workspace)obj).Id == Id;
    public static bool operator ==(Workspace left, Workspace right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(Workspace left, Workspace right) => left is null ? right is not null : !left.Equals(right);

    readonly Config config;
	(int, int) _floatingWindowSize;
	public Workspace(Config config)
	{
		this.config = config;
		var sizeStrs = config.floatingWindowSize.Split("x");
		_floatingWindowSize.Item1 = Convert.ToInt32(sizeStrs[0]);
		_floatingWindowSize.Item2 = Convert.ToInt32(sizeStrs[1]);
	}

	public void Add(Window wnd)
	{
		windows.Add(wnd);
		Update();
	}
	public void Remove(Window wnd)
	{
		windows.Remove(wnd);
		Update();
	}

	// applies updated relRects (provided by the layout) to the windows in the workspace
	public void Update()
	{
		//windows.ForEach(wnd => //Console.WriteLine($"WND IS NULL: {wnd == null}"));

		List<Window?> nonFloating = [.. windows.Where(wnd => wnd?.resizeable == true && wnd?.floating == false && wnd?.state != SHOWWINDOW.SW_SHOWMAXIMIZED && wnd?.state != SHOWWINDOW.SW_SHOWMINIMIZED)];

		// non floating
		var relRects = layout.GetRects(nonFloating.Count);
		var rects = layout.ApplyInner(layout.ApplyOuter(relRects.ToArray()));
		for (var i = 0; i < nonFloating.Count; i++)
		{
			nonFloating[i]?.Move(rects[i]);
			nonFloating[i].relRect = relRects[i];
		}

		// floating
		List<Window?> floating = [.. windows.Where(wnd => wnd?.resizeable == true && wnd?.floating == true && wnd?.state != SHOWWINDOW.SW_SHOWMAXIMIZED && wnd?.state != SHOWWINDOW.SW_SHOWMINIMIZED)];

		for (var i = 0; i < floating.Count; i++)
		{
			floating[i].relRect = floating[i].rect;
		}
	}

    public void Show() => windows?.ForEach(wnd => wnd?.Show());

    //var (sx, sy) = Utils.GetScreenSize();
    //wnd?.Move(sx, sy);
    public void Hide() => windows?.ForEach(wnd => wnd?.Hide());

    public void Focus()
	{
		Update();
		Show();
		SetFocusedWindow();
	}

    public void Redraw() => windows?.ForEach(wnd => wnd?.Redraw());

    public void Move(int? x, int? y, bool redraw = true) => windows.ForEach(wnd => wnd?.Move(wnd.relRect.Left + x, wnd.relRect.Top + y, redraw));

    Window? _lastFocusedWindow;
	public void SetFocusedWindow()
	{
        if (_lastFocusedWindow == null)
        {
            var wnd = windows?.FirstOrDefault();
            _lastFocusedWindow = wnd;
            wnd?.Focus();
        }
        else
        {
            _lastFocusedWindow.Focus();
        }
    }

	public void CloseFocusedWindow()
	{
		var index = focusedWindowIndex;
		if (index == null) return;
		index = index > 0 ? index - 1 : 0;
		focusedWindow?.Close();
		windows.ElementAtOrDefault((int)index)?.Focus();
	}

	public void FocusAdjacentWindow(EDGE direction)
	{
		//Console.WriteLine($"focusing window on: {direction}, focusedWindowIndex: {focusedWindowIndex}");
		if (focusedWindowIndex == null) return;
		var index = layout.GetAdjacent((int)focusedWindowIndex, direction);
		if (index != null) windows?[(int)index]?.Focus();
	}

	public void ShiftFocusedWindow(int shiftBy)
	{
		var _fwnd = focusedWindow;
		var index = focusedWindowIndex;
		if (index == null) return;
		index += shiftBy;
		//Console.WriteLine($"SHIFTING");
		if (index < 0 || index > windows.Count - 1) return;
		windows.Remove(_fwnd);
		windows.Insert((int)index, _fwnd);
		Focus();
	}

	public void MakeFloating(Window wnd)
	{
		if (!wnd.resizeable) return;
		wnd.Move(GetCenterRect(_floatingWindowSize.Item1, _floatingWindowSize.Item2));
	}

	public void ToggleFloating(Window? wnd = null)
	{
		if (wnd == null) wnd = focusedWindow;
		if (wnd == null) return;
		wnd.floating = !wnd.floating;
		//Console.WriteLine($"[ TOGGLE FLOATING ] : {wnd.floating}, [ {config.floatingWindowSize} ]");
		if (wnd.floating && wnd.resizeable) MakeFloating(wnd);
		Update();
	}

	RECT GetCenterRect(int w, int h)
	{
		(var sw, var sh) = Utils.GetScreenSize();
		return new()
		{
			Left = (sw - w) / 2,
			Right = (sw + w) / 2,
			Top = (sh - h) / 2,
			Bottom = (sh + h) / 2,
		};
	}

	public void SwapWindows(Window wnd1, Window wnd2)
	{
		if (!windows.Contains(wnd1) || !windows.Contains(wnd2)) return;
		var wnd1_index = windows.Index().First(iwnd => iwnd.Item == wnd1).Index;
		var wnd2_index = windows.Index().First(iwnd => iwnd.Item == wnd2).Index;
		windows[wnd1_index] = wnd2;
		windows[wnd2_index] = wnd1;
		Update();
	}

	public Window? GetWindowFromPoint(POINT pt)
	{
		return windows.FirstOrDefault(wnd =>
		{
			return wnd?.relRect.Left < pt.X && pt.X < wnd?.relRect.Right &&
				   wnd?.relRect.Top < pt.Y && pt.Y < wnd?.relRect.Bottom;
		});
	}
}
