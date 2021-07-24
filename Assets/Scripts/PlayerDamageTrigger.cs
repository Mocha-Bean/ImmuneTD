using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageTrigger : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private AudioSource audioSource;

    private MovingAttacker attacker;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<MovingAttacker>(out attacker))
        {
            attacker.energyReward = 0;  // so the player doesn't get rewarded for this enemy's death
            gameManager.ReportEnemyDeath(attacker, attacker.energyReward);
            gameManager.ReduceHealth();
            audioSource.Play();
            attacker.Attack(attacker.healthMax * 5, new HashSet<Combat.StatusEffect>() { }, null);  // kill attacker
        }
    }
}
