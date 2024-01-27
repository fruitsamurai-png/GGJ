using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlowTime : Ability
{

    public override void OnActive()
    {
        Debug.Log("Slow Time");
        Time.timeScale = 0.1f;

    }

    public override void OnCooldown()
    {
        Debug.Log("Re Time");

        Time.timeScale = 1.0f;
    }
}
