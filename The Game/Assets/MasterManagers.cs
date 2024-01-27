using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManagers : MonoBehaviour
{
    public static MasterManagers instance;
    public AbilityManager abilityManager;
    private void Awake()
    {
        if (instance && instance != this)
            Destroy(this);
        else
            instance = this;

        abilityManager = GetComponent<AbilityManager>();
    }
}
