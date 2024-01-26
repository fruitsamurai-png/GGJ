using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject cam;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float smoothTime = 0.2f;
    [SerializeField]
    private Vector3 smoothVelocity = new Vector3(0.5f,0.5f,0.5f);
    // Start is called before the first frame update

    private Vector3 internalOffset = Vector3.zero;
    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (!cam || !target) return; 
#endif

        cam.transform.position = Vector3.SmoothDamp(
            cam.transform.position,
            target.position + offset + internalOffset,
            ref smoothVelocity,
            smoothTime
        );

    }
    private void Update()
    {
#if UNITY_EDITOR
        if (!cam || !target)
        {
            Debug.LogError("No target or camera assigned to CameraFollow.cs");
            return;
        }
#endif
        RaycastHit hit;
        if (Physics.Linecast(cam.transform.position, target.position, out hit))
        {
            Vector3 n = (hit.point - cam.transform.position).normalized;
            internalOffset = n * hit.distance * 0.5f;
        }
    }
}
