using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOETower : Attacker
{
    public TowerManager towerManager;
    public Vector3Int tilePos;

    protected override void HandleNewTarget(Attacker target)
    {
        Debug.Log(target.name);
        AttackInRange();
    }

    // since we have no check for attackers leaving range, this can only be used on non-moving attackers for now

    protected void AttackInRange()
    {
        foreach(Attacker target in TargetsInRange)
        {
            target.Attack(0, attackEffects, this);  // this has no cooldown; we can't apply direct damage
        }
    }

    protected override IEnumerator Kill()
    {
        towerManager.RequestDelete(tilePos);
        return base.Kill();
    }
}
