using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class Hemocytoblast : AOETower
{
    private Hemocytoblast()
    {
        healthMax = 100;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 0;
        team = Team.Friendly;
        targetTeam = Team.Friendly;
        attackEffects = new HashSet<StatusEffect>() { StatusEffect.PassiveHeal };
        activeEffects = attackEffects;
    }
}
