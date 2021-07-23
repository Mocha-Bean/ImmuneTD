using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Keeps track of how healthy immune system is
    [SerializeField]
    private int health;
    // Energy can be used to place towers. Replenishes before each round.
    [SerializeField]
    private int energy;

    private int healthDamage = 10;  // amount of damage to apply when an enemy makes it through defenses

    private enum EnemySpawn
    {
        Pause,      // pause for 1 sec
        MAC,
        Strep,
        Influenza,
        Staph,
        Covid
    }

    // Variables:
    /*private enum gameState    // we might not end up needing this
    {
        MainMenu,
        Round1,
        Round2,
        Round3,
        Round4,
        ImmuneSysFail,
        EndGame
    }

    private gameState state;*/
    [SerializeField]
    private Transform enemyStart;
    [SerializeField]
    private GameObject macPrefab;
    [SerializeField]
    private GameObject strepPrefab;
    [SerializeField]
    private GameObject covidPrevab;
    [SerializeField]
    private GameObject staphPrefab;
    [SerializeField]
    private Tilemap floor;

    private List<List<EnemySpawn>> rounds = new List<List<EnemySpawn>>()
    {
        new List<EnemySpawn>()   // round 0
        {
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Strep,
            EnemySpawn.Strep,
            EnemySpawn.Strep,
        },
        new List<EnemySpawn>()   // round 1
        {
            EnemySpawn.Strep,
            EnemySpawn.Strep,
            EnemySpawn.Strep,
            EnemySpawn.Strep,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
        },
        new List<EnemySpawn>()   // round 2
        {
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Strep,
            EnemySpawn.Strep,
            EnemySpawn.Strep,
            EnemySpawn.Strep,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Covid,
        },
        new List<EnemySpawn>()   // round 3
        {
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Staph,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.Pause,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.MAC,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
            EnemySpawn.Covid,
        }
    };

    private HashSet<MovingAttacker> currentSpawned = new HashSet<MovingAttacker>();

    // Start is called before the first frame update
    void Start()
    {
        // Queue up welcome screen
        // state = gameState.MainMenu;

        StartCoroutine(BeginGame());
    }

    // Player has clicked start to initiate game
    IEnumerator BeginGame()
    {
        // Replenish health and energy
        health = 100;
        energy = 70;

        print("15 seconds until first round start");
        yield return new WaitForSeconds(15);

        for(int i = 0; i < rounds.Count; i++)
        {
            if (health > 0)
            {
                print("Starting round " + i);
                StartCoroutine(StartRound(i));
                yield return new WaitForFixedUpdate();  // make sure we're not in a race condition with the first enemy spawning
                while (currentSpawned.Count > 0 && health > 0)    // wait until all spawned enemies have disappeared OR health hits 0
                {
                    yield return new WaitForFixedUpdate();
                }
                if (health > 0)
                {
                    print("Round " + i + " success! 15 seconds until next round.");
                    yield return new WaitForSeconds(15);
                } else
                {
                    break;      // breaks from the parent loop; game over state
                }
            }
        }
        if(health > 0)      // we've exited the for loop; is it because we won or we lost?
        {
            print("You win!");
        } else
        {
            print("Immune system failure!");
        }
        EndGame();
    }

    public IEnumerator StartRound(int num)
    {
        currentSpawned.Clear();
        MovingAttacker newSpawn;
        foreach(EnemySpawn enemy in rounds[num])
        {
            newSpawn = null;
            switch (enemy)
            {
                case EnemySpawn.MAC:
                    newSpawn = EnemySetup(GameObject.Instantiate(macPrefab));
                    break;
                case EnemySpawn.Strep:
                    newSpawn = EnemySetup(GameObject.Instantiate(strepPrefab));
                    break;
                case EnemySpawn.Covid:
                    newSpawn = EnemySetup(GameObject.Instantiate(covidPrevab));
                    break;
                case EnemySpawn.Staph:
                    newSpawn = EnemySetup(GameObject.Instantiate(staphPrefab));
                    break;
                case EnemySpawn.Pause:
                default:
                    yield return new WaitForSeconds(1);
                    break;
            }
            if(newSpawn != null)
            {
                currentSpawned.Add(newSpawn);
                yield return new WaitForFixedUpdate();
                while(newSpawn.currentTilePos == floor.WorldToCell(enemyStart.position))
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }

    private void EndGame()
    {
        print("End of game!");
    }

    /*
     *      Spawn in mobs from prefabs
     */

    private MovingAttacker EnemySetup(GameObject enemy)
    {
        MovingAttacker movingAttacker;
        enemy.transform.position = enemyStart.position;
        movingAttacker = enemy.GetComponent<MovingAttacker>();
        movingAttacker.Floor = floor;
        movingAttacker.gameManager = this;
        return movingAttacker;
    }

    /*
     *      Getter/Setter Methods
     */

    public int GetHealth()
    {
        return health;
    }

    public void ReduceHealth()
    {
        health -= healthDamage;
        if(health < 0)
        {
            health = 0; // keep health from being negative
        }               // we don't need to handle death procedure here; main game loop checks health
        print("Current health: " + health);
    }

    public int getEnergy()
    {
        return energy;
    }

    public void reduceEnergy(int reduction)
    {
        energy -= reduction;
        print("Current energy: " + energy);
    }

    public void ReportEnemyDeath(MovingAttacker enemy, int energyReward)
    {
        currentSpawned.Remove(enemy);
        energy += energyReward;
        print("Current energy: " + energy);
    }
}
