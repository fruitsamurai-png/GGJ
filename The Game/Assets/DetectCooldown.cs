using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class DetectCooldown : MonoBehaviour
{
    [SerializeField]
    string abilityName;

    Image imageComp;
    AbilityHolder abilityHolder;
    float recipocal;
    // Start is called before the first frame update
    void Start()
    {
        imageComp = GetComponent<Image>();

        GameObject player = GameObject.FindWithTag("Player");
        AbilityHolder[] comps = player.GetComponents<AbilityHolder>();
        foreach(var c in comps)
        {
            if (c.GetAbilityName().ToLower() == abilityName.ToLower()) //hack to get the correct ref
            {
                //Debug.Log("set for " + abilityName);
                abilityHolder = c;
                break;
            }
        }
        recipocal = 1.0f / abilityHolder.GetOverallCd();
    }

    // Update is called once per frame
    void Update()
    {
        float fill = 1.0f - abilityHolder.GetCurrentCd() * recipocal;
        imageComp.fillAmount = fill;
    }
}
