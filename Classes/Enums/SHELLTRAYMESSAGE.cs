/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

namespace aviyal.Classes.Enums;

/// <summary>
/// WM_COPYDATA message sent to Taskbar [Shell_TrayWnd] carries data
/// in its lpData field which is one of the three 
/// different types identified by its dwData field
/// </summary>
public enum SHELLTRAYMESSAGE : int
{
	// Resolve to SHELLTRAYDATA struct to use data for ICONUPDATE
	ICONUPDATE = 1,
	APPBAR = 2,
	TRAYICONPOSITION = 3
}
