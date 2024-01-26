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
    private Vector3 smoothVelocity = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField]
    private int playerLayer = 3;
    private Vector3 internalOffset = Vector3.zero;

    private void Start()
    {
        internalOffset = offset;
    }
    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (!cam || !target) return;
#endif
        cam.transform.position = Vector3.SmoothDamp(
          cam.transform.position,
          target.position + internalOffset,
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
        bool blocked = Physics.Linecast(cam.transform.position, target.position, out hit, playerLayer);
        internalOffset = blocked ? offset.normalized * (hit.point - target.position).magnitude : offset;
    }
}

