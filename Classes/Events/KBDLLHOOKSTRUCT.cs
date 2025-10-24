/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

namespace aviyal.Classes.Events;

public struct KBDLLHOOKSTRUCT
{
	public uint vkCode;
	public uint scanCode;
	public uint flags;
	public uint time;
	public nint dwExtraInfo;
}
