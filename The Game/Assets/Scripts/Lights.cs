using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    private Light lit;
    // Start is called before the first frame update
    void Start()
    {
        lit=GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TurnOnOff(bool state)
    {
        if (!state) {
            lit.intensity = 0;
        }
        else
        {
            lit.intensity = 100;
        }
    }
}
