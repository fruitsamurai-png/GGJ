using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStolenArtworkInventory : MonoBehaviour
{
	public GameObject artworkDisplayPrefab;

	List<UIArtworkDisplay> displays = new();
	void Update()
	{
		UpdateInventory();
	}

	void UpdateInventory()
	{
		if (StealingInventory.inventory.Count > displays.Count)
		{
			UIArtworkDisplay newDisplay = Instantiate(artworkDisplayPrefab, transform).GetComponent<UIArtworkDisplay>();
			newDisplay.transform.localScale = Vector3.one * 0.3f;
			displays.Add(newDisplay);
		}

		for (int i = 0; i < displays.Count; i++)
		{
			if (i >= StealingInventory.inventory.Count)
			{
				Destroy(displays[i].gameObject);
				displays.RemoveAt(i);
				i--;
				continue;
			}
			displays[i].artwork = StealingInventory.inventory[i].artwork;
		}
	}
}
