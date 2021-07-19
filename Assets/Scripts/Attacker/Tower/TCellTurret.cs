using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class TCellTurret : Turret
{
    public TCellTurret()
    {
        healthMax = 100;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 5;
        attackSpeed = 1;
        team = Team.Friendly;
        targetTeam = Team.Enemy;
        bulletColor = Color.white;
    }
}
