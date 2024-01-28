using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ResetToMenuButton : MonoBehaviour
{
   public void BackToMenu()
	{
		StealingInventory.Reset();
		AbilityPassiveManager.Reset();
		SceneManager.LoadScene("MainMenu");
	}
}
