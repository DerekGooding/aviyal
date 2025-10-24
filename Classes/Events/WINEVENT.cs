/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

namespace aviyal.Classes.Events;

// https://learn.microsoft.com/en-us/windows/win32/winauto/event-constants
enum WINEVENT : uint
{
	OBJECT_CREATE = 0x8000,
	OBJECT_DESTROY = 0x8001,
	OBJECT_SHOW = 0x8002,
	OBJECT_HIDE = 0x8003,
	EVENT_SYSTEM_MOVESIZEEND = 0x000B,
	EVENT_SYSTEM_MINIMIZESTART = 0x0016,
	EVENT_SYSTEM_MINIMIZEEND = 0x0017,
	// because windows doesnt have a maximize winevent
	EVENT_OBJECT_LOCATIONCHANGE = 0x800B,
	EVENT_SYSTEM_FOREGROUND = 0x0003,
	EVENT_OBJECT_UNCLOAKED = 0x8018,
}
