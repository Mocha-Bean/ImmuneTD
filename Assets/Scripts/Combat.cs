using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public enum StatusEffect
    {
        AttackSpeedUp,
        AttackSpeedDown,
        AttackDamageUp,
        AttackDamageDown,
        PassiveHeal,
        PassiveDmg,
        MoveSpeed,
        MoveSlow,
        DefenseUp,
        DefenseDown,
    }

    public enum Team
    {
        Enemy,
        Friendly
    }
}