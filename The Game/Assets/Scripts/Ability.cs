using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public string abilityName;
    public float cd;
    public float activeDur;

    public virtual void OnTrigger() { } 
    public virtual void OnActive() { }
    public virtual void OnCooldown() { }


}
