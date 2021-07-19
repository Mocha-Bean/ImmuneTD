using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public enum StatusEffect
    {
        AttackSpeedUp,
        AttackSpeedDown,
        KnownAttackSpeed,
        AttackDamageUp,
        AttackDamageDown,
        PassiveHeal,
        PassiveDmg,
        MoveSpeed,
        MoveSlow,
        MoveBlock,
        DefenseUp,
        DefenseDown,
    }

    public enum Team
    {
        Enemy,
        Friendly
    }

    public class Gun
    {
        public static Bullet ShootAtMousePos(GameObject bulletPrefab, Attacker source, Vector3 spawnPos, Color color)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return ShootAtPos(bulletPrefab, source, spawnPos, mousePos, color);
        }
        public static Bullet ShootAtPos(GameObject bulletPrefab, Attacker source, Vector3 spawnPos, Vector3 targetPos, Color color)
        {
            Bullet bullet = Shoot(bulletPrefab, source, spawnPos, color);
            bullet.ShootTowards(targetPos);
            return bullet;

        }
        public static Bullet ShootAtAttacker(GameObject bulletPrefab, Attacker source, Vector3 spawnPos, Attacker target, Color color)
        {
            Bullet bullet = Shoot(bulletPrefab, source, spawnPos, color);
            bullet.ShootAt(target);
            return bullet;
        }
        private static Bullet Shoot(GameObject bulletPrefab, Attacker source, Vector3 spawnPos, Color color)
        {
            GameObject bulletObject = GameObject.Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            SpriteRenderer bulletSR = bulletObject.GetComponent<SpriteRenderer>();
            bullet.shotBy = source;
            bulletSR.color = color;
            return bullet;
        }
    }
}