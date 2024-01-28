using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPostHeistMegaController : MonoBehaviour
{
	public static bool playerWasCaught = false;

	public GameObject stolenArtworkEntryPrefab;
	public Transform stolenArtworkPanel;

	public GameObject heistCompleteText;
	public GameObject caughtText;

	public TextMeshProUGUI moneyText;
	public TextMeshProUGUI aiLevelText;
	public TextMeshProUGUI livesText;

	public GameObject expBar;

	List<UIPostHeistStolenPaintingEntry> stolenArtworkEntries = new();

	public AudioClip sellSound;
	public AudioClip trainSound;
	public AudioClip levelUpSound;

	public AudioClip caughtSound;
	public AudioClip heistCompleteSound;

	AudioSource audioPlayer;

	int lastLevel = 0;
	int lastExp = 0;
	int lastMoeny = 0;
	void Awake()
	{
		audioPlayer = GetComponent<AudioSource>();

		heistCompleteText.SetActive(!playerWasCaught);
		caughtText.SetActive(playerWasCaught);

		if (playerWasCaught)
		{
			AbilityPassiveManager.lives--;
			if (AbilityPassiveManager.lives <= 0)
			{
				SceneManager.LoadScene("GameOver");
			}

			playerWasCaught = false;

			audioPlayer.PlayOneShot(caughtSound, 1f);
		}
		else
		{
			audioPlayer.PlayOneShot(heistCompleteSound, 1f);
		}

		lastLevel = AbilityPassiveManager.AILevel;
		lastExp = AbilityPassiveManager.AIexp;
		lastMoeny = StealingInventory.money;
	}

	void Update()
	{
		UpdateList();

		moneyText.text = $"${(StealingInventory.money):n0}";
		aiLevelText.text = AbilityPassiveManager.AILevel.ToString();
		livesText.text = AbilityPassiveManager.lives.ToString();

		expBar.transform.localScale = new Vector3(AbilityPassiveManager.GetExpPercentage(), 1f);

		if(StealingInventory.money > lastMoeny)
		{
			lastMoeny = StealingInventory.money;
			audioPlayer.PlayOneShot(sellSound, 1f);
		}
		if (AbilityPassiveManager.AIexp != lastExp)
		{
			lastExp = AbilityPassiveManager.AIexp;
			audioPlayer.PlayOneShot(trainSound, 1f);
		}
		if (AbilityPassiveManager.AILevel > lastLevel)
		{
			lastLevel = AbilityPassiveManager.AILevel;
			audioPlayer.PlayOneShot(levelUpSound, 1f);
		}

		if(AbilityPassiveManager.AILevel >= 5 || StealingInventory.money >= 1000000)
		{
			SceneManager.LoadScene("WinScreen");
		}

		if (Input.GetKeyDown(KeyCode.F10))
		{
			StealingInventory.money = 999999;
		}
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
			stolenArtworkEntries[i].UpdateArt();
		}
		for (int i = 0; i < stolenArtworkEntries.Count; i++)
		{
			stolenArtworkEntries[i].art = StealingInventory.inventory[i];
			stolenArtworkEntries[i].UpdateArt();
		}
	}
}

