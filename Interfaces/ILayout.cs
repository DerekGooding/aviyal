/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
using aviyal.Classes;
using aviyal.Classes.Structs;

namespace aviyal.Interfaces;

public interface ILayout
{
	public int inner { get; set; }
	public int left { get; set; }
	public int top { get; set; }
	public int right { get; set; }
	public int bottom { get; set; }

	public RECT[] GetRects(int index);
	public RECT[] ApplyInner(RECT[] rects);
	public RECT[] ApplyOuter(RECT[] rects);
	public int? GetAdjacent(int index, EDGE direction);
}

