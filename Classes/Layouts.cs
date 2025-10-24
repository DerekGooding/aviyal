/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
using aviyal.Classes.Structs;
using aviyal.Interfaces;

namespace aviyal.Classes;

public class Dwindle(Config config) : ILayout
{
	// rects with margin
	RECT[]? rects;
	// rects with no margins
	RECT[]? fillRects;

	public RECT[] GetRects(int count /* no of windows */)
	{
		rects = new RECT[count];
		fillRects = new RECT[count];

		(int width, int height) = Utils.GetScreenSize();
		FillDirection fillDirection = FillDirection.HORIZONTAL;
		// where the nth window will go
		RECT fillRect = new() { Left = 0, Top = 0, Right = width, Bottom = height };
		for (int i = 0; i < count; i++)
		{
			fillRects[i] = fillRect;

			// modify the fillRect
			switch (fillDirection)
			{
				case FillDirection.HORIZONTAL:
					if (i - 1 >= 0)
					{
						fillRects[i - 1] = TopHalf(fillRects[i - 1]);
					}
					fillRect.Left += (fillRect.Right - fillRect.Left) / 2;
					break;
				case FillDirection.VERTICAL:
					if (i - 1 >= 0)
					{
						fillRects[i - 1] = LeftHalf(fillRects[i - 1]);
					}
					fillRect.Top += (fillRect.Bottom - fillRect.Top) / 2;
					break;
			}
			fillDirection = fillDirection == FillDirection.HORIZONTAL ? FillDirection.VERTICAL : FillDirection.HORIZONTAL;

		}
		//fillRects.Index().ToList().ForEach(irect => Console.WriteLine($"{irect.Item1}. L:{irect.Item2.Left} R:{irect.Item2.Right} T:{irect.Item2.Top} B:{irect.Item2.Bottom}"));
		//return ApplyInner(ApplyOuter(fillRects.ToArray()));
		return fillRects;
	}

    public int left { get; set; } = config.left;
    public int top { get; set; } = config.top;
    public int right { get; set; } = config.right;
    public int bottom { get; set; } = config.bottom;
    public int inner { get; set; } = config.inner;

    RECT LeftHalf(RECT rect)
	{
		rect.Right -= (rect.Right - rect.Left) / 2;
		return rect;
	}
	RECT TopHalf(RECT rect)
	{
		rect.Bottom -= (rect.Bottom - rect.Top) / 2;
		return rect;
	}

	// applies outer margins
	public RECT[] ApplyOuter(RECT[] fillRects)
	{
		(int width, int height) = Utils.GetScreenSize();
		for (int i = 0; i < fillRects.Length; i++)
		{
			if (fillRects[i].Left == 0) fillRects[i].Left += left;
			if (fillRects[i].Top == 0) fillRects[i].Top += top;
			if (fillRects[i].Right == width) fillRects[i].Right -= right;
			if (fillRects[i].Bottom == height) fillRects[i].Bottom -= bottom;
		}
		return fillRects;
	}

	// applies inner margins (apply only after outer)
	public RECT[] ApplyInner(RECT[] fillRects)
	{
		(int width, int height) = Utils.GetScreenSize();
		for (int i = 0; i < fillRects.Length; i++)
		{
			if (fillRects[i].Left != left) fillRects[i].Left += inner / 2;
			if (fillRects[i].Top != top) fillRects[i].Top += inner / 2;
			if (fillRects[i].Right != width - right) fillRects[i].Right -= inner / 2;
			if (fillRects[i].Bottom != height - bottom) fillRects[i].Bottom -= inner / 2;
		}
		return fillRects;
	}

	EDGE[] GetEdges(RECT rect, int screenWidth, int screenHeight)
	{
		List<EDGE> edges = [];
		//Console.WriteLine($"GetEdges: {rect.Left}");
		if (rect.Left == 0) edges.Add(EDGE.LEFT);
		if (rect.Top == 0) edges.Add(EDGE.TOP);
		if (rect.Right == screenWidth) edges.Add(EDGE.RIGHT);
		if (rect.Bottom == screenHeight) edges.Add(EDGE.BOTTOM);
		return edges.ToArray();
	}

	public int? GetAdjacent(int index, EDGE direction)
	{
		// 1. figure out if the window is on an edge
		// 2. if not just add +1 to index if direction is RIGHT, -1 if direction is LEFT
		// 3. if at edge return index
		if (index > fillRects.Length - 1) return null;
		(int width, int height) = Utils.GetScreenSize();
		EDGE[] edges = GetEdges(fillRects[index], width, height);
		//Console.WriteLine("edgesCount: " + edges.Length);
		edges.ToList().ForEach(edge => Console.Write($"{edge}, "));

        return edges.Contains(direction) ? index : direction is EDGE.LEFT or EDGE.TOP ? index - 1 : index + 1;
    }
}
