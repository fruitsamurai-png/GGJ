using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBubble : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform mainCameraTransform;
    private TextMeshPro textComponent;
    public string[] messages;
    public float switchInterval = 2.0f;
    private bool alerted = false;
    private int currentIndex = 0;
    private float lastSwitchTime;
    void Start()
    {
        // Assuming you have a reference to the parent GameObject's Transform
        Transform parentTransform = transform;

        // Specify the index of the child (0 for the first child, 1 for the second, and so on)
        int childIndex = 0;

        // Get the child Transform
        Transform childTransform = parentTransform.GetChild(childIndex);

        // Access the child GameObject
        GameObject childGameObject = childTransform.gameObject;
        textComponent = childGameObject.GetComponent<TextMeshPro>();
        gameObject.SetActive(false);
        textComponent.text = messages[currentIndex];
        //// Start the timer coroutine
        //StartCoroutine(SwitchTextTimer());
    }

    // Update is called once per frame
    void Update()
    {
        Lookat();
        // Check if enough time has passed to switch text
        if (Time.time - lastSwitchTime >= switchInterval &&alerted)
        {
            SwitchText();
            lastSwitchTime = Time.time;
        }
    }
    void SwitchText()
    {
        // Update the text component with the next message
        textComponent.text = messages[currentIndex];

        // Increment the index for the next message
        currentIndex = (currentIndex + 1) % messages.Length;
    }
    void Lookat()
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
        transform.LookAt(mainCameraTransform.position, Vector3.up);
    }
    // Call this function to restart the coroutine and enable/disable the GameObject
    public void TriggerTextSwitch(bool alert)
    {
        alerted = alert;
        gameObject.SetActive(alerted);
    }
}
