/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using aviyal.Classes.Enums;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace aviyal.Classes.Events;
public partial class MouseEventsListener : IDisposable
{
	delegate int MouseProc(int code, nint wparam, nint lparam);

	[LibraryImport("user32.dll", SetLastError = true)]
	private static partial nint SetWindowsHookExA(int idHook, MouseProc lpfn, nint hmod, uint dwThreadId);

	[LibraryImport("user32.dll", SetLastError = true)]
    private static partial int UnhookWindowsHookEx(nint hhook);

	[LibraryImport("user32.dll", SetLastError = true)]
	private static partial int CallNextHookEx(nint hhk, int nCode, nint wparam, nint lparam);

	[LibraryImport("user32.dll", SetLastError = true)]
	public static partial int GetMessage(out uint msg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

	[LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TranslateMessage(ref uint msg);

	[LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DispatchMessage(ref uint msg);

	readonly Lock @eventLock = new();
	int MouseCallback(int code, nint wparam, nint lparam)
	{
		lock (@eventLock)
		{
			var mouseStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lparam);
			switch ((WINDOWMESSAGE)wparam)
			{
				case WINDOWMESSAGE.WM_LBUTTONDOWN:
					MOUSE_DOWN();
					break;
				case WINDOWMESSAGE.WM_LBUTTONUP:
					MOUSE_UP();
					break;
			}
			////Console.WriteLine($"mouseEvent: {(WINDOWMESSAGE)wparam}");
			return CallNextHookEx(0, code, wparam, lparam);
		}
	}

	nint hhook;
	bool running = true;
	void Loop()
	{
		const int WH_MOUSE_LL = 14;
		// hmod = 0, hook function is in code
		// dwThreadId = 0, hook all threads
		hhook = SetWindowsHookExA(WH_MOUSE_LL, MouseCallback, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
		// always use a message pump, instead of: while(Console.ReadLine() != ":q") { }
		while (running)
		{
			var _ = GetMessage(out var msg, 0, 0, 0);
			TranslateMessage(ref msg);
			DispatchMessage(ref msg);
		}
	}

	public delegate void MouseEventHandler();
	public event MouseEventHandler MOUSE_DOWN = () => { };
	public event MouseEventHandler MOUSE_UP = () => { };

	Thread thread;
	public MouseEventsListener()
	{
		thread = new(Loop);
		thread.Start();
	}

	public void Dispose()
	{
		UnhookWindowsHookEx(hhook);
		running = false;
	}
}



