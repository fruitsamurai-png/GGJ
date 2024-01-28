using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingInventory
{
	public static List<StolenArt> inventory = new();

	public static void Reset()
	{
		inventory = new();
	}
	public static void AddArt(int artwork, int level)
	{
		inventory.Add(new(artwork, level));
	}
	public static void RemoveArt(StolenArt art)
	{
		inventory.Remove(art);
	}
}

public struct StolenArt
{
	public int artwork;
	public int level;

	public StolenArt(int artwork, int level)
	{
		this.artwork = artwork;
		this.level = level;
	}
}
