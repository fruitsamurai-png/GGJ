using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public GameObject sneakyMusic;
	public GameObject chaseMusic;
	public GameObject alertSFX;
	public GameObject noticingSFX;

	void Update()
	{
		bool chase = false;
		bool noticing = false;

		//Check every single guard to see if they're alerted or noticing player
		foreach (GuardEnemyBehavior e in FindObjectsOfType<GuardEnemyBehavior>())
		{
			if (e.m_Enemy.isAltered)
			{
				chase = true;
				break;
			}
			if (e.m_Enemy.m_IsPlayerInFOV)
			{
				noticing = true;
				break;
			}
		}

		if (chase)
		{
			sneakyMusic.SetActive(false);
			chaseMusic.SetActive(true);
			alertSFX.SetActive(true);
			noticingSFX.SetActive(false);
		}
		else
		{
			sneakyMusic.SetActive(true);
			chaseMusic.SetActive(false);
			alertSFX.SetActive(false);
			noticingSFX.SetActive(noticing);
		}
	}
}