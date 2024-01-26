using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ability
{
    public Ability(float cd, IEnumerator action)
    {
        this.action = action;
        this.currentCd = cd;
        this.cd = cd;

    }

    public IEnumerator action;
    public float currentCd;
    private float cd;

    public float Cd
    {
        get { return cd; }
    }
};

public class AbilityManager : MonoBehaviour
{
    private Dictionary<string, Ability> abilities;

    // Start is called before the first frame update
    public void UseAbility(string name)
    {
        if (!abilities.ContainsKey(name)) return;

        if (abilities[name].currentCd == 0)
        {
            Ability a = abilities[name];
            StartCoroutine(a.action);
            a.currentCd = a.Cd;
            abilities[name] = a;

        }
    }
    public void Update()
    {
        foreach (string k in abilities.Keys)
        {
            Ability a = abilities[k];
            a.currentCd -= Time.deltaTime;
            if (a.currentCd < 0.0f)
                a.currentCd = 0.0f;
            abilities[k] = a;
        }

    }
}
