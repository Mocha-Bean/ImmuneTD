using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class Staphylococcus : MovingShooter
{
    private Staphylococcus()
    {
        healthMax = 75;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 15;
        attackSpeed = 0.5f;
        team = Team.Enemy;
        targetTeam = Team.Friendly;
        moveSpeed = 1.5f;
        numShoot = 1;
        bulletColor = Color.white;
    }
}
