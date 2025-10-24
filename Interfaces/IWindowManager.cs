/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
using aviyal.Classes;

namespace aviyal.Interfaces;

public interface IWindowManager
{
	public List<Workspace?> workspaces { get; }
	public Workspace focusedWorkspace { get; }
	public int focusedWorkspaceIndex { get; }

	public void FocusWorkspace(Workspace wksp);
	public void FocusNextWorkspace() { }
	public void FocusPreviousWorkspace() { }

	public void WindowShown(Window wnd);
	public void WindowHidden(Window wnd);
	public void WindowDestroyed(Window wnd);
	public void WindowMoved(Window wnd);
	public void WindowMaximized(Window wnd);
	public void WindowMinimized(Window wnd);
	public void WindowRestored(Window wnd);
}

