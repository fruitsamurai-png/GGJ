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

	public GameObject levelWarning;

	Artwork targetedArtwork;

	GameObject player;

	AudioSource audioPlayer;
	public AudioClip openSfx;
	public AudioClip stealSfx;
	public AudioClip replaceSfx;

	void Awake()
	{
		audioPlayer = GetComponent<AudioSource>();
		player = GameObject.FindWithTag("Player");
		instance = this;
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			CloseButton();
		}
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

		levelWarning.SetActive(AbilityPassiveManager.AILevel < targetedArtwork.artLevel);
	}
	public void OpenArtwork(Artwork targetArt)
	{
		targetedArtwork = targetArt;
		gameObject.SetActive(true);

		audioPlayer.PlayOneShot(openSfx, 1f);
	}

	public void StealButton()
	{
		targetedArtwork.Steal(player);
		UpdateDisplay();

		audioPlayer.PlayOneShot(stealSfx, 1f);
	}
	public void ReplaceButton()
	{
		targetedArtwork.Replace(AbilityPassiveManager.AILevel);
		UpdateDisplay();

		audioPlayer.PlayOneShot(replaceSfx, 1f);
	}
	public void CloseButton()
	{
		gameObject.SetActive(false);
	}
}
