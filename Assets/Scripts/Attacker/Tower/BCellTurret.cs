using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class BCellTurret : Turret
{
    private BCellTurret()
    {
        healthMax = 100;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 0;
        attackSpeed = 1.5f;
        team = Team.Friendly;
        targetTeam = Team.Enemy;
        attackEffects = new HashSet<StatusEffect>() { StatusEffect.DefenseDown, StatusEffect.MoveSlow };
        bulletColor = new Color(0.0875f, 0.2056f, 0.54f, 1f);
    }
}
