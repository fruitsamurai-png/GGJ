using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityLock : MonoBehaviour
{
    [SerializeField]
    private int Doorlevel = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool Jailbreak(int playerLevel)
    {
        if (playerLevel > Doorlevel)
        {
            gameObject.SetActive(false);
            return true;
        }
        else
            return false; ;
    }
}
