using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractPrompt : MonoBehaviour
{
	CanvasGroup cg;

	bool interactShow = false;
	GameObject interactable;

	public static UIInteractPrompt instance;
	void Start()
	{
		instance = this;
		cg = GetComponent<CanvasGroup>();
	}

	public static void ShowInteractable(GameObject interactable)
	{
		instance.interactable = interactable;
		instance.interactShow = true;
	}

	void Update()
	{
		float alphaGoal = 0f;

		if (interactable != null)
		{
			transform.position = Camera.main.WorldToScreenPoint(interactable.transform.position);

			if (interactShow)
			{
				alphaGoal = 1f;
			}
		}

		cg.alpha = Mathf.MoveTowards(cg.alpha, alphaGoal, 3f * Time.deltaTime);

		interactShow = false;
	}
}
