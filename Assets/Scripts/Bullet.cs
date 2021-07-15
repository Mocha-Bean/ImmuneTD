using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speedMultiplier;

    [SerializeField]
    private Rigidbody2D rb2D;
    [SerializeField]
    private Animator anim;

    public void ShootTowards (Vector3 pos)
    {
        Vector3 moveVector = pos - transform.position;
        moveVector.Normalize();
        moveVector = moveVector * speedMultiplier;
        rb2D.AddForce(moveVector, ForceMode2D.Impulse);
    }

    public void Pop()
    {
        rb2D.Sleep();   // animation looks better when the bullet stops moving while popping
        anim.SetTrigger("Pop");
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
