using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class Streptococcus : MovingShooter
{
    private Streptococcus()
    {
        healthMax = 100;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 20;
        attackSpeed = 0.25f;
        team = Team.Enemy;
        targetTeam = Team.Friendly;
        moveSpeed = 1.5f;
        numShoot = 1;
        bulletColor = Color.white;
    }
}
