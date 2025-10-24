/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
using aviyal.Interfaces;
using System.Text.Json.Nodes;

namespace aviyal.Classes.Utilities;

public class WindowManagerState : IJson<WindowManagerState>
{
	public List<Window> windows = new();
	public int focusedWorkspaceIndex;
	public int workspaceCount;

	public string ToJson()
	{
		JsonObject j = new()
		{
			["windows"] = new JsonArray(
				windows.Select(wnd =>
				{
					return new JsonObject()
					{
						["hWnd"] = wnd.hWnd.ToString(),
						["title"] = wnd.title,
						["exe"] = wnd.exe,
						["state"] = wnd.state.ToString(),
						["className"] = wnd.className.ToString(),
						["borderThickness"] = wnd.borderThickness.ToString(),
						["elevated"] = wnd.elevated.ToString(),
						["floating"] = wnd.floating.ToString(),
						["resizeable"] = wnd.resizeable.ToString(),
						["workspace"] = wnd.workspace.ToString(),
					};
				}).ToArray()
			),
			["focusedWorkspaceIndex"] = focusedWorkspaceIndex.ToString(),
			["workspaceCount"] = workspaceCount.ToString(),
		};
		return j.ToString();
	}

	public static WindowManagerState FromJson(string json)
	{
		WindowManagerState state = new();
		var node = JsonNode.Parse(json);
		var _arr = node?["windows"]?.AsArray();
		_arr?.ToList().ForEach(
			_wnd =>
			{
				var hWnd = (nint)Convert.ToInt32(_wnd?["hWnd"]?.ToString());
				Window wnd = new(hWnd);
				state.windows.Add(wnd);
			}
		);
		state.focusedWorkspaceIndex = Convert.ToInt32(node?["focusedWorkspaceIndex"].ToString());
		state.workspaceCount = Convert.ToInt32(node?["workspaceCount"].ToString());
		return state;
	}
}
