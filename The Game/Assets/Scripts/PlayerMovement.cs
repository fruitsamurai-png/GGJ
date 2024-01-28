using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

	[SerializeField]
	private float moveSpd = 5.0f;
	[SerializeField]
	private float sprintSpd = 10.0f;
	[SerializeField]
	private float noiseGen = 0.5f; // noise added by sprint
	[SerializeField]
	private float noiseRadius = 5.0f; // noise detection radius
	bool collided = false;

	private CharacterController cc;
	private bool sprintToggle = false;
	// Start is called before the first frame update

	void Start()
	{
		cc = GetComponent<CharacterController>();
	}

	void UpdateNoiseForGuards()
	{
		var cols = Physics.OverlapSphere(transform.position, noiseRadius, LayerMask.GetMask("Guards"));

		foreach (Collider c in cols)
		{
			GuardEnemyBehavior comp = c.gameObject.GetComponent<GuardEnemyBehavior>();

			if (comp)
			{
				comp.m_Enemy.Noise(noiseGen * Time.deltaTime); // add to this enemy noise level
			}
		}

	}
	// Update is called once per frame
	void Update()
	{
		sprintToggle = Input.GetKey(KeyCode.LeftShift);

		float spd = sprintToggle ? sprintSpd : moveSpd;
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		Vector3 movement = cc.isGrounded ? v * Vector3.forward + h * Vector3.right : Vector3.zero;

		if (sprintToggle && cc.isGrounded)
		{
			UpdateNoiseForGuards();
		}

		if (movement.sqrMagnitude > 0)
			transform.rotation = Quaternion.LookRotation(movement, Vector3.up);

		cc.SimpleMove(movement * spd);

		CheckforGuards();

		if (Input.GetKeyDown(KeyCode.Space))
		{
			InteractNearby(true);
		}
		else
		{
			InteractNearby(false);
		}
	}
	private void CheckforGuards()
	{
		float interactRange = 0.5f;
		float closestDistance = 10000f;
		GameObject closestEnemy = null;

		//Get the interactables within range and get the closest
		foreach (Collider c in Physics.OverlapSphere(transform.position, interactRange, LayerMask.GetMask("Guards")))
		{
			GameObject o = c.gameObject;
			float dist = Vector3.Distance(o.transform.position, transform.position);
			if (dist < closestDistance)
			{
				closestDistance = dist;
				closestEnemy = o;
			}
		}

		//Exit out if there was no interactable found
		if (closestEnemy == null)
		{
			return;
		}
		if (closestEnemy.TryGetComponent(out SecurityBotEnemyBehavior sec))
		{
			collided = sec.m_Enemy.isAltered;
			Caught();
			return;
		}
		if (closestEnemy.TryGetComponent(out GuardEnemyBehavior gua))
		{
			collided = gua.m_Enemy.isAltered;
			Caught();
			return;
		}
	}

	void Caught()
	{
		UIPostHeistMegaController.playerWasCaught = true;
		SceneManager.LoadScene("PostHeist");
	}

	void InteractNearby(bool activate = true)
	{
		float interactRange = 1.5f;
		float closestDistance = 10000f;
		GameObject closestInteractable = null;

		//Get the interactables within range and get the closest
		foreach (Collider c in Physics.OverlapSphere(transform.position, interactRange, LayerMask.GetMask("Interactable")))
		{
			GameObject o = c.gameObject;
			float dist = Vector3.Distance(o.transform.position, transform.position);
			if (dist < closestDistance)
			{
				closestDistance = dist;
				closestInteractable = o;
			}
		}

		//Exit out if there was no interactable found
		if (closestInteractable == null)
		{
			return;
		}

		UIInteractPrompt.ShowInteractable(closestInteractable);

		if (!activate)
			return;

		//Actually interact
		if (closestInteractable.TryGetComponent(out TrashBin trashbin))
		{
			trashbin.Knockover(gameObject);
			return;
		}
		if (closestInteractable.TryGetComponent(out LightSwitch light))
		{
			light.TriggerSwitch();
			return;
		}
		if (closestInteractable.TryGetComponent(out Artwork art))
		{
			art.Interact(gameObject);
			return;
		}
	}
}
