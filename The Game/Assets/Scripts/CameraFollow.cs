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


    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (!cam || !target) return;
#endif
        cam.transform.position = Vector3.SmoothDamp(
          cam.transform.position,
          target.position + offset,
          ref smoothVelocity,
          smoothTime
      );
    }

}

