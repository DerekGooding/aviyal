using aviyal.Classes.APIs;
using aviyal.Classes.Structs;
using aviyal.Classes.Win32;
using aviyal.Interfaces;

namespace aviyal.Classes;

public class Workspace : IWorkspace
{
	public Guid id { get; } = Guid.NewGuid();
	public List<Window?> windows { get; private set; } = new();
	public Window? focusedWindow
	{
		get
		{
			return windows.FirstOrDefault(_wnd => _wnd == new Window(User32.GetForegroundWindow()));
		}
		private set;
	}
	public int? focusedWindowIndex
	{
		get
		{
			int? index = null;
			if (focusedWindow == null) return null;
			for (int i = 0; i < windows.Count; i++)
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
	public ILayout layout { get; set; }

	public override bool Equals(object? obj)
	{
		if (obj is null) return this is null;
		if (((Workspace)obj).id == this.id) return true;
		return false;
	}

	public static bool operator ==(Workspace left, Workspace right)
	{
		if (left is null) return right is null;
		return left.Equals(right);
	}

	public static bool operator !=(Workspace left, Workspace right)
	{
		if (left is null) return right is not null;
		return !left.Equals(right);
	}

	Config config;
	(int, int) floatingWindowSize;
	public Workspace(Config config)
	{
		this.config = config;
		var sizeStrs = config.floatingWindowSize.Split("x");
		floatingWindowSize.Item1 = Convert.ToInt32(sizeStrs[0]);
		floatingWindowSize.Item2 = Convert.ToInt32(sizeStrs[1]);
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

		List<Window?> nonFloating = windows
		.Where(wnd => wnd?.resizeable == true)
		.Where(wnd => wnd?.floating == false)
		.Where(wnd => wnd?.state != SHOWWINDOW.SW_SHOWMAXIMIZED)
		.Where(wnd => wnd?.state != SHOWWINDOW.SW_SHOWMINIMIZED)
		.ToList();

		// non floating
		RECT[] relRects = layout.GetRects(nonFloating.Count);
		RECT[] rects = layout.ApplyInner(layout.ApplyOuter(relRects.ToArray()));
		for (int i = 0; i < nonFloating.Count; i++)
		{
			nonFloating[i]?.Move(rects[i]);
			nonFloating[i].relRect = relRects[i];
		}

		// floating
		List<Window?> floating = windows
		.Where(wnd => wnd?.resizeable == true)
		.Where(wnd => wnd?.floating == true)
		.Where(wnd => wnd?.state != SHOWWINDOW.SW_SHOWMAXIMIZED)
		.Where(wnd => wnd?.state != SHOWWINDOW.SW_SHOWMINIMIZED)
		.ToList();

		for (int i = 0; i < floating.Count; i++)
		{
			floating[i].relRect = floating[i].rect;
		}
	}

	public void Show()
	{
		windows?.ForEach(wnd => wnd?.Show());
	}

	public void Hide()
	{
		windows?.ForEach(wnd =>
		{
			//var (sx, sy) = Utils.GetScreenSize();
			//wnd?.Move(sx, sy);
			wnd?.Hide();
		});
	}

	public void Focus()
	{
		Update();
		Show();
		SetFocusedWindow();
	}

	public void Redraw()
	{
		windows?.ForEach(wnd => wnd?.Redraw());
	}

	public void Move(int? x, int? y, bool redraw = true)
	{
		windows.ForEach(wnd =>
		{
			wnd?.Move(wnd.relRect.Left + x, wnd.relRect.Top + y, redraw);
		});
	}

	Window? lastFocusedWindow = null;
	public void SetFocusedWindow()
	{
		if (lastFocusedWindow == null)
		{
			var wnd = windows?.FirstOrDefault();
			lastFocusedWindow = wnd;
			wnd?.Focus();
		}
		else lastFocusedWindow.Focus();
	}

	public void CloseFocusedWindow()
	{
		int? index = focusedWindowIndex;
		if (index == null) return;
		index = index > 0 ? index - 1 : 0;
		focusedWindow?.Close();
		windows.ElementAtOrDefault((int)index)?.Focus();
	}

	public void FocusAdjacentWindow(EDGE direction)
	{
		//Console.WriteLine($"focusing window on: {direction}, focusedWindowIndex: {focusedWindowIndex}");
		if (focusedWindowIndex == null) return;
		int? index = layout.GetAdjacent((int)focusedWindowIndex, direction);
		if (index != null) windows?[(int)index]?.Focus();
	}

	public void ShiftFocusedWindow(int shiftBy)
	{
		Window? _fwnd = focusedWindow;
		int? index = focusedWindowIndex;
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
		wnd.Move(GetCenterRect(floatingWindowSize.Item1, floatingWindowSize.Item2));
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
		(int sw, int sh) = Utils.GetScreenSize();
		return new()
		{
			Left = (int)((sw - w) / 2),
			Right = (int)((sw + w) / 2),
			Top = (int)((sh - h) / 2),
			Bottom = (int)((sh + h) / 2),
		};
	}

	public void SwapWindows(Window wnd1, Window wnd2)
	{
		if (!windows.Contains(wnd1) || !windows.Contains(wnd2)) return;
		int wnd1_index = windows.Index().First(iwnd => iwnd.Item == wnd1).Index;
		int wnd2_index = windows.Index().First(iwnd => iwnd.Item == wnd2).Index;
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
