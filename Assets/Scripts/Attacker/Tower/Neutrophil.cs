using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class Neutrophil : MeleeTower
{
    private Neutrophil()
    {
        healthMax = 100;
        health = healthMax;
        effectiveHP = health * defenseMult;
        attackDamage = 30;
        attackSpeed = 1f;
        team = Team.Friendly;
        targetTeam = Team.Enemy;
        blockNum = 1;
    }
}
