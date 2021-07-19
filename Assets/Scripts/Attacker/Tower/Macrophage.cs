using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class Macrophage : MeleeTower
{
    private Macrophage()
    {
        healthMax = 200;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 100;
        attackSpeed = 0.3f;
        team = Team.Friendly;
        targetTeam = Team.Enemy;
        blockNum = 2;
    }
}
