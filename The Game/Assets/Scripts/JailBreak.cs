using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class JailBreak : Ability
{
    [SerializeField]
    float queryRadius = 1.5f;
    [SerializeField]
    float enemyStunDur = 1.5f;
    [SerializeField]
    int playerLayer = 3;
    private bool canunlock = false;

    public float QueryRadius  { get => queryRadius; }
    public override void OnTrigger()
    {
        var go = GameObject.FindWithTag("Player");
        if (!go) return;
        Transform transform = go.transform;

        float closestDistance = Mathf.Infinity;
        GameObject closestInteractable = null;
        foreach (Collider c in Physics.OverlapSphere(transform.position, queryRadius, LayerMask.GetMask("Guards")| LayerMask.GetMask("Interactable")))
        {
            GameObject o = c.gameObject;
            float dist = Vector3.Distance(o.transform.position, transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestInteractable = o;
            }
        }

        //Exit out if there was no interactable found
        if (!closestInteractable)
        {
            return;
        }
        if (closestInteractable.TryGetComponent(out SecurityLock loc))
        {
            MasterManagers mm = MasterManagers.instance;
            canunlock = loc.Jailbreak(AbilityPassiveManager.AILevel);
            return;
        }
        if (closestInteractable.TryGetComponent(out GuardEnemyBehavior behavior))
        {
            MasterManagers mm = MasterManagers.instance;
            behavior.m_Enemy.Jailbreak(AbilityPassiveManager.AILevel, enemyStunDur);
            return;
        }
        
    }
}
