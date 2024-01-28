using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertBar : MonoBehaviour
{
	public Transform bar;
	public float alertValue = 0f;

	Vector3 baseScale;
	void Start()
	{
		baseScale = bar.localScale;
	}
	void Update()
	{
		transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);

		Vector3 s = baseScale;
		s.x = s.x * alertValue;
		bar.localScale = s;

		if (alertValue <= 0f)
		{
			gameObject.SetActive(false);
		}
	}
}
