using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

	[SerializeField]
	private float moveSpd = 5.0f;
	[SerializeField]
	private float sprintSpd = 10.0f;
	private CharacterController cc;
	private bool sprintToggle = false;

	// Start is called before the first frame update
	void Start()
	{
		cc = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
			sprintToggle = !sprintToggle;
		float spd = sprintToggle ? sprintSpd : moveSpd;
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		Vector3 movement = v * Vector3.forward + h * Vector3.right;
		if (movement.sqrMagnitude > 0)
			transform.rotation = Quaternion.LookRotation(movement, Vector3.up);

		cc.SimpleMove(movement * spd);


		if (Input.GetKeyDown(KeyCode.Space))
		{
			InteractNearby();
		}
	}
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
	}

	void InteractNearby()
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
    }
}
