using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBox : MonoBehaviour
{
    [SerializeField]
    private Attacker parent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        parent.RangeBoxEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        parent.RangeBoxExit(collision);
    }
}
