using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExtractionZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			UIPostHeistMegaController.playerWasCaught = false;
			SceneManager.LoadScene("PostHeist");
		}
	}
}
