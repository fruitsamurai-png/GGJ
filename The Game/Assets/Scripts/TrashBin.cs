using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrashBin : MonoBehaviour
{
	[Tooltip("The radius to notify guards of this distraction")]
	public float notifyRange = 50f;

	private Rigidbody rb;
	private Collider mainCollider;
	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		mainCollider = GetComponent<Collider>();
	}

	// Update is called once per frame
	void Update()
    {
    }

	public void Knockover(GameObject interactor)
	{
		rb.freezeRotation = false;
		rb.constraints = RigidbodyConstraints.None;

		//Get a random magnitude vector of the push direction from the interactor
		Vector3 randomVector = (transform.position - interactor.transform.position);
		randomVector.y = 0f;
		randomVector = randomVector.normalized * Random.Range(5f, 10f);
		rb.velocity = randomVector;

		//Disable the main collider so it doesn't block the player anymore
		mainCollider.enabled = false;

		//Notify nearby guards
		//GameObject.Find("Guard").GetComponent<GuardEnemyBehavior>().m_Enemy.Alert(gameObject);
		foreach (Collider c in Physics.OverlapSphere(transform.position, notifyRange, LayerMask.GetMask("Guards")))
		{
			GameObject o = c.gameObject;
			if(o.TryGetComponent(out GuardEnemyBehavior gb))
			{
				gb.NotifyDistraction(gameObject);
			}
		}

	}
	private void OnCollisionEnter(Collision collision)
	{
		//if (collision != null)
		//{
		//    if(collision.gameObject.tag=="Player")
		//    {
		//        Knockover();
		//        Debug.Log("KnockedOver");
		//    }
		//    else
		//    {
		//        Debug.Log(collision.gameObject.tag);
		//    }
		//}
	}
}
