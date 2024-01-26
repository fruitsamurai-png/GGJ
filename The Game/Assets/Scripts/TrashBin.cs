using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrashBin : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    void Start()
    {
        rb= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyUp(KeyCode.Space))
        //{
        //    Knockover();
        //}
    }
    public void Knockover()
    {
        rb.freezeRotation = false;
        rb.constraints= RigidbodyConstraints.None;
        Vector3 randomVector = new Vector3(Random.Range(-10f, 10f), Random.Range(0f, 5f), Random.Range(-5f, 5f));
        rb.velocity= randomVector;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if(collision.gameObject.tag=="Player")
            {
                Knockover();
                Debug.Log("KnockedOver");
            }
            else
            {
                Debug.Log(collision.gameObject.tag);
            }
        }
    }
}
