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
		if (AlertManager.alert)
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
			noticingSFX.SetActive(AlertManager.noticing);
		}
	}
}