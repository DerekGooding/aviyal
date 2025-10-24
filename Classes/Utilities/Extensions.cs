/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
namespace aviyal.Classes.Utilities;

public static class Extensions
{
    public static bool ContainsFlag(this uint flag, uint flagToCheck) => (flag & flagToCheck) != 0;
}
