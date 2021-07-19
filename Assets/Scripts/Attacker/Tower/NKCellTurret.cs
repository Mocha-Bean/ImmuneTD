using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class NKCellTurret : Turret
{
    public NKCellTurret()
    {
        healthMax = 100;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 30;
        attackSpeed = 0.25f;
        team = Team.Friendly;
        targetTeam = Team.Enemy;
        bulletColor = new Color(0.377f, 0.377f, 0.377f, 1f);
    }
}
