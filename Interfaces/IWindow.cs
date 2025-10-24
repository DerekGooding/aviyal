/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
using aviyal.Classes.Structs;
using aviyal.Classes.Win32;

namespace aviyal.Interfaces;
public interface IWindow
{
	public nint hWnd { get; }
	public string title { get; }
	public string className { get; }
	public string? exe { get; }
	public RECT rect { get; }
	public SHOWWINDOW state { get; }
	public bool resizeable { get; }
	public bool floating { get; set; }
	public WINDOWSTYLE styles { get; }
	public WINDOWSTYLEEX exStyles { get; }

	public void Hide();
	public void Show();
	public void Focus();
	public void Move(RECT pos, bool redraw);
	public void Move(int? x, int? y, bool redraw);
	public void Close();
	public void Redraw();
}

