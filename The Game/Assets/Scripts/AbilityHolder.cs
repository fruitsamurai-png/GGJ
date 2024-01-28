using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
	[SerializeField]
	private Ability ability;
	[SerializeField]
	private KeyCode key;
	private float cd;
	private float activeDur;

	public AudioClip sfx;
	public GameObject particlePrefab;
	enum AbilityState { Ready, Active, Cooldown }
	AbilityState state = AbilityState.Ready;

	public string GetAbilityName() { return ability.abilityName; }
	public float GetCurrentCd() { return cd; }
	public float GetOverallCd() { return ability.cd; }



	// Update is called once per frame
	void Update()
	{
		switch (state)
		{
			case AbilityState.Ready:
				if (Input.GetKeyDown(key))
				{
					ability.OnTrigger();
					state = AbilityState.Active;
					activeDur = ability.activeDur;
					Instantiate(particlePrefab, transform.position + Vector3.up, Quaternion.identity);
					GetComponent<AudioSource>().PlayOneShot(sfx);
				}
				break;
			case AbilityState.Active:
				if (activeDur > 0)
				{
					activeDur -= Time.deltaTime;
					ability.OnActive();
				}
				else
				{
					state = AbilityState.Cooldown;
					cd = ability.cd;
				}
				break;
			case AbilityState.Cooldown:
				if (cd > 0)
				{
					cd -= Time.deltaTime;
					ability.OnCooldown();
				}
				else
				{
					state = AbilityState.Ready;
				}
				break;
		}
	}
}
