/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/
namespace aviyal.Interfaces;
public interface IJson<T>
{
	public string ToJson();
	public static T? FromJson(string json) => throw new NotImplementedException();
}
