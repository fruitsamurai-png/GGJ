using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{ 
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float smoothTime = 0.2f;
    [SerializeField]
    private Vector3 smoothVelocity = new Vector3(0.5f, 0.5f, 0.5f);
    private bool blocked = false;
    private Vector3 realOffset;

    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (!target) return;
#endif

        transform.position = Vector3.SmoothDamp(
          transform.position,
          target.position + offset,
          ref smoothVelocity,
          smoothTime
      );
        //transform.LookAt(target);
      //Debug.Log(realOffset);
    }

    //private void FixedUpdate()
    //{
    //    RaycastHit hit;
    //    blocked = Physics.Linecast(transform.position,target.position, out hit , 3);
    //    realOffset = blocked ? Vector3.zero : offset;

    //    Debug.Log(realOffset);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    realOffset = offset;
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    realOffset = Vector3.zero;

    //}

}
