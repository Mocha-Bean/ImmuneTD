using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingShooter : MovingAttacker
{
    protected HashSet<Attacker> targetsRemaining = new HashSet<Attacker>();
    protected HashSet<Attacker> targetsSelected = new HashSet<Attacker>();
    private Attacker priorityTarget;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    protected Color bulletColor;
    [SerializeField]
    protected int numShoot;

    protected override void TimedAttack()
    {
        StartCoroutine(DelayAttack());
    }

    protected IEnumerator DelayAttack()
    {
        yield return new WaitForFixedUpdate();  // avoid race condition with multiple target finding
        foreach (Attacker target in targetsSelected)
        {
            Combat.Gun.ShootAtAttacker(bulletPrefab, this, transform.position, target, bulletColor);
        }
    }

    protected override void HandleNewTarget(Attacker target)
    {
        RefreshTargets();
    }

    protected override void HandleLeaveTarget(Attacker target)
    {
        RefreshTargets();
    }

    protected void RefreshTargets()
    {
        targetsRemaining.Clear();
        targetsRemaining.UnionWith(TargetsInRange);
        targetsSelected.Clear();
        for (int i = 0; i < numShoot; i++)   // find the best priority target(s) to shoot and put them in targetsSelected
        {
            if (targetsRemaining.Count != 0)
            {
                priorityTarget = GetPriorityTarget(targetsRemaining);
                targetsRemaining.Remove(priorityTarget);
                targetsSelected.Add(priorityTarget);
            }
        }
    }
}
