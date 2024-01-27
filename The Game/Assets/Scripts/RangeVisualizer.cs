using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeVisualizer : MonoBehaviour
{
    [SerializeField]
    JailBreak ability;
    [SerializeField]
    Color color;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, ability.QueryRadius);

    }
}
