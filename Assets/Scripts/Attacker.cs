using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class Attacker : MonoBehaviour
{
    public float healthMax;
    public float health;
    public float passiveHeal;
    public float passiveHealDefault = 0;
    public float effectiveHP; // health * defenseMult
    public float attackDamage;
    public float effectiveDamage;   // attackDamage * attackDamageMult;
    public float attackSpeed;
    public Team team;
    public Team targetTeam; // for example, a healer would target its own team

    public HashSet<StatusEffect> activeEffects = new HashSet<StatusEffect>();
    public HashSet<StatusEffect> attackEffects = new HashSet<StatusEffect>();     // what debuff(s)/buff(s) do we give on attack?

    [SerializeField]        // <- for testing; we can un-serialize it later
    protected HashSet<Attacker> TargetsInRange = new HashSet<Attacker>();

    // do not change; these should stay as 1 by default.
    protected float attackSpeedMult = 1;
    protected float moveSpeedMult = 1;
    protected float defenseMult = 1;
    protected float attackDamageMult = 1;
    // -------------------------------------------------

    [SerializeField]
    private BoxCollider2D rangeBox;
    private Bullet bullet;

    // public methods:

    public void Attack(float dmg, HashSet<StatusEffect> effects, Attacker source)   // fyi: this doesn't send attacks, it receives them
    {
        if(dmg > 0)             // are we being damaged?
        {
            ChangeHealth(-dmg / defenseMult);
            HandleAttack(source);
        } else if (dmg < 0)     // are we being healed? (don't scale heal with defense)
        {
            ChangeHealth(-dmg);
        }
        if (!effects.IsSubsetOf(activeEffects))     // is this giving us any effects we don't already have?
        {
            activeEffects.UnionWith(effects);
            SetStatusEffects();
        }
    }

    public void RangeBoxEnter(Collider2D collision)
    {
        Attacker newAttacker;
        if (collision.gameObject.TryGetComponent<Attacker>(out newAttacker))
        {
            if (newAttacker.team == targetTeam)
            {
                TargetsInRange.Add(newAttacker);
                HandleNewTarget(newAttacker);
            }
        }
    }

    public void RangeBoxExit(Collider2D collision)
    {
        Attacker newAttacker;
        if (collision.gameObject.TryGetComponent<Attacker>(out newAttacker))
        {
            TargetsInRange.Remove(newAttacker);     // if newAttacker (the attacker leaving rangeBox) is not already in TargetsInRange,
            HandleLeaveTarget(newAttacker);         // this will just do nothing, as it should.
        }
    }

    public virtual void Kill()
    {
        Destroy(gameObject);    // override if animation is desired, or if there are other things we need to clean up upon death
    }

    // protected methods:

    protected virtual void HandleAttack(Attacker source)
    {
        // in case derived class wants to perform some behavior upon being attacked, e.g. switch aggro
    }

    protected virtual void HandleNewTarget(Attacker target)
    {
        // in case derived class wants to perform some behavior upon seeing a new target
        // in range (particularly for passive units that don't have an attackSpeed)
    }

    protected virtual void HandleLeaveTarget(Attacker target)
    {
        // in case derived class wants to perform some behavior upon a target leaving its
        // range, e.g. cleaning up debuffs
    }

    protected virtual void SetStatusEffects()
    {
        // reset all multipliers before setting them
        // we'd need to override this method to change default values (e.g. we want a unit with passiveHeal by default)
        // or if we want to give the unit different multipliers for certain buffs / debuffs
        attackSpeedMult = 1;
        attackDamageMult = 1;
        passiveHeal = passiveHealDefault;
        moveSpeedMult = 1;
        defenseMult = 1;
        foreach (StatusEffect effect in activeEffects)
        {
            switch (effect)
            {
                case StatusEffect.AttackSpeedUp:
                    attackSpeedMult *= 2f;
                    break;
                case StatusEffect.AttackSpeedDown:
                    attackSpeedMult *= (2f / 3f);
                    break;
                case StatusEffect.AttackDamageUp:
                    attackDamageMult *= 1.5f;
                    break;
                case StatusEffect.AttackDamageDown:
                    attackDamageMult *= 0.75f;
                    break;
                case StatusEffect.PassiveHeal:
                    passiveHeal += 0.1f;
                    break;
                case StatusEffect.PassiveDmg:
                    passiveHeal -= 0.1f;
                    break;
                case StatusEffect.MoveSpeed:
                    moveSpeedMult *= (4f / 3f);
                    break;
                case StatusEffect.MoveSlow:
                    moveSpeedMult *= 0.5f;
                    break;
                case StatusEffect.DefenseUp:
                    defenseMult *= 1.5f;
                    break;
                case StatusEffect.DefenseDown:
                    defenseMult *= (2f / 3f);
                    break;
            }
        }
        effectiveDamage = attackDamage * attackDamageMult;
        effectiveHP = health * defenseMult;
    }

    // private methods:

    private void ChangeHealth(float amt)
    {
        health += amt;
        if (health <= 0)
        {
            attackSpeedMult = 0;    // these variables are set so that if derived Attacker classes play
            moveSpeedMult = 0;      // an animation in Kill() overrides, they aren't moving or attacking
            Kill();                 // while doing so.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet;
        Attacker bulletSource;
        if (collision.gameObject.TryGetComponent<Bullet>(out bullet))
        {
            bulletSource = bullet.shotBy;
            if (bulletSource.targetTeam == team)    // was this bullet meant for me? (note: this check cannot be performed on real-life bullets. don't try this at home)
            {
                Attack(bulletSource.effectiveDamage, bulletSource.attackEffects, bulletSource);
                bullet.Pop();
            }
        }
    }

    private void OnEnable()
    {
        SetStatusEffects();
    }

    private void FixedUpdate()
    {
        if (passiveHeal != 0)
        {
            if (health + passiveHeal < healthMax)
            {
                health += passiveHeal;
            }
            else
            {
                health = healthMax;
            }
        }
    }
}
