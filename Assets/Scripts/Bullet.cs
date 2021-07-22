using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speedMultiplier;
    public float homingMultiplier;
    public Attacker shotBy;

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

    public void ShootAt (Attacker target)
    {
        Vector3 moveVector = target.gameObject.transform.position - transform.position;
        moveVector.Normalize();
        moveVector = moveVector * speedMultiplier;
        rb2D.AddForce(moveVector, ForceMode2D.Impulse);
        if (target is MovingAttacker)
        {
            StartCoroutine(Homing((MovingAttacker)target, moveVector));
        }
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

    private IEnumerator Homing(MovingAttacker target, Vector3 prevVector)
    {
        while (target)
        {
            Vector3 moveVector = target.gameObject.transform.position - transform.position;
            moveVector.Normalize();
            moveVector = moveVector * speedMultiplier;
            Vector3 adjust = moveVector - prevVector;
            adjust *= homingMultiplier;
            rb2D.AddForce(adjust, ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
    }
}
