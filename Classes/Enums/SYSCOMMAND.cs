/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

namespace aviyal.Classes.Enums;

public enum SYSCOMMAND : uint
{
	SC_SIZE = 0xF000,           // Sizes the window
	SC_MOVE = 0xF010,           // Moves the window
	SC_MINIMIZE = 0xF020,       // Minimizes the window
	SC_MAXIMIZE = 0xF030,       // Maximizes the window
	SC_NEXTWINDOW = 0xF040,     // Moves to the next window
	SC_PREVWINDOW = 0xF050,     // Moves to the previous window
	SC_CLOSE = 0xF060,          // Closes the window
	SC_VSCROLL = 0xF070,        // Scrolls vertically
	SC_HSCROLL = 0xF080,        // Scrolls horizontally
	SC_MOUSEMENU = 0xF090,      // Retrieves the window menu as a result of a mouse click
	SC_KEYMENU = 0xF100,        // Retrieves the window menu as a result of a keystroke
	SC_RESTORE = 0xF120,        // Restores the window to its normal position and size
	SC_TASKLIST = 0xF130,       // Activates the Start menu
	SC_SCREENSAVE = 0xF140,     // Executes the screen saver application specified in the [boot] section of the System.ini file
	SC_HOTKEY = 0xF150,         // Activates the window associated with the application-specified hot key
	SC_DEFAULT = 0xF160,        // Selects the default item; the user double-clicked the window menu
	SC_MONITORPOWER = 0xF170,   // Sets the state of the display (supports power-saving features)
	SC_CONTEXTHELP = 0xF180     // Changes the cursor to a question mark with a pointer
}
