using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject[] lightarray;
    private bool trigger=false;

	public AudioClip onSfx;
    // Start is called before the first frame update
    void Start()
    {
     
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    TriggerSwitch();
        //}
    }
    public void TriggerSwitch()
    {
        foreach (GameObject light in lightarray)
        {
            light.GetComponent<Lights>().TurnOnOff(trigger);
        }
        trigger = !trigger;

		GetComponent<AudioSource>().PlayOneShot(onSfx);
    }
}
