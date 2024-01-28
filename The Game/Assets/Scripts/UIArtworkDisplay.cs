using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArtworkDisplay : MonoBehaviour
{
	public int artwork = 0;
	public bool fake = false;
	public GameObject[] realArtObjects;
	public GameObject[] fakeArtObjects;
	public GameObject realArtSeal;
	public GameObject fakeArtSeal;

	void Update()
	{
		UpdateArtwork();
	}
	private void OnValidate()
	{
		UpdateArtwork();
	}

	public void UpdateArtwork()
	{
		for (int i = 0; i < realArtObjects.Length; i++)
		{
			realArtObjects[i].SetActive(!fake && i == artwork);
		}

		for (int i = 0; i < fakeArtObjects.Length; i++)
		{
			fakeArtObjects[i].SetActive(fake && i == artwork);
		}

		realArtSeal.SetActive(!fake);
		fakeArtSeal.SetActive(fake);
	}

}
