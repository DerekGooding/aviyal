/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Runtime.InteropServices;

namespace aviyal.Classes.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct _NOTIFYICONIDENTIFIERINTERNAL
{
	//--------------------
	public int magicNumber;
	public int msg;
	//---------------------
	public int callbackSize;
	//---------------------
	public int padding;
	//---------------------
	public nint hWnd;
	public uint UID;
	public Guid guidItem;
}

