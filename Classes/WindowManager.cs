using aviyal.Classes.APIs;
using aviyal.Classes.Enums;
using aviyal.Classes.Structs;
using aviyal.Classes.Utilities;
using aviyal.Interfaces;
using System.Diagnostics;
using System.Linq;

namespace aviyal.Classes;

public class WindowManager : IWindowManager
{
	public List<Window>? initWindows { get; set; } // initWindows := initial set of windows to start the WM with
	public List<Workspace?> workspaces { get; } = [];
	public Workspace focusedWorkspace { get; private set; }

	public int focusedWorkspaceIndex
	{
		get
		{
			var index = 0;
			for (var i = 0; i < workspaces.Count; i++)
			{
				if (workspaces[i] == focusedWorkspace)
				{
					index = i;
					break;
				}
			}
			return index;
		}
	}
	public int WORKSPACES = 9;

	//Server server = new();
	Config config;
	public static bool DEBUG = false;
	public WindowManager(Config config)
	{
		this.config = config;
	}

	public void Start()
	{
		if (initWindows == null)
		{
			initWindows = GetVisibleWindows()!;
			initWindows = initWindows
						  .Where(wnd => !ShouldWindowBeIgnored(wnd))
						  .ToList();
			initWindows.ForEach(wnd => ApplyConfigsToWindow(wnd));
		}

		// when running in debug mode, only window containing the title "windowgen" will 
		// be managed by the program. This is so that your ide or terminal is left free
		// while testing
		if (DEBUG)
		{
			initWindows = initWindows.Where(wnd => wnd.title.Contains("windowgen")).ToList();
		}
		//initWindows.ForEach(wnd => //Console.WriteLine($"Title: {wnd.title}, hWnd: {wnd.hWnd}"));

		for (var i = 0; i < WORKSPACES; i++)
		{
			Workspace wksp = new(config);
			wksp.layout = new Dwindle(config);
			workspaces.Add(wksp);
		}
		// add all windows to 1st workspace
		initWindows.ForEach(wnd =>
		{
			wnd.workspace = 0;
			workspaces.FirstOrDefault()?.windows.Add(wnd);
		});
		FocusWorkspace(workspaces?.FirstOrDefault()!);
	}

	public List<Window?> GetVisibleWindows()
	{
		List<Window?> windows = [];
		var hWnds = Utils.GetAllTaskbarWindows();
		hWnds?.ForEach(hWnd =>
		{
			windows.Add(new(hWnd));
		});
		return windows;
	}

	public List<Window?> GetAllWindows()
	{
		List<Window?> windows = [];
        windows.AddRange(from wksp in workspaces
                         from wnd in wksp!.windows
                         select wnd);
        return windows;
	}

    // search for the window in our workspace and give a local reference that
    // has all the valid states set, the window instance emmitted by window event
    // listener gives a blank window that only matches the stateless properties
    // call this in all event handlers that deal with windows events of windows
    // that already exist in the workspace so basically every one except WindowShown
    Window? GetAlreadyStoredWindow(Window wnd) => focusedWorkspace?.windows?.FirstOrDefault(_wnd => _wnd == wnd);

    public void FocusWorkspace(Workspace wksp) => SuppressEvents(() =>
    {
        workspaces.ForEach(wksp => wksp?.Hide());
        wksp.Focus();
        focusedWorkspace = wksp;
    });

    // all workspace/window actions must be executed inside this wrapper function
    // This is to ensure that our own actions dont trigger the window events recursively
    readonly Lock @addLock = new();
	bool suppressEvents = false;
	void SuppressEvents(Action func)
	{
		lock (@addLock)
		{
			suppressEvents = true;
			func();
			Thread.Sleep(500);
			suppressEvents = false;
		}
	}

	public void FocusNextWorkspace()
	{

		var next = focusedWorkspaceIndex >= workspaces.Count - 1 ? 0 : focusedWorkspaceIndex + 1;
		var prev = focusedWorkspaceIndex > 0 ? focusedWorkspaceIndex - 1 : workspaces.Count - 1;

		if (config.workspaceAnimations)
		{
			// move left
			(var w, var h) = Utils.GetScreenSize();
			SuppressEvents(() =>
			{
				workspaces[next].Move(w, null);

				// we call Show() here instead of Focus() because Focus() has a call to Update()
				// if we Update() our Workspace then all the windows will be set to their
				// appropriate relRect effectively reversing Move(w, null). Hence as a result
				// you will see a flash of the next/prev workspace before it appears sliding.
				// So whats exactly going on ? Move(w, null) moves your workspace out of screen,
				// Focus() brings it back using Update() and Shows it until WorkspaceAnimate()
				// takes it out of screen as part of the animation start position which is also
				// beyond the screen.
				//Thread.Sleep(100);
				workspaces[next].Show();

				var duration = 500;
				List<Task> _ts =
                [
                    Task.Run(() => WorkspaceAnimate(focusedWorkspace, 0, -w, duration)),
                    Task.Run(() => WorkspaceAnimate(workspaces[next], w, 0, duration)),
                ];
				Task.WhenAll(_ts).Wait();
				focusedWorkspace.Hide();
				focusedWorkspace = workspaces[next];
				focusedWorkspace.Update(); // when animation finishes, margins dont match
				focusedWorkspace.Redraw(); // manually redraw
				focusedWorkspace.SetFocusedWindow();
			});
		}
		else
		{
			FocusWorkspace(workspaces[next]);
			//Console.WriteLine($"FOCUSING NEXT WORKSPACE, focusedWorkspaceIndex: {focusedWorkspaceIndex}");
		}

		SaveState("FocusNextWorkspace");
	}

	public void FocusPreviousWorkspace()
	{
		var next = focusedWorkspaceIndex >= workspaces.Count - 1 ? 0 : focusedWorkspaceIndex + 1;
		var prev = focusedWorkspaceIndex <= 0 ? workspaces.Count - 1 : focusedWorkspaceIndex - 1;

		if (config.workspaceAnimations)
		{
			// move right
			(var w, var h) = Utils.GetScreenSize();
			SuppressEvents(() =>
			{
				workspaces[prev].Move(-w, null);
				workspaces[prev].Show();

				var duration = 500;
				List<Task> _ts =
                [
                    Task.Run(() => WorkspaceAnimate(focusedWorkspace, 0, w, duration)),
                    Task.Run(() => WorkspaceAnimate(workspaces[prev], -w, 0, duration)),
                ];
				Task.WhenAll(_ts).Wait();
				focusedWorkspace.Hide();
				focusedWorkspace = workspaces[prev];
				focusedWorkspace.Update();
				focusedWorkspace.Redraw();
				focusedWorkspace.SetFocusedWindow();
			});
		}
		else
		{
			FocusWorkspace(workspaces[prev]);
			//Console.WriteLine($"FOCUSING PREVIOUS WORKSPACE, focusedWorkspaceIndex: {focusedWorkspaceIndex}");
		}

		SaveState("FocusPreviousWorkspace");
	}

	public void ShiftFocusedWindowToWorkspace(int index)
	{
		SuppressEvents(() =>
		{
			if (index < 0 || index > workspaces.Count - 1) return;
			var wnd = focusedWorkspace.focusedWindow;
			if (wnd == null) return;
			focusedWorkspace.Remove(wnd);
			wnd.workspace = index;
			workspaces[index].Add(wnd);
			FocusWorkspace(workspaces[index]);
			focusedWorkspace = workspaces[index];
			wnd.Focus();
		});

	}

	public void ShiftFocusedWindowToNextWorkspace()
	{
		var next = focusedWorkspaceIndex >= workspaces.Count - 1 ? 0 : focusedWorkspaceIndex + 1;
		ShiftFocusedWindowToWorkspace(next);

		SaveState("ShiftWindowToNextWorkspace");
	}

	public void ShiftFocusedWindowToPreviousWorkspace()
	{
		var prev = focusedWorkspaceIndex <= 0 ? workspaces.Count - 1 : focusedWorkspaceIndex - 1;
		ShiftFocusedWindowToWorkspace(prev);

		SaveState("ShiftWindowToPreviousWorkspace");
	}

	bool IsWindowInConfigRules(Window wnd, string ruleType)
	{
		var rules = config.rules.Where(rule => rule.type == ruleType).ToList();

		foreach (var rule in rules)
		{
			Func<string, string, bool> condition = rule.method switch
			{
				"equals" => (wndAttribute, identifier) => wndAttribute == identifier,
				"contains" => (wndAttribute, identifier) => wndAttribute.Contains(identifier),
				_ => (x, y) => false
			};

			var wndAttribute = rule.identifierType switch
			{
				"windowProcess" => wnd.exeName,
				"windowTitle" => wnd.title,
				"windowClass" => wnd.className,
				_ => ""
			};
			if (condition(wndAttribute, rule.identifier)) return true;
		}
		return false;
	}

	// filter out windows that should never be interacted with
	bool ShouldWindowBeIgnored(Window wnd)
	{
		// not required actually because WINDOW_ADDED only fires on OBJECT_SHOW
		// however adding for completeness
		if (!wnd.styles.HasFlag(WINDOWSTYLE.WS_VISIBLE)) return true;
		if (wnd.styles.HasFlag(WINDOWSTYLE.WS_CHILD)) return true;

		// all normal top level windows must have either "WS_OVERLAPPED" - OR - "WS_POPUP"
		// so kick out windows that dont have neither
		// WS_OVERLAPPED is the default style with which you get a normal window
		// since WS_OVERLAPPED = 0x00000000L it must be checked by the absence of both
		// WS_POPUP and WS_CHILD
		var isOverlapped = ((uint)wnd.styles & ((uint)WINDOWSTYLE.WS_POPUP | (uint)WINDOWSTYLE.WS_CHILD)) == 0;
		if (!isOverlapped &&
		   !wnd.styles.HasFlag(WINDOWSTYLE.WS_POPUP)
		) return true;

		if (wnd.exStyles.HasFlag(WINDOWSTYLEEX.WS_EX_TOOLWINDOW)) return true;
		if (wnd.exStyles.HasFlag(WINDOWSTYLEEX.WS_EX_TOPMOST)) return true;

		if (wnd.className == null || wnd.className == "") return true;

		if (wnd.className.Contains("#32770") &&
			!wnd.styles.HasFlag(WINDOWSTYLE.WS_SYSMENU) &&
			(wnd.rect.Bottom - wnd.rect.Top < 50 ||
			 wnd.rect.Right - wnd.rect.Left < 50)
			) return true; // dialogs

		// tooltips
		// https://learn.microsoft.com/en-us/windows/win32/controls/common-control-window-classes
		if (wnd.className.Contains("MicrosoftWindowsTooltip") ||
			wnd.className.Contains("tooltips_class32")
			) return true;

		// menus
		// https://learn.microsoft.com/en-us/windows/win32/winmsg/about-window-classes
		if (wnd.className.Contains("#32768") ||
			wnd.className.Contains("#32772")
			) return true;

		// filter out windows without the normal/default border thickness
		const int SM_CXSIZEFRAME = 32;
		if (wnd.BorderThickness < User32.GetSystemMetrics(SM_CXSIZEFRAME))
			return true;

		if (!Environment.IsPrivilegedProcess && wnd.Elevated) return true;

		if (IsWindowInConfigRules(wnd, "ignore"))
		{
			//Console.WriteLine($"ignoring {wnd.title} due to config rules");
			return true;
		}

		return false;
	}

	public void CleanGhostWindows()
	{
		lock (@addLock)
		{
			var visibleWindows = GetVisibleWindows();
			// visible windows will give all alt-tab programs, even tool windows
			// which we dont need and for whom winevents would typically not fire.
			// That is why whe have an '>' instead of an '!='
			// The reason we are doing all this is that for some windows such as
			// the file explorer, win events wont fire an OBJECT_SHOW when closing
			if (focusedWorkspace.windows.Count > visibleWindows.Count)
			{
				var ghostWindows = focusedWorkspace.windows.Where(wnd => !visibleWindows.Contains(wnd)).ToList();
				ghostWindows.ForEach(wnd => focusedWorkspace.Remove(wnd!));
				focusedWorkspace.Update();
			}

			// windows that have been added but has gone bad and should be removed
			var rottenWindows = focusedWorkspace.windows.Where(wnd => ShouldWindowBeIgnored(wnd!)).ToList();
			rottenWindows.ForEach(wnd => focusedWorkspace.Remove(wnd!));
		}
	}

	void ApplyConfigsToWindow(Window wnd)
	{
		wnd.floating = IsWindowInConfigRules(wnd, "floating");
	}

	public void WindowShown(Window wnd)
	{
		if (ShouldWindowBeIgnored(wnd)) return;
		if (suppressEvents) return;
		foreach (var wksp in workspaces)
			if (wksp!.windows.Contains(wnd))
			{
				// This is for cases where an already added window gets focused without direct interaction
				// for eg say you click a link on your terminal and your default browser is open
				// in another workspace. The reason why we are handling it here instead of
				// WindowFocused is because the event emmited is OBJECT_SHOW rather than
				// EVENT_FOREGROUND_CHANGED
				if (wksp != focusedWorkspace) FocusWorkspace(wksp);
				return;
			}

		// Add() and CleanGhostWindows() can cause windows to be re added if they
		// occur while the other hasnt completed, so lock them
		lock (@addLock)
		{
			ApplyConfigsToWindow(wnd);
			wnd.workspace = focusedWorkspaceIndex;
			focusedWorkspace.Add(wnd);
			if (wnd.floating) focusedWorkspace.MakeFloating(wnd);
			focusedWorkspace.Update();
			//Console.WriteLine($"WindowAdded, {wnd.title}, hWnd: {wnd.hWnd}, class: {wnd.className}, floating: {wnd.floating}, exeName: {wnd.exeName}, count: {focusedWorkspace.windows.Count}");
		}

		CleanGhostWindows();
		SaveState($"WindowShown, wnd: {wnd.title}, exe: {wnd.exe}");
	}

	public void WindowHidden(Window wnd)
	{
		// we shouldn'd filter out by ShouldWindowBeIgnored() and in WindowDestroyed
		// here because windows that get hidden or destroyed might meet the 
		// ignorable criteria
		if (suppressEvents) return;
		if ((wnd = GetAlreadyStoredWindow(wnd)) == null) return;

		if (focusedWorkspace.windows.Contains(wnd))
		{
			focusedWorkspace.Remove(wnd);
			focusedWorkspace.Update();
		}

		CleanGhostWindows();
		SaveState("WindowHidden");
	}

	public void WindowDestroyed(Window wnd)
	{
		if (suppressEvents) return;
		if ((wnd = GetAlreadyStoredWindow(wnd)) == null) return;

		//Console.WriteLine($"WindowRemoved, {wnd.title}, hWnd: {wnd.hWnd}, class: {wnd.className}");

		if (focusedWorkspace.windows.Contains(wnd))
		{
			focusedWorkspace.Remove(wnd);
			focusedWorkspace.Update();
		}

		CleanGhostWindows();
		SaveState("WindowRemoved");
	}

	// window handlers must always check window properties of the already stored windows
	public void WindowMoved(Window wnd)
	{
		if (ShouldWindowBeIgnored(wnd)) return;
		if (suppressEvents) return;
		if ((wnd = GetAlreadyStoredWindow(wnd)) == null) return;

		//Console.WriteLine($"WindowMoved, {wnd.title}, hWnd: {wnd.hWnd}, class: {wnd.className}");

		//var _wnd = focusedWorkspace.windows.FirstOrDefault(_wnd => _wnd == wnd);
		//if (_wnd == null) return;
		// wnd -> window being moved
		// cursorPos
		// wndEnclosingCursor -> window enclosing cursor
		if (!wnd.floating && wnd.resizeable)
		{
			User32.GetCursorPos(out var pt);
			var wndUnderCursor = focusedWorkspace.GetWindowFromPoint(pt);
			if (wndUnderCursor == null) return;
			focusedWorkspace.SwapWindows(wnd, wndUnderCursor);
		}

		focusedWorkspace.Update();
		CleanGhostWindows();
		SaveState("WindowMoved");
	}

	public void WindowMaximized(Window wnd)
	{
		if (ShouldWindowBeIgnored(wnd)) return;
		if (suppressEvents) return;
		if ((wnd = GetAlreadyStoredWindow(wnd)) == null) return;

		//Console.WriteLine($"WindowMazimized, {wnd.title}, hWnd: {wnd.hWnd}, class: {wnd.className}");

		focusedWorkspace.Update();
		CleanGhostWindows();
		SaveState("WindowMaximized");
	}

	public void WindowMinimized(Window wnd)
	{
		if (ShouldWindowBeIgnored(wnd)) return;
		if (suppressEvents) return;
		if ((wnd = GetAlreadyStoredWindow(wnd)) == null) return;

		//Console.WriteLine($"WindowMinimized, {wnd.title}, hWnd: {wnd.hWnd}, class: {wnd.className}");
		// render only after state has updated (winevent and GetWindowPlacement() is not synchronous)
		TaskEx.WaitUntil(() => wnd.state == SHOWWINDOW.SW_SHOWMINIMIZED).Wait();

		focusedWorkspace.Update();
		CleanGhostWindows();
		SaveState("WindowMinimized");
	}

	public bool mouseDown { get; set; } = false;
	public void WindowRestored(Window wnd)
	{

		if (ShouldWindowBeIgnored(wnd)) return;
		if (suppressEvents) return;
		if ((wnd = GetAlreadyStoredWindow(wnd)) == null) return;
		if (mouseDown) return;

		//Console.WriteLine($"WindowRestored, {wnd.title}, hWnd: {wnd.hWnd}, class: {wnd.className}");

		focusedWorkspace.Update();
		CleanGhostWindows();
		SaveState($"WindowRestored, wnd: {wnd.title}, hWnd: {wnd.hWnd}");
	}

	Workspace? GetWindowWorkspace(Window wnd)
	{
		return workspaces.FirstOrDefault(wksp => wksp!.windows.Contains(wnd));
	}

	public void WindowFocused(Window wnd)
	{
		if (ShouldWindowBeIgnored(wnd)) return;
		if (suppressEvents) return;
		if ((wnd = GetAlreadyStoredWindow(wnd)!) == null) return;

		//Console.WriteLine($"WindowFocused, {wnd.title}, hWnd: {wnd.hWnd}, class: {wnd.className}");

		focusedWorkspace.Update();
		CleanGhostWindows();
		SaveState($"WindowFocused, {wnd.title}");
	}

	public WindowManagerState GetState()
	{
		WindowManagerState state = new();
		GetAllWindows().ForEach(wnd => state.windows.Add(wnd!));
		state.focusedWorkspaceIndex = focusedWorkspaceIndex;
		state.workspaceCount = workspaces.Count;
		return state;
	}

	int stateCounter = 0;
	readonly Lock @lock = new();
	public void SaveState(string? lastAction = null)
	{
		lock (@lock)
		{
			var state = GetState();
			WINDOW_MANAGER_MESSAGE_SENT(state.ToJson());
			try
			{
				File.WriteAllText(Paths.stateFile, state.ToJson());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine($"{stateCounter++}. lastAction: {lastAction}\n{state.ToJson()}");
		}
	}

	// server event handler
	public delegate void WindowManagerMessageHandler(string message);
	public event WindowManagerMessageHandler WINDOW_MANAGER_MESSAGE_SENT = (message) => { };

	public string RequestReceived(string request)
	{
		var args = request.Split(" ");
		args[args.Length - 1] = args.Last().Replace("\n", "");
		//Console.WriteLine($"arg0: {args.FirstOrDefault()}, arg1: {args.ElementAtOrDefault(1)}");
		var verb = args.FirstOrDefault();
		//Console.WriteLine($"verb: {verb}");
		var response = "";
		switch (verb)
		{
			case null or "":
				break;
			case "get":
				switch (args.ElementAtOrDefault(1))
				{
					case null or "":
						break;
					case "state":
						response = GetState().ToJson();
						break;
				}
				break;
			case "set":
				switch (args.ElementAtOrDefault(1))
				{
					case null or "":
						break;
					case "focusedWorkspaceIndex":
						var index = Convert.ToInt32(args.ElementAtOrDefault(2));
						if (index >= 0 && index <= workspaces.Count - 1) FocusWorkspace(workspaces[index]);
						break;
				}
				break;
			default:
				break;
		}
		return response;
	}

	// animation related 
	int GetX(int start, int end, int frames, int frame)
	{
		var progress = (double)frame / frames;
		progress = EaseOutQuint(progress);
		return start + (int)((end - start) * progress);
	}

	public double EaseOutQuint(double x)
	{
		return 1 - Math.Pow(1 - x, 3);
	}

	public async Task WorkspaceAnimate(Workspace wksp, int startX, int endX, int duration)
	{
		var fps = 60;
		var dt = 1000 / fps; // milliseconds
		var frames = (int)(((float)duration / 1000) * fps);

		Stopwatch sw = new();
		sw.Start();
		for (var i = 0; i < frames; i++)
		{
			wksp.Move(GetX(startX, endX, frames, i), null, redraw: false); // not drawn, so must be manually redrawn once finished
			var wait = (int)(i * dt - sw.ElapsedMilliseconds);
			wait = wait < 0 ? 0 : wait;
			await Task.Delay(wait);
		}
		sw.Stop();
	}
}
