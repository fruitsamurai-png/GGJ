using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artwork : MonoBehaviour
{

    bool isStolen = false;
    int replicaLevel = 1;
    [SerializeField]
    string artWorkName;
    [SerializeField]
    GameObject artWorkUI;

    public bool IsStolen
    {
        get { return isStolen; }
    }
    public int ReplicaLevel
    {
        get { return replicaLevel; }
        set { replicaLevel = value; }
    }

    public void Interact(GameObject interactor)
    {
        //pop ui
        artWorkUI.SetActive(true);
    }
    public void Steal(GameObject interactor)
    {
        isStolen = true;
        artWorkUI.SetActive(false);
    }
    public void Replace(int level)
    {
        artWorkUI.SetActive(false);

    }


}
