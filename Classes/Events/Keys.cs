/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace aviyal.Classes.Events;
public class KeyEventsListener : IDisposable
{
	delegate int KeyboardProc(int code, nint wparam, nint lparam);

	[DllImport("user32.dll", SetLastError = true)]
	static extern nint SetWindowsHookExA(int idHook, KeyboardProc lpfn, nint hmod, uint dwThreadId);

	[DllImport("user32.dll", SetLastError = true)]
	static extern int UnhookWindowsHookEx(nint hhook);

	[DllImport("user32.dll", SetLastError = true)]
	static extern int CallNextHookEx(nint hhk, int nCode, nint wparam, nint lparam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int GetMessage(out uint msg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool TranslateMessage(ref uint msg);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool DispatchMessage(ref uint msg);

	List<VK> captured = [];
	List<Keymap> keymaps = [];

	void Log(List<VK> keys, uint dt = 0, VK? key = null, string prefix = "")
	{
		Console.Write($"{prefix}[");
		keys.ForEach(key => Console.Write($"{key}, "));
		Console.Write($"], {dt}ms, lasKey: {lastKey}, letKeyPass: {letKeyPass}, key: {key}\n");
	}

	// we use locking because future keys if allowed to go through the callback proc
	// as it comes before the current key is fully processed will cause unintended
	// consequences such as clearing the captured collection. However this is not
	// required for the window events as no such lists exists for that and we can
	// adopt a fire and forget mechanism for them
	private readonly Lock @eventLock = new();
	uint lastKeyTime = 0;
	VK? lastKey; // the trailing key of a hotkey action -> H in Ctrl+Shift+H
	bool letKeyPass = true;
	int KeyboardCallback(int code, nint wparam, nint lparam)
	{
		lock (@eventLock)
		{
			var kbdStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lparam);
			if (kbdStruct.dwExtraInfo == Globals.FOREGROUND_FAKE_KEY)
			{
				//Console.WriteLine("FOREGROUND_FAKE_KEY");
				return 1;
			}
			var key = (VK)kbdStruct.vkCode;
			//Console.WriteLine($"START, Key: {key}, {(WINDOWMESSAGE)wparam}");
			var dt = kbdStruct.time - lastKeyTime;
			if (dt > 500) captured.Clear();
			letKeyPass = true;
			switch ((WINDOWMESSAGE)wparam)
			{
				case WINDOWMESSAGE.WM_KEYDOWN or WINDOWMESSAGE.WM_SYSKEYDOWN /* ALT */:
					if (!captured.Contains(key)) captured.Add(key);
					//Log(captured, dt);
					foreach (var keymap in keymaps)
					{
						if (Utils.ListContentEqual<VK>(captured, keymap.keys))
						{
							//Log(captured, dt, "HOTKEY_PRESSED");
							//Log(keymap.keys, dt, "HOTKEY_PRESSED");
							lastKey = key;
							letKeyPass = false;
							captured.Clear();

							Task.Run(() => HOTKEY_PRESSED(keymap));

							break;
						}
					}
					break;
				case WINDOWMESSAGE.WM_KEYUP:
					if (key == lastKey)
					{
						letKeyPass = false;
						lastKey = null;
					}
					captured.Remove(key);
					break;
			}
			//Console.WriteLine($"KEY: {key}, MSG: {(WINDOWMESSAGE)wparam}");
			lastKeyTime = kbdStruct.time;
			//Console.WriteLine($"key: {key}, {(WINDOWMESSAGE)wparam}, pass: {letKeyPass}");
			//Console.WriteLine($"END, Key: {key}, {(WINDOWMESSAGE)wparam}");

			if (letKeyPass) return CallNextHookEx(0, code, wparam, lparam);
			return 1; // handled
		}
	}

	nint hhook;
	bool running = true;
	void Loop()
	{
		const int WH_KEYBOARD_LL = 13;
		// hmod = 0, hook function is in code
		// dwThreadId = 0, hook all threads
		hhook = SetWindowsHookExA(WH_KEYBOARD_LL, KeyboardCallback, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
		// always use a message pump, instead of: while(Console.ReadLine() != ":q") { }
		while (running)
		{
			var _ = GetMessage(out var msg, 0, 0, 0);
			TranslateMessage(ref msg);
			DispatchMessage(ref msg);
		}
	}

	public delegate void HotkeyPressedEventHandler(Keymap keymap);
	public event HotkeyPressedEventHandler HOTKEY_PRESSED = (keymap) => { };

	Thread thread;
	public KeyEventsListener(Config config)
	{
		keymaps = config.keymaps;

		thread = new(Loop);
		thread.Start();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
        UnhookWindowsHookEx(hhook);
		running = false;
	}
}
