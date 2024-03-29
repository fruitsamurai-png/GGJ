using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
	public static bool alert = false;
	public static bool noticing = false;

	// Update is called once per frame
	void Update()
	{
		alert = false;
		noticing = false;
		//Check every single guard to see if they're alerted or noticing player
		foreach (GuardEnemyBehavior e in FindObjectsOfType<GuardEnemyBehavior>())
		{
			if (e.m_Enemy.m_IsAlerted)
			{
				alert = true;
				break;
			}
			if (e.m_Enemy.m_IsPlayerInFOV || e.m_Enemy.IsNoticingStolenPainting)
			{
				noticing = true;
				break;
			}
		}
		//Check every single robot guard to see if they're alerted or noticing player
		foreach (SecurityBotEnemyBehavior e in FindObjectsOfType<SecurityBotEnemyBehavior>())
		{
			if (e.m_Enemy.m_IsAlerted)
			{
				alert = true;
				break;
			}
			if (e.m_Enemy.m_IsPlayerInFOV || e.m_Enemy.IsNoticingStolenPainting)
			{
				noticing = true;
				break;
			}
		}
		//Check every single camera to see if they're alerted or noticing player
		foreach (CameraEnemyBehavior e in FindObjectsOfType<CameraEnemyBehavior>())
		{
			if (e.m_Enemy.m_IsAlerted)
			{
				alert = true;
				break;
			}
			if (e.m_Enemy.m_IsPlayerInFOV || e.m_Enemy.IsNoticingStolenPainting)
			{
				noticing = true;
				break;
			}
		}
	}
}
