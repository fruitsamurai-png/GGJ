using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artwork : MonoBehaviour
{
	public bool randomise = false;

	public int artwork = 0;
	public enum ArtState { REAL, STOLEN, FAKE };

	public ArtState artState = ArtState.REAL;
	public int artLevel = 1;
	public int replicaLevel = 1;

	public GameObject[] realArtObjects;
	public GameObject[] fakeArtObjects;
	public GameObject blankSpaceObject;

	private void Start()
	{
		Setup();
		UpdateArtwork();
	}

	void Setup()
	{
		if (randomise || artwork == -1)
		{
			artwork = Random.Range(0, realArtObjects.Length);
		}
		if (randomise || artLevel == -1)
		{
			artLevel = Random.Range(1, 6);
		}
	}

	private void Update()
	{

	}
	public void UpdateArtwork()
	{
		for (int i = 0; i < realArtObjects.Length; i++)
		{
			realArtObjects[i].SetActive(artState == ArtState.REAL && i == artwork);
		}

		for (int i = 0; i < fakeArtObjects.Length; i++)
		{
			fakeArtObjects[i].SetActive(artState == ArtState.FAKE && i == artwork);
		}

		blankSpaceObject.SetActive(artState == ArtState.STOLEN);
	}

	public bool IsStolen
	{
		get { return artState != ArtState.REAL; }
	}
	public int ReplicaLevel
	{
		get { return replicaLevel; }
		set { replicaLevel = value; }
	}

	public void Interact(GameObject interactor)
	{
		UIStealArtwork.instance.OpenArtwork(this);
	}
	public void Steal(GameObject interactor)
	{
		artState = ArtState.STOLEN;
		StealingInventory.AddArt(artwork, artLevel);
		UpdateArtwork();
	}
	public void Replace(int level)
	{
		artState = ArtState.FAKE;
		replicaLevel = level;
		UpdateArtwork();
	}

	private void OnValidate()
	{
		UpdateArtwork();
	}

	private void OnDrawGizmos()
	{
		if (randomise)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, 1f);
		}
		if (artLevel == -1)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, 1f);
		}
	}
}
