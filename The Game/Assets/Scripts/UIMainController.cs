using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainController : MonoBehaviour
{
	public UIStealArtwork stealArtworkUI;

	private void Awake()
	{
		UIStealArtwork.instance = stealArtworkUI;
	}

}
