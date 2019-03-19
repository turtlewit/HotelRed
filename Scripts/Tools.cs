using Godot;
using System;

public static class Tools// : Node
{
	private static Random rand = new Random();

	public static T Choose<T>(T[] items)
	{
		return items[Tools.rand.Next(items.Length)];
	}
}
