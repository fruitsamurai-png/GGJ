using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingInventory
{
	public static List<StolenArt> inventory = new();
	public static int money = 0;

	public static int moneyPerArtLevel = 10000;
	public static int expPerArtLevel = 10;

	public static void Reset()
	{
		inventory = new();
		money = 0;
	}
	public static void AddArt(int artwork, int level)
	{
		inventory.Add(new(artwork, level));
	}
	public static void RemoveArt(StolenArt art)
	{
		inventory.Remove(art);
	}
	public static void SellArt(StolenArt art)
	{
		money += moneyPerArtLevel * art.level;
		RemoveArt(art);
	}
	public static void TrainArt(StolenArt art)
	{
		AbilityPassiveManager.AddExp(expPerArtLevel * art.level);
		RemoveArt(art);
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
