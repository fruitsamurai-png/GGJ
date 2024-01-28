using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIPostHeistStolenPaintingEntry : MonoBehaviour
{
	public TextMeshProUGUI priceText;
	public StolenArt art;
	public UIArtworkDisplay display;
	void Start()
	{
		UpdateArt();
	}

	public void SellButton()
	{
		StealingInventory.SellArt(art);
	}
	public void TrainButton()
	{
		StealingInventory.TrainArt(art);
	}

	public void UpdateArt()
	{
		display.artwork = art.artwork;
		priceText.text = $"${(StealingInventory.moneyPerArtLevel * art.level):n0}";
		display.UpdateArtwork();
	}
}
