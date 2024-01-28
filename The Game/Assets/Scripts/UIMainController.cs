using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIMainController : MonoBehaviour
{
	public UIStealArtwork stealArtworkUI;
	public Image fader;
	public TextMeshProUGUI aiLevelText;

	public bool doFader = false;

	private void Awake()
	{
		UIStealArtwork.instance = stealArtworkUI;

		fader.gameObject.SetActive(true);
		Color c = fader.color;
		c.a = 1f;
		fader.color = c;
	}

	private void Update()
	{
		Color c = fader.color;
		c.a = Mathf.MoveTowards(c.a, doFader ? 1f : 0f, Time.deltaTime);
		fader.color = c;

		aiLevelText.text = AbilityPassiveManager.AILevel.ToString();
	}
}
