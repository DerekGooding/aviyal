/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
using aviyal.Classes;
using aviyal.Classes.Structs;

namespace aviyal.Interfaces;

public interface IWorkspace
{
	public List<Window> windows { get; }
	public Window? focusedWindow { get; }
	public int? focusedWindowIndex { get; }
	public ILayout layout { get; set; }

	public void Add(Window wnd);
	public void Remove(Window wnd);

	public void Show();
	public void Hide();
	public void Focus();
	public void Redraw();
	public void SetFocusedWindow();
	public void CloseFocusedWindow();
	public void FocusAdjacentWindow(EDGE direction);
	public void Move(int? x, int? y, bool redraw);
	public void SwapWindows(Window wnd1, Window wnd2);
	public Window? GetWindowFromPoint(POINT pt);
}

