using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class MeleeTower : Attacker
{
    protected int blockNum;
    private HashSet<MovingAttacker> targetsBlocking = new HashSet<MovingAttacker>();
    private HashSet<MovingAttacker> newTargetsBlocking = new HashSet<MovingAttacker>();
    private HashSet<Attacker> targetsRemaining = new HashSet<Attacker>();
    private Attacker target;
    private float totalValue;
    private float totalValueNew;

    [SerializeField]
    private Animator weaponAnimator;

    public MeleeTower()
    {
        attackEffects = new HashSet<StatusEffect>() { StatusEffect.MoveBlock };
        totalValue = 0;
    }

    protected override void TimedAttack()
    {
        if (newTargetsBlocking.Count != 0)
        {
            weaponAnimator.SetTrigger("Attack");
        }
        foreach (MovingAttacker attackTarget in newTargetsBlocking)
        {
            bool targetAlive = attackTarget.Attack(effectiveDamage, attackEffects, this);
            if (targetAlive)       // if we killed the target, don't call their method!
            {
                attackTarget.BlockBy(this);
            }
            else
            {
                Debug.Log("gottem");
                // target is dead, note this
            }
        }
    }

    protected void BlockRemove(MovingAttacker target)
    {
        target.activeEffects.Remove(StatusEffect.MoveBlock);
        target.Attack(0f, new HashSet<StatusEffect>() { }, this);  
    }

    private void HandleNewMovingAttacker()
    {
        targetsRemaining.Clear();
        targetsRemaining.UnionWith(TargetsInRange);
        newTargetsBlocking.Clear();
        target = null;
        for (int i = 0; i < blockNum; i++)   // find the best priority target(s) to block and put them in newTargetsBlocking
        {
            do
            {
                target = GetPriorityTarget(targetsRemaining);
                targetsRemaining.Remove(target);
            } while (!(target is MovingAttacker) && targetsRemaining.Count != 0);      // find the highest-priority target in range that is a MovingAttacker
            if (target is MovingAttacker)
            {
                newTargetsBlocking.Add((MovingAttacker)target);
                totalValueNew += GetPriorityValue(target);
            }
            else if (newTargetsBlocking.Count == 0)
            {
                awake = false;
            }

        }
        foreach (MovingAttacker attacker in newTargetsBlocking)
        {
            totalValueNew += GetPriorityValue(attacker);
        }
        if (totalValue >= totalValueNew)
        {
            newTargetsBlocking.Clear();
            newTargetsBlocking.UnionWith(targetsBlocking);
        }
        targetsBlocking.ExceptWith(newTargetsBlocking);
        foreach (MovingAttacker missingTarget in targetsBlocking)
        {
            if (missingTarget)  // sanity check
            {
                if (missingTarget.blockedBy == this)     // were we the last ones blocking this Attacker?
                {
                    missingTarget.blockedBy = null;
                    BlockRemove(missingTarget);
                }
            }
        }
        foreach (MovingAttacker target in newTargetsBlocking)
        {
            target.Attack(0f, attackEffects, this);     // send the targets a 0-damage attack to immediately block them
        }
        targetsBlocking.Clear();
        targetsBlocking.UnionWith(newTargetsBlocking);  // set this so we can check it next cycle
        totalValue = totalValueNew;
    }

    protected override void HandleNewTarget(Attacker target)
    {
        if(target is MovingAttacker)
        {
            HandleNewMovingAttacker();
        }
    }

    protected override void HandleLeaveTarget(Attacker target)
    {
        MovingAttacker movingTarget;
        if (target)
        {
            if (target is MovingAttacker)
            {
                movingTarget = (MovingAttacker)target;
                if (targetsBlocking.Contains(movingTarget))
                {
                    BlockRemove(movingTarget);
                    targetsBlocking.Remove(movingTarget);
                    totalValue = 0;
                    foreach (MovingAttacker blockTarget in targetsBlocking)
                    {
                        totalValue += GetPriorityValue(blockTarget);
                    }
                    HandleNewMovingAttacker();
                }
            }
        }
    }
}
