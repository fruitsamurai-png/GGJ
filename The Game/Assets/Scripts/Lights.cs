using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    [Tooltip("The radius to notify guards of this distraction")]
    public float notifyRange = 50f;
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
			lit.enabled = false;
        }
        else
        {
			lit.enabled = true;
        }
        foreach (Collider c in Physics.OverlapSphere(transform.position, notifyRange, LayerMask.GetMask("Guards")))
        {
            GameObject o = c.gameObject;
            if (o.TryGetComponent(out GuardEnemyBehavior gb))
            {
                gb.NotifyDistraction(gameObject);
            }
        }
    }
}
