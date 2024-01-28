using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPostHeistMegaController : MonoBehaviour
{
	public GameObject stolenArtworkEntryPrefab;
	public Transform stolenArtworkPanel;

	public TextMeshProUGUI moneyText;
	public TextMeshProUGUI aiLevelText;

	public GameObject expBar;

	List<UIPostHeistStolenPaintingEntry> stolenArtworkEntries = new();

	void Start()
	{

	}

	void Update()
	{
		UpdateList();

		moneyText.text = $"${(StealingInventory.money):n0}";
		aiLevelText.text = AbilityPassiveManager.AILevel.ToString();
		expBar.transform.localScale = new Vector3(AbilityPassiveManager.GetExpPercentage(), 1f);
	}

	public void NextHeist()
	{
		SceneManager.LoadScene("MainLevel"); 
	}

	void UpdateList()
	{
		if (StealingInventory.inventory.Count > stolenArtworkEntries.Count)
		{
			UIPostHeistStolenPaintingEntry newDisplay = Instantiate(stolenArtworkEntryPrefab, stolenArtworkPanel).GetComponent<UIPostHeistStolenPaintingEntry>();
			stolenArtworkEntries.Add(newDisplay);
		}

		for (int i = 0; i < stolenArtworkEntries.Count; i++)
		{
			if (i >= StealingInventory.inventory.Count)
			{
				Destroy(stolenArtworkEntries[i].gameObject);
				stolenArtworkEntries.RemoveAt(i);
				i--;
				continue;
			}
			stolenArtworkEntries[i].art = StealingInventory.inventory[i];
		}
	}
}

