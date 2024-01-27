using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBubble : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform mainCameraTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCameraTransform = mainCamera.transform;
            }
            else
            {
                // Handle the case when the camera is not found
                return;
            }
        }
    }
}
