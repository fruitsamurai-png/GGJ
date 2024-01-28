using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLightController : MonoBehaviour
{
	Light light;
	public Color peaceColor;
	public Color alertColor1;
	public Color alertColor2;

	// Start is called before the first frame update
	void Start()
	{
		light = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
	{
		Color c = peaceColor;
		if (AlertManager.alert)
		{
			c = Vector4.Lerp(alertColor1, alertColor2, Mathf.PingPong(Time.time * 3f, 1f));
		}

		light.color = Vector4.MoveTowards(light.color, c, 3f * Time.deltaTime);
	}
}
