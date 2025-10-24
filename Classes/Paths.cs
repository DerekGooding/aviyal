/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
namespace aviyal.Classes;
public static class Paths
{
    public static string rootDir = Path.GetDirectoryName(Environment.ProcessPath)!;
	public static string configFile = Path.Join(rootDir, "aviyal.json");
	public static string stateFile = Path.Join(rootDir, "state.json");
	public static string errorFile = Path.Join(rootDir, "error.log");
}
