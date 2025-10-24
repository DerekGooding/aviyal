/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using Aviyal;

namespace aviyal.Classes.Events;

public class Keymap
{
	public Guid id { get; } = Guid.NewGuid();
	public List<VK> keys = [];
	public COMMAND command;
	public List<string> arguments = [];
    public override bool Equals(object? obj) => ((Keymap)obj).id == id;
    public static bool operator ==(Keymap left, Keymap right) => left.Equals(right);
    public static bool operator !=(Keymap left, Keymap right) => !left.Equals(right);
}
