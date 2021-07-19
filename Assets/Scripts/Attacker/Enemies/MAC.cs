using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class MAC : MovingAttacker
{
    public MAC()
    {
        healthMax = 50;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 5;
        attackSpeed = 1.25f;
        team = Team.Enemy;
        targetTeam = Team.Friendly;
        moveSpeed = 2;
    }
}
