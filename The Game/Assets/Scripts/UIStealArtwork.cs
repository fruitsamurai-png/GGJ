using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIStealArtwork : MonoBehaviour
{
	public static UIStealArtwork instance;
	public UIArtworkDisplay artDisplayer;

	public TextMeshProUGUI artLevelText;

	public GameObject stealButton;
	public GameObject replaceButton;

	Artwork targetedArtwork;

	GameObject player;

	void Awake()
	{
		player = GameObject.FindWithTag("Player");
		instance = this;
	}

	private void OnEnable()
	{
		Time.timeScale = 0f;
		UpdateDisplay();
	}
	private void OnDisable()
	{
		Time.timeScale = 1f;
	}
	void UpdateDisplay()
	{
		artDisplayer.artwork = targetedArtwork.artwork;
		artDisplayer.fake = targetedArtwork.artState == Artwork.ArtState.FAKE;

		artLevelText.text = (targetedArtwork.artState == Artwork.ArtState.FAKE ? "A.I Art Level: " + targetedArtwork.replicaLevel : "Artwork Level: " + targetedArtwork.artLevel);
		artDisplayer.gameObject.SetActive(targetedArtwork.artState != Artwork.ArtState.STOLEN);
		artLevelText.gameObject.SetActive(targetedArtwork.artState != Artwork.ArtState.STOLEN);

		replaceButton.SetActive(targetedArtwork.artState == Artwork.ArtState.STOLEN);
		stealButton.SetActive(targetedArtwork.artState == Artwork.ArtState.REAL);
	}
	public void OpenArtwork(Artwork targetArt)
	{
		targetedArtwork = targetArt;
		gameObject.SetActive(true);
	}

	public void StealButton()
	{
		targetedArtwork.Steal(player);
		UpdateDisplay();
	}
	public void ReplaceButton()
	{
		targetedArtwork.Replace(AbilityPassiveManager.AILevel);
		UpdateDisplay();
	}
	public void CloseButton()
	{
		gameObject.SetActive(false);
	}
}
