using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Attacker
{
    public Color bulletColor = Color.white;
    public TowerManager towerManager;
    public Vector3Int tilePos;

    [SerializeField]
    private Transform gunTransform;
    [SerializeField]
    private Animator gunAnimator;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private AudioSource audioSource;

    protected override void TimedAttack()
    {
        gunAnimator.SetTrigger("Shoot");
        audioSource.Play();
        Attacker target = GetPriorityTarget(TargetsInRange);
        Vector3 diff = transform.position - target.gameObject.transform.position;
        float diffAngle = Mathf.Atan2(diff.y, diff.x);
        gunTransform.rotation = Quaternion.Euler(0f, 0f, diffAngle * Mathf.Rad2Deg);
        Combat.Gun.ShootAtAttacker(bulletPrefab, this, gunTransform.position, target, bulletColor);
    }

    protected override IEnumerator Kill()
    {
        towerManager.RequestDelete(tilePos);
        return base.Kill();
    }
}