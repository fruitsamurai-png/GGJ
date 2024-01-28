using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilityPassiveManager : MonoBehaviour
{
	public static int AILevel = 1;
	public static int AIexp = 0;
	static int AImaxExp = 100;

	public static void AddExp(int exp)
	{
		AIexp += exp;
		for (int i = 0; i < 10; i++)
		{
			if (AIexp > AImaxExp)
			{
				AIexp -= 100;
				AILevel++;
			}
		}
	}

	public static float GetExpPercentage()
	{
		return ((float)AIexp) / ((float)AImaxExp);
	}

	public static void Reset()
	{
		AILevel = 1;
		AIexp = 0;
	}
}
