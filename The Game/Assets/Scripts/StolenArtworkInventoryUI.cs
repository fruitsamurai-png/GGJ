using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StolenArtworkInventoryUI : MonoBehaviour
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
				displays.RemoveAt(i);
				Destroy(displays[i].gameObject);
				i--;
				continue;
			}
			displays[i].artwork = StealingInventory.inventory[i].artwork;
		}
	}
}
