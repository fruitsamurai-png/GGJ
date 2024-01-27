using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlowTime : Ability
{
    [Range(0.0f, 1.0f)]
    public float slowMult = 1.0f;
    public override void OnTrigger()
    {
        GuardEnemyBehavior.speedMult = slowMult;
    }

    public override void OnActive()
    {
        GuardEnemyBehavior.speedMult = slowMult;
        Debug.Log("Slow guards");
    }
    public override void OnCooldown()
    {
        GuardEnemyBehavior.speedMult = 1.0f;
        Debug.Log("Resume guards");

    }
}
