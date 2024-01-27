using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    [SerializeField]
    private Ability ability;
    [SerializeField]
    private KeyCode key;
    private float cd;
    private float activeDur;
 
    enum AbilityState { Ready, Active, Cooldown }
    AbilityState state = AbilityState.Ready;


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AbilityState.Ready:
                if(Input.GetKeyDown(key))
                {
                    ability.OnTrigger();
                    state = AbilityState.Active;
                    activeDur = ability.activeDur;
                }
                break;
            case AbilityState.Active:
                if(activeDur > 0)
                {
                    activeDur -= Time.deltaTime;
                    ability.OnActive();
                }
                else
                {
                    state = AbilityState.Cooldown;
                    cd = ability.cd;
                }
                break;
            case AbilityState.Cooldown:
                if (cd > 0)
                {
                    cd -= Time.deltaTime;
                    ability.OnCooldown();
                }
                else
                    state = AbilityState.Cooldown;
                
                break;
        }
    }
}