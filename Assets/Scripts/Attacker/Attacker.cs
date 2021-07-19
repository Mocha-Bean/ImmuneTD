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
    public bool awake;
    public bool attackCooldown;
    public Team team;
    public Team targetTeam; // for example, a healer would target its own team

    public HashSet<StatusEffect> activeEffects = new HashSet<StatusEffect>();
    public HashSet<StatusEffect> attackEffects = new HashSet<StatusEffect>();     // what debuff(s)/buff(s) do we give on attack?
    public Dictionary<StatusEffect, float> effectModifiers = new Dictionary<StatusEffect, float>
    {
        {StatusEffect.AttackSpeedUp, 4f/3f},
        {StatusEffect.KnownAttackSpeed, 2f},
        {StatusEffect.AttackSpeedDown, 2f/3f},
        {StatusEffect.AttackDamageUp, 1.5f},
        {StatusEffect.AttackDamageDown, 0.75f},
        {StatusEffect.PassiveHeal, 0.1f},
        {StatusEffect.PassiveDmg, 0.1f},
        {StatusEffect.MoveSpeed, 4f/3f},
        {StatusEffect.MoveSlow, 0.5f},
        {StatusEffect.MoveBlock, 0f},
        {StatusEffect.DefenseUp, 1.5f},
        {StatusEffect.DefenseDown, 2f/3f}
    };

    protected HashSet<Attacker> TargetsInRange = new HashSet<Attacker>();

    // do not change; these should stay as 1 by default.
    protected float attackSpeedMult = 1;
    protected float moveSpeedMult = 1;
    protected float defenseMult = 1;
    protected float attackDamageMult = 1;
    // -------------------------------------------------

    private Bullet bullet;

    // public methods:

    public bool Attack(float dmg, HashSet<StatusEffect> effects, Attacker source)   // fyi: this doesn't send attacks, it receives them
    {                                                                               // returns true if still alive afterwards
        bool aliveStatus = (health > 0);
        if (aliveStatus)
        {
            if (dmg > 0)             // are we being damaged?
            {
                aliveStatus = ChangeHealth(-dmg / defenseMult);
                HandleAttack(source);

            }
            else if (dmg < 0)     // are we being healed? (don't scale heal with defense)
            {
                ChangeHealth(-dmg);
            }
            else if (dmg == 0)
            {
                SetStatusEffects(); // manual effect refresh
            }
            if (!effects.IsSubsetOf(activeEffects))     // is this giving us any effects we don't already have?
            {
                activeEffects.UnionWith(effects);
                SetStatusEffects();
            }
            if (!aliveStatus)
            {
                StartCoroutine(Kill());
            }
        } else
        {
            StartCoroutine(Kill());
        }
        return aliveStatus;
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
                if (!awake)
                {
                    StartCoroutine(WakeUp());
                }
            }
        }
    }

    public void RangeBoxExit(Collider2D collision)
    {
        Attacker oldAttacker;
        if (collision.gameObject.TryGetComponent<Attacker>(out oldAttacker))
        {
            TargetsInRange.Remove(oldAttacker);     // if oldAttacker was not already in TargetsInRange, this will just do nothing, as it should.
            if (TargetsInRange.Count == 0)
            {
                awake = false;
            }
            HandleLeaveTarget(oldAttacker);
        }
    }

    public virtual Attacker GetPriorityTarget(HashSet<Attacker> targets)
    {
        Attacker priority = null;
        foreach(Attacker attacker in targets)
        {
            if(priority == null)
            {
                priority = attacker;
            }
            else
            {
                if (GetPriorityValue(attacker) > GetPriorityValue(priority)){
                    priority = attacker;
                }
            }
        }
        return priority;
    }

    public virtual float GetPriorityValue(Attacker target)  // return some float value to represent relative value of target
    {
        return target.effectiveHP;
    }

    // protected methods:

    protected virtual void TimedAttack()
    {
        // let derived class decide how to perform attack
    }

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
                case StatusEffect.KnownAttackSpeed:
                    attackSpeedMult *= effectModifiers[effect];
                    break;
                case StatusEffect.AttackSpeedDown:
                    attackSpeedMult *= effectModifiers[effect];
                    break;
                case StatusEffect.AttackDamageUp:
                    attackDamageMult *= effectModifiers[effect];
                    break;
                case StatusEffect.AttackDamageDown:
                    attackDamageMult *= effectModifiers[effect];
                    break;
                case StatusEffect.PassiveHeal:
                    passiveHeal += effectModifiers[effect];
                    break;
                case StatusEffect.PassiveDmg:
                    passiveHeal -= effectModifiers[effect];
                    break;
                case StatusEffect.MoveSpeed:
                    moveSpeedMult *= effectModifiers[effect];
                    break;
                case StatusEffect.MoveBlock:
                case StatusEffect.MoveSlow:
                    moveSpeedMult *= effectModifiers[effect];
                    break;
                case StatusEffect.DefenseUp:
                    defenseMult *= effectModifiers[effect];
                    break;
                case StatusEffect.DefenseDown:
                    defenseMult *= effectModifiers[effect];
                    break;
            }
        }
        effectiveDamage = attackDamage * attackDamageMult;
        effectiveHP = health * defenseMult;
    }

    // private methods:

    protected IEnumerator WakeUp()    // calls TimedAttack() in sync with attackSpeed
    {
        awake = true;
        while (awake && (health > 0))
        {
            TimedAttack();
            attackCooldown = true;
            yield return new WaitForSeconds(1f / (attackSpeed * attackSpeedMult));
            attackCooldown = false;
        }
    }
    protected IEnumerator Kill()
    {
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }

    private bool ChangeHealth(float amt)    // returns true if still alive
    {
        health += amt;
        if (health <= 0)
        {
            attackSpeedMult = 0;    // these variables are set so that if derived Attacker classes play
            moveSpeedMult = 0;      // an animation in Kill() overrides, they aren't moving or attacking
            return false;           // while doing so.
        }
        else
        {
            return true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
